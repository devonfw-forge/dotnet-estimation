using System;
using Xunit;
using Moq;
using Devon4Net.Application.WebAPI.Implementation.Business.SessionManagement.Service;
using Devon4Net.Application.WebAPI.Implementation.Domain.Entities;
using System.Collections.Generic;
using System.Linq;
using Task = Devon4Net.Application.WebAPI.Implementation.Domain.Entities.Task;
using Devon4Net.Infrastructure.LiteDb.Repository;
using FluentAssertions;
using Devon4Net.Application.WebAPI.Implementation.Business.SessionManagement.Dtos;
using Xunit.Abstractions;

namespace Devon4Net.Test.xUnit.Test.UnitTest.Management.Controllers
{
    public class SessionServiceTests : UnitTest
    {
        private readonly Mock<ILiteDbRepository<Session>> repositoryStub = new Mock<ILiteDbRepository<Session>>();
        private readonly Random rnd = new();
        private readonly ITestOutputHelper output;

        public SessionServiceTests(ITestOutputHelper output)
        {
            this.output = output;
        }
        
        [Fact]
        public async void GetSession_WithPopulatedRepo_ReturnsTheSearchedSession()
        {
            //Arrange
            var ExpectedSession = CreateRandomSession(2);
            repositoryStub.Setup(repo => repo.GetFirstOrDefault(
                It.IsAny<LiteDB.BsonExpression>()
            ))
                .Returns(ExpectedSession);

            var sessionService = new SessionService(repositoryStub.Object);

            //Act
            var actualSession2 = await sessionService.GetSession(2);

            //Assert
            actualSession2.Should().BeEquivalentTo(ExpectedSession, options => options.ComparingByMembers<Session>());
        }
        

        [Theory]
        [InlineData(1, "Task1", Status.Open)]
        [InlineData(1, "Task1", Status.Evaluated)]
        [InlineData(1, "Task1", Status.Suspended)]
        [InlineData(1, "Task1", Status.Ended)]
        public async void ChangeTaskStatus_WithSameStatus_ReturnsNoStatusChanges(long sessionId, string statusId, Status status)
        {
            //Arrange
            var expectedTaskStatusChangeDtoResult = new List<TaskStatusChangeDto>() { };
            var destinationStatus = new TaskStatusChangeDto()
            {
                Id = statusId,
                Status = status
            };

            //Arrange the Mock Repository with a Session
            var repoStub = new Mock<ILiteDbRepository<Session>>();
            var InitialSession = CreateRandomSession(1);
            repositoryStub.Setup(repo => repo.GetFirstOrDefault(
                It.IsAny<LiteDB.BsonExpression>()
            ))
                .Returns(InitialSession);

            var service = new SessionService(repositoryStub.Object);

            //Act
            var (modified, modifiedTasks) = await service.ChangeTaskStatus(sessionId, destinationStatus);

            //Assert
            //Assert that the delivered indicator whether changes were made is false, since no changes are expected
            Assert.False(modified);
            //Assert that the list of modified TaskStatusChangeDtos is empty
            modifiedTasks.Should().BeEquivalentTo(expectedTaskStatusChangeDtoResult, options => options.ComparingByMembers<TaskStatusChangeDto>());
        }

        
        [Theory]
        [InlineData(1, "Task1", Status.Suspended)]
        public async void ChangeTaskStatus_OpenToSuspended_ReturnsTheChangedTaskStatus(long sessionId, string statusId, Status status)
        {
            //Arrange
            //Arrange the expected Result
            var expectedTaskStatusChangeDtoResult = new List<TaskStatusChangeDto>() { new TaskStatusChangeDto () { Id = statusId, Status = status } };
            var destinationStatus = new TaskStatusChangeDto()
            {
                Id = statusId,
                Status = status
            };

            //Arrange the Mock Repository with a Session
            var repoStub = new Mock<ILiteDbRepository<Session>>(MockBehavior.Strict);
            var InitialSession = CreateRandomSession(1);
            var firstTask = InitialSession.Tasks[0];
            firstTask.Id = statusId;
            firstTask.Status = Status.Open;

            repositoryStub.Setup(repo => repo.GetFirstOrDefault(
                It.IsAny<LiteDB.BsonExpression>()
            ))
                .Returns(InitialSession);

            repositoryStub.Setup(repo => repo.Update(
                It.IsAny<Session>()
            ))
                .Returns(true);

            var service = new SessionService(repositoryStub.Object);

            //Act
            var (modified, modifiedTasks) = await service.ChangeTaskStatus(sessionId, destinationStatus);

            //Assert 
            //Assert that the delivered indicator whether changes were made is false, since no changes are expected
            Assert.True(modified);
            //Assert that the list of modified TaskStatusChangeDtos delivers a suspended Status
            modifiedTasks.Should().BeEquivalentTo(expectedTaskStatusChangeDtoResult, options => options.ComparingByMembers<TaskStatusChangeDto>());
        }

