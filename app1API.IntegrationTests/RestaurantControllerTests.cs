using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using app1API.Entities;
using app1API.IntegrationTests.Helpers;
using app1API.Models;
using Microsoft.AspNetCore.Hosting;
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace app1API.IntegrationTests
{
    public class RestaurantControllerTests: IClassFixture<WebApplicationFactory<Startup>>
    {
        private HttpClient _httpClient;
        private WebApplicationFactory<Startup> _factory;

        public RestaurantControllerTests(WebApplicationFactory<Startup> factory)
        {
            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    var dbContextOptions = services.SingleOrDefault(service =>
                        service.ServiceType == typeof(DbContextOptions<RestaurantDbContext>));

                    services.Remove(dbContextOptions);

                    services.AddSingleton<IPolicyEvaluator, FakePolicyEvaluator>();

                    services.AddMvc(option => option.Filters.Add(new FakeUserFilter()));

                    services.AddDbContext<RestaurantDbContext>(options =>
                    {
                        options.UseInMemoryDatabase("RestaurantDb");
                    });
                });
            });
            _httpClient = _factory.CreateClient();
    }

        private void SeedRestaurant(Restaurant restaurant)
        {
            var scopeFactory = _factory.Services.GetService<IServiceScopeFactory>();
            using var scope = scopeFactory.CreateScope();
            var _dbContext = scope.ServiceProvider.GetService<RestaurantDbContext>();

            _dbContext.Restaurants.Add(restaurant);
            _dbContext.SaveChanges();
        }


        [Theory]
        [InlineData("pageSize=5&pageNumber=1")]
        [InlineData("pageSize=10&pageNumber=2")]
        [InlineData("pageSize=15&pageNumber=3")]
        public async Task GetAll_WithQueryParameters_ReturnsOkResult(string queryParams)
        {
            //act
            var response = await _httpClient.GetAsync($"/api/restaurant?" + queryParams);

            //assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        }

        [Theory]
        [InlineData("pageSize=155&pageNumber=3")]
        [InlineData("pageSize=105&pageNumber=3")]
        public async Task GetAll_WithInvalidQueryParams_ReturnsBadRequest(string queryParams)
        {
            //act
            var response = await _httpClient.GetAsync($"/api/restaurant?" + queryParams);

            //assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task CreateRestaurant_WithValidModel_ReturnsCreatedStatus()
        {
            //arrange 
            var model = new CreateRestaurantDto()
            {
                Name = "testRestaurant",
                City = "Kraków",
                Street = "Długa 5"
            };

            var httpContent = model.ToJsonHttpContent();

            //act
            var response = await _httpClient.PostAsync("/api/restaurant", httpContent);

            //assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);
            response.Headers.Location.Should().NotBeNull();
        }

        [Fact]
        public async Task CreateRestaurant_WithInvalidModel_ReturnsBadRequest()
        {
            //arrange
            var model = new CreateRestaurantDto()
            {
                ContactEmail = "test@test.gmail",
                ContactNumber = "999 888 777"
            };

            var httpContent = model.ToJsonHttpContent();

            //act
            var response = await _httpClient.PostAsync("/api/restaurant", httpContent);

            //assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Delete_ForRestaurantOwner_ReturnsNoContent()
        {
            //arrange
            var restaurant = new Restaurant()
            {
                CreatedById = 1,
                Name = "Test"
            };

            //seed
            SeedRestaurant(restaurant);

            //act
            var response = await _httpClient.DeleteAsync("/api/restaurant/" + restaurant.Id);

            //assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NoContent);

        }

        [Fact]
        public async Task Delete_ForNonRestaurantOwner_ReturnsForbidden()
        {
            //arrange
            var restaurant = new Restaurant()
            {
                CreatedById = 5,
                Name = "Test"
            };

            //seed
            SeedRestaurant(restaurant);

            //act
            var response = await _httpClient.DeleteAsync("/api/restaurant/" + restaurant.Id);

            //assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Forbidden);

        }

        [Fact]
        public async Task Delete_ForNonExistingRestaurant_ReturnsNotFound()
        {
            //act
            var response = await _httpClient.DeleteAsync("/api/restaurant/987");

            //assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
        }
    }
}
