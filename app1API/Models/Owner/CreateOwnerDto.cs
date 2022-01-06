using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace app1API.Models.Owner
{
    public class CreateOwnerDto
    {
        [Required]
        public string Name { get; set; }
    }
}