        [Theory]
        [InlineData(1, "Task1", Status.Ended)]
        public async void ChangeTaskStatus_EvaluatedToEnded_ReturnsTheChangedTaskStatus(long sessionId, string statusId, Status status)
        {
            //Arrange
            //Arrange the expected Result
            var expectedTaskStatusChangeDtoResult = new List<TaskStatusChangeDto>() { new TaskStatusChangeDto() { Id = statusId, Status = status } };
            var destinationStatus = new TaskStatusChangeDto()
            {
                Id = statusId,
                Status = status
            };

            //Arrange the Mock Repository with a Session
            var repoStub = new Mock<ILiteDbRepository<Session>>(MockBehavior.Strict);
            var InitialSession = CreateRandomSession(1);
            var firstTask = InitialSession.Tasks[0];
            firstTask.Id = statusId;
            firstTask.Status = Status.Evaluated;

            repositoryStub.Setup(repo => repo.GetFirstOrDefault(
                It.IsAny<LiteDB.BsonExpression>()
            ))
                .Returns(InitialSession);

            repositoryStub.Setup(repo => repo.Update(
                It.IsAny<Session>()
            ))
                .Returns(true);

            var service = new SessionService(repositoryStub.Object);

            //Act
            var (modified, modifiedTasks) = await service.ChangeTaskStatus(sessionId, destinationStatus);

            //Assert 
            //Assert that the delivered indicator whether changes were made is false, since no changes are expected
            Assert.True(modified);
            //Assert that the list of modified TaskStatusChangeDtos delivers a suspended Status
            modifiedTasks.Should().BeEquivalentTo(expectedTaskStatusChangeDtoResult, options => options.ComparingByMembers<TaskStatusChangeDto>());
        }

        [Theory]
        [InlineData(1, "Task1", Status.Ended)]
        public async void ChangeTaskStatus_SuspendedToEnded_ReturnsTheChangedTaskStatus(long sessionId, string statusId, Status status)
        {
            //Arrange
            //Arrange the expected Result
            var expectedTaskStatusChangeDtoResult = new List<TaskStatusChangeDto>() { new TaskStatusChangeDto() { Id = statusId, Status = status } };
            var destinationStatus = new TaskStatusChangeDto()
            {
                Id = statusId,
                Status = status
            };

            //Arrange the Mock Repository with a Session
            var repoStub = new Mock<ILiteDbRepository<Session>>(MockBehavior.Strict);
            var InitialSession = CreateRandomSession(1);
            var firstTask = InitialSession.Tasks[0];
            firstTask.Id = statusId;
            firstTask.Status = Status.Suspended;

            repositoryStub.Setup(repo => repo.GetFirstOrDefault(
                It.IsAny<LiteDB.BsonExpression>()
            ))
                .Returns(InitialSession);

            repositoryStub.Setup(repo => repo.Update(
                It.IsAny<Session>()
            ))
                .Returns(true);

            var service = new SessionService(repositoryStub.Object);

            //Act
            var (modified, modifiedTasks) = await service.ChangeTaskStatus(sessionId, destinationStatus);

            //Assert 
            //Assert that the delivered indicator whether changes were made is false, since no changes are expected
            Assert.True(modified);
            //Assert that the list of modified TaskStatusChangeDtos delivers a suspended Status
            modifiedTasks.Should().BeEquivalentTo(expectedTaskStatusChangeDtoResult, options => options.ComparingByMembers<TaskStatusChangeDto>());
        }


        [Fact]
        public async void AddUserToSession_WithPopulatedRepo_ReturnsTrue()
        {
            //Arrange
            var newUser = new User { Id = "Vlad", Role = 0 };
            var InitialSession = CreateRandomSession(1);

            repositoryStub.Setup(repo => repo.GetFirstOrDefault(
                It.IsAny<LiteDB.BsonExpression>()
            ))
                .Returns(InitialSession);

            repositoryStub.Setup(repo => repo.Update(
                It.IsAny<Session>()
            ))
                .Returns(true);

            var Controller = new SessionService(repositoryStub.Object);
            var InitialUsers = InitialSession.Users;

            //Act
            var UserAdded = await Controller.AddUserToSession(1, newUser.Id, newUser.Role);
            var actualSession = await Controller.GetSession(1);
            var resultUsers = actualSession.Users;
            var LastUser = resultUsers.Last<User>();

            //Assert
            Assert.True(UserAdded);
            resultUsers.Last<User>().Should().BeEquivalentTo(newUser);
        }

        private Session CreateRandomSession(long sessionId)
        {
            IList<User> Users = new List<User>();
            Users.Add(CreateRandomUser());
            Users.Add(CreateRandomUser());
            Users.Add(CreateRandomUser());
            IList<Task> Tasks = new List<Task>();
            Tasks.Add(CreateRandomTask());
            Tasks.Add(CreateRandomTask());
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
                ExpiresAt = DateTime.Now.AddDays(1),
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
    }
}