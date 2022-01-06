using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Threading.Tasks;
using app1API.Authorization;
using app1API.Entities;
using app1API.Exceptions;
using app1API.Models;
using app1API.Models.PageResults;
using app1API.Models.RestaurantQuery;
using app1API.Models.SortDirection;
using app1API.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace app1API
{
    public class RestaurantService : IRestaurantService
    {
        private readonly RestaurantDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly ILogger<RestaurantService> _logger;
        private readonly IAuthorizationService _authorizationService;
        private readonly IUserContextService _contextService;

        public RestaurantService(RestaurantDbContext dbContext, IMapper mapper, ILogger<RestaurantService> logger, IAuthorizationService authorizationService, IUserContextService contextService)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _logger = logger;
            _authorizationService = authorizationService;
            _contextService = contextService;
        }
        public RestaurantDto GetById(int id)
        {
            var restaurant = _dbContext
                .Restaurants
                .Include(r => r.Address)
                .Include(r => r.Dishes)
                .FirstOrDefault(x => x.Id == id);

            return restaurant == null ? throw new NotFoundException("Restaurant not found") : _mapper.Map<RestaurantDto>(restaurant);
        }

        public PageResults<RestaurantDto> GetAll(RestaurantQuery query)
        {
            var baseQuery =
                _dbContext
                    .Restaurants
                    .Include(r => r.Address)
                    .Include(r => r.Dishes)
                    .Where(r => query.SearchPhrase == null || (r.Description.ToLower().Contains(query.SearchPhrase.ToLower())
                                || r.Name.ToLower().Contains(query.SearchPhrase.ToLower())));

            if (!string.IsNullOrEmpty(query.SortBy))
            {
                var columnsSelectors = new Dictionary<string, Expression<Func<Restaurant, object>>>()
                {
                    {nameof(Restaurant.Name), r => r.Name},
                    {nameof(Restaurant.Description), r => r.Description},
                    {nameof(Restaurant.Category), r => r.Category}
                };

                var selectedColumn = columnsSelectors[query.SortBy];

                baseQuery = query.SortDirection == SortDirection.ASC
                    ? baseQuery.OrderBy(selectedColumn)
                    : baseQuery.OrderByDescending(selectedColumn);
            }

            var restaurants = baseQuery
                    .Skip(query.PageSize * (query.PageNumber - 1))
                    .Take(query.PageSize)
                    .ToList();

            var totalNumberCount = baseQuery.Count();

            var restaurantDtos = _mapper.Map<List<RestaurantDto>>(restaurants);

            var pageResults = new PageResults<RestaurantDto>(restaurantDtos, totalNumberCount, query.PageSize, query.PageNumber);

            return pageResults;
        }

        public int Create(CreateRestaurantDto restaurantDto)
        {
            var restaurant = _mapper.Map<Restaurant>(restaurantDto);
            restaurant.CreatedById = _contextService.GetUserId;
            _dbContext.Restaurants.Add(restaurant);
            _dbContext.SaveChanges();

            return restaurant.Id;
        }

        public void Delete(int id)
        {
            _logger.LogWarning($"Restuarant with id: {id} DELETE action invoked.");
            var restaurant = _dbContext.Restaurants.FirstOrDefault(x => x.Id == id);

            if (restaurant is null)
                throw new NotFoundException("Restaurant not found");

            var authorizationResult = _authorizationService.AuthorizeAsync(_contextService.User, restaurant,
                new ResourceOperationRequirement(ResourceOperationEnum.ResourceOperation.Delete)).Result;

            if (!authorizationResult.Succeeded)
            {
                throw new ForbidException();
            }

            _dbContext.Restaurants.Remove(restaurant);
            _dbContext.SaveChanges();

        }

        public void Update(UpdateRestaurantDto updateRestaurantDto, int id)
        {
            
            var restaurant = _dbContext.Restaurants.FirstOrDefault(x => x.Id == id);

            if (restaurant is null)
                throw new NotFoundException("Restaurant not found");

            var authorizationResult = _authorizationService.AuthorizeAsync(_contextService.User, restaurant,
                new ResourceOperationRequirement(ResourceOperationEnum.ResourceOperation.Update)).Result;

            if (!authorizationResult.Succeeded)
            {
                throw new ForbidException();
            }

            restaurant.Description = updateRestaurantDto.Description;
            restaurant.HasDelivery = updateRestaurantDto.HasDelivery;
            restaurant.Name = updateRestaurantDto.Name;

            _dbContext.SaveChanges();

        }
    }
}
