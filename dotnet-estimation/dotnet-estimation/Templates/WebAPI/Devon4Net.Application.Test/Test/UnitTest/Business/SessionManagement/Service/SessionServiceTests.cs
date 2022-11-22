using System;
using Xunit;
using Moq;
using Devon4Net.Application.WebAPI.Implementation.Business.SessionManagement.Service;
using Devon4Net.Application.WebAPI.Implementation.Domain.Entities;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Task = Devon4Net.Application.WebAPI.Implementation.Domain.Entities.Task;
using Devon4Net.Infrastructure.LiteDb.Repository;
using Devon4Net.Infrastructure.Test;
using FluentAssertions;
using Devon4Net.Application.WebAPI.Implementation.Business.SessionManagement.Dtos;
using Xunit.Abstractions;

namespace Devon4Net.Test.xUnit.Test.UnitTest.Management.Controllers
{
    public class SessionServiceTests : UnitTest
    {
        //private readonly Mock<ISessionRepository> repositoryStub = new();
        private readonly Mock<ILiteDbRepository<Session>> repositoryStub = new Mock<ILiteDbRepository<Session>>();
        private readonly Random rnd = new();
        private readonly ITestOutputHelper output;

        public SessionServiceTests(ITestOutputHelper output)
        {
            this.output = output;
        }
        /*
        [Fact]
        public async Task<Object> GetSession_WithPopulatedRepo_ReturnsAllSessions()
        {
            //Arrange
            var ExpectedSessions = new[] { CreateRandomSession(1), CreateRandomSession(2), CreateRandomSession(3) };
            repositoryStub.Setup(repo => repo.Get())
                .Returns(ExpectedSessions);

            var controller = new SessionService(repositoryStub.Object);
            //Act
            var actualSessions = await controller.GetSession(1);
            Console.WriteLine("Expected Sessions[0]:\n" + ExpectedSessions[0]);
            Console.WriteLine("Expected Sessions:\n" + ExpectedSessions);
            Console.WriteLine("Actual Session:\n" + actualSessions);
            //Assert
            return actualSessions.Should().BeEquivalentTo(ExpectedSessions[0], options => options.ComparingByMembers<Session>());
        }
        */

        [Fact]
        public async void ChangeTaskStatus_WithSameStatus_ReturnsNoStatusChanges()
        {
            //Arrange
            var InitialSession = new[] { CreateRandomSession(1) };
            repositoryStub.Setup(repo => repo.Get())
                .Returns(InitialSession);

            var Controller = new SessionService(repositoryStub.Object);
            var firstTask = InitialSession[0].Tasks[0];

            output.WriteLine("Hello" + firstTask);
            //Act
            var newStatus = new TaskStatusChangeDto()
            {
                Id = firstTask.Id,
                Status = firstTask.Status,
            };

            var (TaskChanged, modifiedTasks) = await Controller.ChangeTaskStatus(1, newStatus);
            //Assert
            Assert.False(TaskChanged);
        }

        [Fact]
        public async void ChangeTaskStatus_OpenToSuspended_ReturnsTheChangedTaskStatus()
        {
            //Arrange
            var InitialSession = new[] { CreateRandomSession(1) };
            var firstTask = InitialSession[0].Tasks[0];

            //Set the first Task Status to Open
            firstTask.Status = Status.Open;

            repositoryStub.Setup(repo => repo.Get())
                .Returns(InitialSession);

            var Controller = new SessionService(repositoryStub.Object);

            //Act
            var newStatusSuspended = new TaskStatusChangeDto()
            {
                Id = firstTask.Id,
                Status = Status.Suspended,
            };

            var (TaskChanged, modifiedTasks) = await Controller.ChangeTaskStatus(1, newStatusSuspended);
            Assert.False(TaskChanged);

            //Assert that the TaskStatusChangeDto delivers a suspended Status
            foreach (var task in modifiedTasks)
            {
                if(task.Id == firstTask.Id)
                {
                    //resultTaskStatusChangeDto = task.Status.Should().Equals(Status.Suspended);
                    //resultTaskStatusChangeDto =
                    Assert.True(task.Status == Status.Suspended);
                }
            }
            output.WriteLine("Hello" + modifiedTasks.Count);
            //TODO : Check whether the changes took also place in the database.
            //Assert that the Task's status inside the Database was changed to suspended
            //Assert.True(firstTask.Status != Status.Open);
            //var updatedSessionRepo = await Controller.GetSession(1);
            //Assert.NotNull(updatedSessionRepo);
            //Assert.True(updatedSessionRepo.Tasks.Where(task => task.Id == firstTask.Id && task.Status == Status.Suspended).Any());
        }

