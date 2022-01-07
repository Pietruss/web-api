using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using app1API.Entities;
using Microsoft.AspNetCore.Hosting;
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace app1API.IntegrationTests
{
    public class RestaurantControllerTests: IClassFixture<WebApplicationFactory<Startup>>
    {
        private HttpClient _httpClient;

        public RestaurantControllerTests(WebApplicationFactory<Startup> factory)
        {
            _httpClient = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    var dbContextOptions = services.SingleOrDefault(service =>
                        service.ServiceType == typeof(DbContextOptions<RestaurantDbContext>));

                    services.Remove(dbContextOptions);

                    services.AddDbContext<RestaurantDbContext>(options =>
                    {
                        options.UseInMemoryDatabase("RestaurantDb");
                    });
                });
            }).CreateClient();
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
    }
}
