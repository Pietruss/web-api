using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using app1API.Entities;
using app1API.Models;
using app1API.Models.RestaurantQuery;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace app1API.Controllers
{
    [Route("api/restaurant")]
    [ApiController]
    [Authorize]
    public class RestaurantController: ControllerBase
    {
        private readonly IRestaurantService _restaurantService;

        public RestaurantController(IRestaurantService restaurantService)
        {
            _restaurantService = restaurantService;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Restaurant>> GetAll([FromQuery] RestaurantQuery query)
        {
            var restaurantsDtos = _restaurantService.GetAll(query);

            return Ok(restaurantsDtos);
        }

        [HttpGet]
        [Route("{id}")]
        [AllowAnonymous]
        public ActionResult<IEnumerable<RestaurantDto>> Get([FromRoute]int id)
        {
            var restaurant = _restaurantService.GetById(id);

            return Ok(restaurant);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public ActionResult CreateRestaurant([FromBody] CreateRestaurantDto dto)
        {
            var userId = int.Parse(User.FindFirst(x => x.Type == ClaimTypes.NameIdentifier).Value);
            var id =_restaurantService.Create(dto);

            return Created($"/api/restaurant/{id}", null);
        }

        [HttpDelete("{id}")]
        public ActionResult Delete([FromRoute] int id)
        {
            _restaurantService.Delete(id);

            return NoContent();
        }

        [HttpPut("{id}")]
        public ActionResult Update([FromRoute] int id, [FromBody] UpdateRestaurantDto updateRestaurantDto)
        {
            _restaurantService.Update(updateRestaurantDto, id);

            return Ok();
            
        }
    }
}