        [Fact]
        public async void ChangeTaskStatus_EvaluatedToClosed_ReturnsTheChangedTaskStatus()
        {
            //Arrange
            var InitialSession = new[] { CreateRandomSession(1) };
            repositoryStub.Setup(repo => repo.Get())
                .Returns(InitialSession);

            var Controller = new SessionService(repositoryStub.Object);
            var firstTask = InitialSession[0].Tasks[0];

            //Set the first Tasks Status to Evaluated
            firstTask.Status = Status.Evaluated;

            //Act
            var newStatusEnded= new TaskStatusChangeDto()
            {
                Id = firstTask.Id,
                Status = Status.Ended,
            };
            //output.WriteLine("Hello" + firstTask);
            var (TaskChanged, modifiedTasks) = await Controller.ChangeTaskStatus(1, newStatusEnded);
            //Assert.True(TaskChanged);
            output.WriteLine("List" + modifiedTasks.Count);
            //Assert that the TaskStatusChangeDto delivers an Ended Status
            var resultTaskStatusChangeDto = modifiedTasks.Where(task => task.Id == firstTask.Id).ToList();
            Assert.True(resultTaskStatusChangeDto[0].Status.Equals(Status.Ended));

            //Assert that the Task's status inside the Database was changed to suspended
            //var updatedRepositorySession = await Controller.GetSession(1);
            //var updatedTask = updatedRepositorySession.Tasks[0];
            //Assert.Equal(Status.Ended, updatedTask.Status);
        }


        [Fact]
        public async Task<bool> ChangeSessionStatus_SuspendedToSuspended_ReturnsNoStatusChanges()
        {
            //Arrange
            var InitialSession = new[] { CreateRandomSession(1) };
            repositoryStub.Setup(repo => repo.Get())
                .Returns(InitialSession);

            var Controller = new SessionService(repositoryStub.Object);
            var firstTask = InitialSession[0].Tasks[0];

            //Set the first Tasks Status to Suspended
            firstTask.Status = Status.Suspended;

            //Act
            var newStatusSuspended = new TaskStatusChangeDto()
            {
                Id = firstTask.Id,
                Status = Status.Suspended,
            };

            var (TaskChanged, modifiedTasks) = await Controller.ChangeTaskStatus(1, newStatusSuspended);
            if (!TaskChanged)
            {
                return false;
            }

            //Assert that the TaskStatusChangeDto delivers a suspended Status
            var resultTaskStatusChangeDto = modifiedTasks.Where(task => task.Id == firstTask.Id).ToList();
            Assert.True(resultTaskStatusChangeDto[0].Status.Equals(Status.Suspended));

            //Assert that the Task's status inside the Database was changed to suspended
            var updatedRepositorySession = await Controller.GetSession(1);
            var updatedTask = updatedRepositorySession.Tasks[0];
            Assert.Equal(Status.Suspended, updatedTask.Status);

            return true;
        }

