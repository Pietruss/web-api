using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using app1API.Entities;
using app1API.Models;
using app1API.Models.Owner;
using app1API.Services.Owner;
using Microsoft.AspNetCore.Mvc;

namespace app1API.Controllers
{
    [Route("api/owner")]
    [ApiController]
    public class OwnerController: ControllerBase
    {
        private readonly IOwnerService _ownerService;

        public OwnerController(IOwnerService ownerService)
        {
            _ownerService = ownerService;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Owner>> GetAll()
        {
            var ownerDto = _ownerService.GetAll();

            return Ok(ownerDto);
        }

        [HttpGet]
        [Route("{id}")]
        public ActionResult<IEnumerable<OwnerDto>> Get([FromRoute]int id)
        {
            var owner = _ownerService.Get(id);

            return Ok(owner);
        }

        [HttpPost]
        public ActionResult CreateOwner([FromBody] CreateOwnerDto owner)
        {
            var id = _ownerService.Create(owner);

            return Created($"api/owner/{id}", null);
        }

        [HttpDelete]
        [Route("{id}")]
        public ActionResult DeleteOwner([FromRoute]int id)
        {
            _ownerService.Delete(id);

            return NoContent();
        }

        [HttpPut("{id}")]
        public ActionResult Update([FromRoute] int id, [FromBody] UpdateOwnerDto updateOwner)
        {
            _ownerService.Update(updateOwner, id);

            return Ok();
        }
    }
}
