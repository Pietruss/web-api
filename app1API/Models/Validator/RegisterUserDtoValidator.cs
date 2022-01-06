using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using app1API.Entities;
using app1API.Models.User;
using FluentValidation;

namespace app1API.Models.Validator
{
    public class RegisterUserDtoValidator: AbstractValidator<RegisterUserDto>
    {
        public RegisterUserDtoValidator(RestaurantDbContext dbContext)
        {
            RuleFor(x => x.Email)
                .NotEmpty().EmailAddress();

            RuleFor(x => x.Password)
                .MinimumLength(6);

            RuleFor(x => x.ConfirmPassword).Equal(e => e.Password);

            RuleFor(x => x.Email)
                .Custom((value, context) =>
                {
                    var emailInUse = dbContext.User.Any(x => x.Email == value);
                    if (emailInUse)
                    {
                        context.AddFailure("Email", "That email is taken");
                    }
                });
        }
    }
}