        [Fact]
        public async Task<Object> AddUserToSession_WithPopulatedRepo_ReturnsTrue()
        {
            //Arrange
            var InitialSession = new[] { CreateRandomSession(1) };
            repositoryStub.Setup(repo => repo.Get())
                .Returns(InitialSession);

            var Controller = new SessionService(repositoryStub.Object);
            var InitialUsers = InitialSession[0].Users;
            //Act
            var newUser = new User { Id = "Vlad", Role = 0 };
            var UserAdded = await Controller.AddUserToSession(1, newUser.Id, newUser.Role);
            if (!UserAdded)
            {
                return false;
            }
            //Assert
            var JoinedSessions = await Controller.GetSession(1);
            var resultUsers = JoinedSessions.Users;

            var LastUser = resultUsers.Last<User>();

            return !resultUsers.Equals(InitialUsers) && LastUser.Equals(newUser);
        }

        private Session CreateRandomSession(long sessionId)
        {
            IList<User> Users = new List<User>();
            Users.Add(CreateRandomUser());
            Users.Add(CreateRandomUser());
            Users.Add(CreateRandomUser());
            IList<Task> Tasks = new List<Task>();
            Tasks.Add(CreateRandomTask());

            //Mockup estimations by each of the 3 random created users
            Estimation estimation1 = new Estimation { VoteBy = Users[0].Id, Complexity = rnd.Next(13) };
            Estimation estimation2 = new Estimation { VoteBy = Users[1].Id, Complexity = rnd.Next(13) };
            Estimation estimation3 = new Estimation { VoteBy = Users[2].Id, Complexity = rnd.Next(13) };

            Tasks[0].Estimations.Add(estimation1);
            Tasks[0].Estimations.Add(estimation2);
            Tasks[0].Estimations.Add(estimation3);


            //TODO Add to Task.Estimations, one or two Voteby strings of already created Users + random int Complexity vote
            return new()
            {
                Id = sessionId,
                InviteToken = Guid.NewGuid().ToString(),
                ExpiresAt = DateTime.Now.AddMinutes(30),
                Tasks = Tasks,
                Users = Users,
            };
        }

        private Task CreateRandomTask()
        {
            var StatusValues = Enum.GetValues(typeof(Status));
            var Status = (Status)StatusValues.GetValue(rnd.Next(StatusValues.Length));
            return new()
            {
                Id = rnd.Next().ToString(),
                Title = Guid.NewGuid().ToString(),
                Description = Guid.NewGuid().ToString(),
                Url = Guid.NewGuid().ToString(),
                Status = Status,
                CreatedAt = DateTime.Now,
                Estimations = new List<Estimation>(),
                Result = new Result { AmountOfVotes = 0, ComplexityAverage = 0 },
            };
        }

        private User CreateRandomUser()
        {
            var RoleValues = Enum.GetValues(typeof(Role));
            var role = (Role)RoleValues.GetValue(rnd.Next(RoleValues.Length));

            return new()
            {
                Id = Guid.NewGuid().ToString(),
                Role = role,
            };
        }


        /* Example Fact Check
        [Fact]
        public async Task GetDishesByCategory_WithGivenCategories_ReturnsTheCorrespondingCategoryDishes()
        {
            //Arrange
            var Dishes = new[] { CreateRandomUser(), CreateRandomUser(), CreateRandomUser() };
            var ExpectedCategories = new[] { Dishes[0].Category, Dishes[1].Category };
            IList<Dish> ExpectedDishes = new List<Dish> { Dishes[0], Dishes[1] };
            IList<string> Ids = new List<string>() { ExpectedCategories[0].FirstOrDefault().Id, ExpectedCategories[1].FirstOrDefault().Id };

            var controller = new DishService(repositoryStub.Object);

            repositoryStub.Setup(repo => repo.GetDishesByCategory(Ids))
                .ReturnsAsync(ExpectedDishes);
            //Act
            var result = await controller.GetDishesByCategory(Ids);
            //Assert
            result.Should().BeEquivalentTo(ExpectedDishes, options => options.ComparingByMembers<Dish>());
        }
        */
    }
}