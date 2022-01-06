using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using app1API.Entities;
using FluentValidation;

namespace app1API.Models.Validator
{
    public class RestaurantQueryValidator: AbstractValidator<RestaurantQuery.RestaurantQuery>
    {
        private readonly int[] _allowedPageSizes = new [] {5 , 10, 15};

        private readonly string[] _allowedSortByColumns =
            {nameof(Restaurant.Name), nameof(Restaurant.Category), nameof(Restaurant.Description)};

            public RestaurantQueryValidator()
        {
            RuleFor(r => r.PageNumber).GreaterThanOrEqualTo(1);
            RuleFor(r => r.PageSize).Custom((value, context) =>
            {
                if (!_allowedPageSizes.Contains(value))
                {
                    context.AddFailure("PageSize", $"PageSize does not contain [{string.Join(",", _allowedPageSizes)}]");
                }
            });

            RuleFor(x => x.SortBy)
                .Must(value => string.IsNullOrEmpty(value) || _allowedSortByColumns.Contains(value))
                .WithMessage($"Sort by is optional, or must be in [{string.Join(",", _allowedSortByColumns)}]");
        }
    }
}
