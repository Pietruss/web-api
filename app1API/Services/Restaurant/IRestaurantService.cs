using System.Collections.Generic;
using System.Security.Claims;
using app1API.Models;
using app1API.Models.PageResults;
using app1API.Models.RestaurantQuery;
using Microsoft.EntityFrameworkCore;

namespace app1API
{
    public interface IRestaurantService
    {
        RestaurantDto GetById(int id);
        PageResults<RestaurantDto> GetAll(RestaurantQuery query);
        int Create(CreateRestaurantDto restaurantDto);
        void Delete(int id);
        void Update(UpdateRestaurantDto updateRestaurantDto, int id);
    }
}