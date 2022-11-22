using System;
using System.Collections.Generic;
using Xunit;
using Moq;
using Devon4Net.Application.WebAPI.Implementation.Business.SessionManagement.Service;
using System.Threading.Tasks;
using Devon4Net.Application.WebAPI.Implementation.Domain.Entities;
using System.Linq;
using Devon4Net.Application.WebAPI.Implementation.Business.SessionManagement.Controllers;
using Devon4Net.Application.WebAPI.Implementation.Business.SessionManagement.Dtos;
using Devon4Net.Application.WebAPI.Implementation.Business.SessionManagement.Converters;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;

namespace Devon4Net.Test.xUnit.Test.UnitTest.Management.Controllers
{
    public class SessionControllerTests : UnitTest
    {
        /*
        private readonly Random rand = new();
        [Fact]
        public async Task DishSearch_WithGivenCategoryFilterCriteria_ReturnsAllCorrespondingDishes()
        {
            //Arrange
            var Dishes = new[] { CreateRandomDish(), CreateRandomDish(), CreateRandomDish() };
            var randomPrice = (decimal)rand.NextDouble();
            var minLikes = rand.Next();
            var searchBy = Guid.NewGuid().ToString();
            var ExpectedCategories = new[] { Dishes[0].Category, Dishes[1].Category };
            IList<string> categoryObjIds = new List<string>() { ExpectedCategories[0].FirstOrDefault().Id, ExpectedCategories[1].FirstOrDefault().Id };

            var categorySearchDtoIds = categoryObjIds.Select(c => new CategorySearchDto
            {
                Id = c,
            }).ToList().ToArray();

            IList<Dish> ExpectedDishes = Dishes.Where(dish => dish.Category.Any(cat => categoryObjIds.Contains(cat.Id))).ToList();

            var serviceStub = new Mock<IDishService>();
            serviceStub.Setup(repo => repo.GetDishesMatchingCriteria(
                    //It.IsAny<decimal>(),
                    It.IsAny<decimal>(),
                    It.IsAny<int>(),
                    It.IsAny<string>(),
                    It.Is<IList<string>>(categoryIdList => categoryIdList.All(cat => categoryObjIds.Contains(cat)))
                    )).Returns(Task.FromResult(ExpectedDishes));

            var controller = new DishController(serviceStub.Object);
            //Act
            var filterDto = new FilterDtoSearchObjectDto { MaxPrice = randomPrice, SearchBy = searchBy, MinLikes = minLikes, Categories = categorySearchDtoIds };
            var ExpectedResult = new ResultObjectDto<DishDtoResult> { };
            ExpectedResult.content = ExpectedDishes.Select(DishConverter.EntityToApi).ToList();
            ExpectedResult.Pagination.Total = ExpectedDishes.Count();

            var result = (ObjectResult)await controller.DishSearch(filterDto);

            //Assert
            var compareObj = JsonConvert.SerializeObject(ExpectedResult);
            Assert.Equal(compareObj, result.Value);
        }

        [Fact]
        public async Task DishSearch_WithGivenMaxPriceFilterCriteria_ReturnsAllCorrespondingDishes()
        {
            //Arrange
            var Dishes = new[] { CreateRandomDish(), CreateRandomDish(), CreateRandomDish() };
            var DishPrices = new[] { Dishes[0].Price, Dishes[1].Price, Dishes[2].Price };

            //Price Filter is set to max. Therefore all dishes will be taken into account.
            var expectedPrice = DishPrices.Max();
            var minLikes = rand.Next();
            var searchBy = Guid.NewGuid().ToString();
            var ExpectedCategories = Array.Empty<ICollection<Category>>();
            IList<string> categoryObjIds = new List<string>() { };
            var categorySearchDtoIds = categoryObjIds.Select(c => new CategorySearchDto
            {
                Id = c,
            }).ToList().ToArray();

            IList<Dish> ExpectedDishes = Dishes.Where(dish => dish.Price <= expectedPrice).ToList();

            var serviceStub = new Mock<IDishService>();
            serviceStub.Setup(repo => repo.GetDishesMatchingCriteria(
                    //It.IsAny<decimal>(),
                    It.Is<decimal>(price => price == expectedPrice),
                    It.IsAny<int>(),
                    It.IsAny<string>(),
                    It.IsAny<IList<string>>())).Returns(Task.FromResult(ExpectedDishes));

            var controller = new DishController(serviceStub.Object);
            //Act
            var filterDto = new FilterDtoSearchObjectDto { MaxPrice = expectedPrice, SearchBy = searchBy, MinLikes = minLikes, Categories = categorySearchDtoIds };
            var ExpectedResult = new ResultObjectDto<DishDtoResult> { };
            ExpectedResult.content = ExpectedDishes.Select(DishConverter.EntityToApi).ToList();
            ExpectedResult.Pagination.Total = ExpectedDishes.Count();

            var result = (ObjectResult)await controller.DishSearch(filterDto);

            //Assert
            var compareObj = JsonConvert.SerializeObject(ExpectedResult);
            Assert.Equal(compareObj, result.Value);
        }

        [Fact]
        public async Task DishSearch_WithGivenMinPriceFilterCriteria_ReturnsAllCorrespondingDishes()
        {
            //Arrange
            var Dishes = new[] { CreateRandomDish(), CreateRandomDish(), CreateRandomDish() };
            var DishPrices = new[] { Dishes[0].Price, Dishes[1].Price, Dishes[2].Price };

            //Price Filter is set to min. Therefore only the cheapest dish(es) will be taken into account.
            var expectedPrice = DishPrices.Min();
            var minLikes = rand.Next();
            var searchBy = Guid.NewGuid().ToString();
            var ExpectedCategories = Array.Empty<ICollection<Category>>();
            IList<string> categoryObjIds = new List<string>() { };
            var categorySearchDtoIds = categoryObjIds.Select(c => new CategorySearchDto
            {
                Id = c,
            }).ToList().ToArray();

            IList<Dish> ExpectedDishes = Dishes.Where(dish => dish.Price <= expectedPrice).ToList();

            var serviceStub = new Mock<IDishService>();
            serviceStub.Setup(repo => repo.GetDishesMatchingCriteria(
                    //It.IsAny<decimal>(),
                    It.Is<decimal>(price => price == expectedPrice),
                    It.IsAny<int>(),
                    It.IsAny<string>(),
                    It.IsAny<IList<string>>())).Returns(Task.FromResult(ExpectedDishes));

            var controller = new DishController(serviceStub.Object);
            //Act
            var filterDto = new FilterDtoSearchObjectDto { MaxPrice = expectedPrice, SearchBy = searchBy, MinLikes = minLikes, Categories = categorySearchDtoIds };
            var ExpectedResult = new ResultObjectDto<DishDtoResult> { };
            ExpectedResult.content = ExpectedDishes.Select(DishConverter.EntityToApi).ToList();
            ExpectedResult.Pagination.Total = ExpectedDishes.Count();

            var result = (ObjectResult)await controller.DishSearch(filterDto);

            //Assert
            var compareObj = JsonConvert.SerializeObject(ExpectedResult);
            Assert.Equal(compareObj, result.Value);
        }

        [Fact]
        public async Task DishSearch_WithGivenSearchByCriteria_ReturnsAllCorrespondingDishes()
        {
            //Arrange
            var Dishes = new[] { CreateRandomDish(), CreateRandomDish(), CreateRandomDish() };
            var expectedPrice = (decimal)rand.NextDouble();
            var minLikes = rand.Next();
            //Choose randomly one of the Dish names to filter for
            var DishNames = new[] { Dishes[0].Name, Dishes[1].Name, Dishes[2].Name };
            var searchBy = DishNames[rand.Next(0, 3)];

            var ExpectedCategories = Array.Empty<ICollection<Category>>();
            IList<string> categoryObjIds = new List<string>() { };
            var categorySearchDtoIds = categoryObjIds.Select(c => new CategorySearchDto
            {
                Id = c,
            }).ToList().ToArray();

            IList<Dish> ExpectedDishes = Dishes.Where(dish => dish.Name.Contains(searchBy, StringComparison.CurrentCultureIgnoreCase)).ToList();

            var serviceStub = new Mock<IDishService>();
            serviceStub.Setup(repo => repo.GetDishesMatchingCriteria(
                    It.IsAny<decimal>(),
                    It.IsAny<int>(),
                    It.Is<string>(name => name == searchBy),
                    It.IsAny<IList<string>>())).Returns(Task.FromResult(ExpectedDishes));

            var controller = new DishController(serviceStub.Object);
            //Act
            var filterDto = new FilterDtoSearchObjectDto { MaxPrice = expectedPrice, SearchBy = searchBy, MinLikes = minLikes, Categories = categorySearchDtoIds };
            var ExpectedResult = new ResultObjectDto<DishDtoResult> { };
            ExpectedResult.content = ExpectedDishes.Select(DishConverter.EntityToApi).ToList();
            ExpectedResult.Pagination.Total = ExpectedDishes.Count();

            var result = (ObjectResult)await controller.DishSearch(filterDto);

            //Assert
            var compareObj = JsonConvert.SerializeObject(ExpectedResult);
            Assert.Equal(compareObj, result.Value);
        }

        [Fact]
        public async Task DishSearch_WithFullFilterCriteria_ReturnsAllCorrespondingDishes()
        {
            //Arrange
            var Dishes = new[] { CreateRandomDish(), CreateRandomDish(), CreateRandomDish() };
            var DishPrices = new[] { Dishes[0].Price, Dishes[1].Price, Dishes[2].Price };

            //Price Filter is set to max. Therefore all dishes will be taken into account.
            var expectedPrice = DishPrices.Max();
            var minLikes = rand.Next();

            //Choose randomly one of the Dish names to filter for
            var DishNames = new[] { Dishes[0].Name, Dishes[1].Name, Dishes[2].Name };
            var searchBy = DishNames[rand.Next(0, 3)];

            var ExpectedCategories = new[] { Dishes[0].Category, Dishes[1].Category };

            IList<string> categoryObjIds = new List<string>() { ExpectedCategories[0].FirstOrDefault().Id, ExpectedCategories[1].FirstOrDefault().Id };
            var categorySearchDtoIds = categoryObjIds.Select(c => new CategorySearchDto
            {
                Id = c,
            }).ToList().ToArray();

            IList<Dish> ExpectedDishes = Dishes.Where(dish =>
                dish.Category.Any(cat => categoryObjIds.Contains(cat.Id)) &&
                dish.Price <= expectedPrice &&
                dish.Name.Contains(searchBy, StringComparison.CurrentCultureIgnoreCase)).ToList();

            var serviceStub = new Mock<IDishService>();
            serviceStub.Setup(repo => repo.GetDishesMatchingCriteria(
                    //It.IsAny<decimal>(),
                    It.Is<decimal>(price => price == expectedPrice),
                    It.Is<int>(likes => likes == minLikes),
                    It.Is<string>(searchCriteria => searchCriteria == searchBy),
                    It.Is<IList<string>>(categoryIdList => categoryIdList.All(cat => categoryObjIds.Contains(cat)))
                    )).Returns(Task.FromResult(ExpectedDishes));

            var controller = new DishController(serviceStub.Object);
            //Act

            var filterDto = new FilterDtoSearchObjectDto { MaxPrice = expectedPrice, SearchBy = searchBy, MinLikes = minLikes, Categories = categorySearchDtoIds };
            var ExpectedResult = new ResultObjectDto<DishDtoResult> { };
            ExpectedResult.content = ExpectedDishes.Select(DishConverter.EntityToApi).ToList();
            ExpectedResult.Pagination.Total = ExpectedDishes.Count();

            var result = (ObjectResult)await controller.DishSearch(filterDto);

            //Assert
            var compareObj = JsonConvert.SerializeObject(ExpectedResult);
            Assert.Equal(compareObj, result.Value);
        }


        [Fact]
        public async Task DishSearch_WithNullFilterCriteria_ReturnsDefault()
        {
            //Arrange
            IList<Dish> Dishes = new List<Dish> { CreateRandomDish(), CreateRandomDish(), CreateRandomDish() };
            var serviceStub = new Mock<IDishService>();

            serviceStub.Setup(repo => repo.GetDishesMatchingCriteria(
                    It.IsAny<decimal>(),
                    It.IsAny<int>(),
                    It.IsAny<string>(),
                    It.IsAny<IList<string>>())).Returns(Task.FromResult(Dishes));

            var controller = new DishController(serviceStub.Object);
            //Act
            var ExpectedResult = new ResultObjectDto<DishDtoResult> { };
            ExpectedResult.content = Dishes.Select(DishConverter.EntityToApi).ToList();
            ExpectedResult.Pagination.Total = Dishes.Count();

            var result = (ObjectResult)await controller.DishSearch(null);

            //Assert
            var compareObj = JsonConvert.SerializeObject(ExpectedResult);
            Assert.Equal(compareObj, result.Value);
        }


        private Image CreateRandomImage()
        {
            return new()
            {
                Id = Guid.NewGuid().ToString(),
                Content = Guid.NewGuid().ToString(),
                Name = Guid.NewGuid().ToString(),
                MimeType = Guid.NewGuid().ToString(),
                Extension = Guid.NewGuid().ToString(),
                ContentType = rand.Next(),
                ModificationCounter = rand.Next(),
            };
        }


        private Category CreateRandomCategory()
        {
            return new()
            {
                Id = Guid.NewGuid().ToString(),
                Name = Guid.NewGuid().ToString(),
                Description = Guid.NewGuid().ToString(),
                ShowOrder = rand.Next(),
                ModificationCounter = rand.Next(),
            };
        }


        private Dish CreateRandomDish()
        {
            return new()
            {
                Id = Guid.NewGuid().ToString(),
                Name = Guid.NewGuid().ToString(),
                Price = ((decimal)rand.NextDouble() + rand.Next()),
                Image = CreateRandomImage(),
                Category = new[] { CreateRandomCategory(), CreateRandomCategory(), CreateRandomCategory() }
            };
        }
        */
    }
}