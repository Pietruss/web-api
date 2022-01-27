using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using app1API.Models.Login;
using app1API.Models.User;
using app1API.Services.User;
using Microsoft.AspNetCore.Mvc;

namespace app1API.Controllers
{
    [Route("api/account")]
    [ApiController]
    public class AccountController: ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpPost("register")]
        public ActionResult RegisterUser([FromBody] RegisterUserDto registerUser)
        {
            _accountService.RegisterUser(registerUser);

            return Ok(); 
        }

        [HttpPost("login")]
        public ActionResult Login([FromBody] LoginDto dto)
        {
            string token = _accountService.GenerateJwt(dto);
            return Ok(token);
        }
    }
}
