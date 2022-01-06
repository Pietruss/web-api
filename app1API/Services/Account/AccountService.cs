using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using app1API.Entities;
using app1API.Exceptions;
using app1API.Models.Login;
using app1API.Models.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace app1API.Services.User
{
    public class AccountService: IAccountService
    {
        private readonly RestaurantDbContext _dbContext;
        private readonly IPasswordHasher<Entities.User> _passwordHasher;
        private readonly AuthenticationSettings _authenticationSettings;

        public AccountService(RestaurantDbContext dbContext, IPasswordHasher<Entities.User> passwordHasher, AuthenticationSettings authenticationSettings)
        {
            _dbContext = dbContext;
            _passwordHasher = passwordHasher;
            _authenticationSettings = authenticationSettings;
        }
        public void RegisterUser(RegisterUserDto registerUser)
        {
            var newUser = new Entities.User()
            {
                Email = registerUser.Email,
                Nationality = registerUser.Nationality,
                DateOfBirth = registerUser.DateOfBirth,
                RoleId = registerUser.RoleId
            };

            var hasedPassword =_passwordHasher.HashPassword(newUser, registerUser.Password);
            newUser.PasswordHash = hasedPassword;
            _dbContext.User.Add(newUser);
            _dbContext.SaveChanges();
        }

        public string GenerateJwt(LoginDto dto)
        {
            var user = _dbContext.User
                .Include(x => x.Role)
                .FirstOrDefault(x => x.Email == dto.Email);

            if (user is null)
            {
                throw new BadRequestException("Invalid username or password.");
            }

            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);

            if (result == PasswordVerificationResult.Failed)
            {
                throw new BadRequestException("Invalid username or password.");
            }

            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
                new Claim(ClaimTypes.Role, $"{user.Role.Name}"),
                new Claim("DateOfBirth", user.DateOfBirth.Value.ToString("yyyy-MM-dd"))
            };

            if (!string.IsNullOrEmpty(user.Nationality))
            {
                claims.Add(
                    new Claim("Nationality", user.Nationality)
                    );
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_authenticationSettings.JwtKey));
            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddDays(_authenticationSettings.JwtExpireDays);

            var token = new JwtSecurityToken(_authenticationSettings.JwtIssuer,
                _authenticationSettings.JwtIssuer,
                claims,
                expires: expires,
                signingCredentials: cred);

            var tokenHandler = new JwtSecurityTokenHandler();
            return tokenHandler.WriteToken(token);

        }
    }
}
