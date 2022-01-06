using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using app1API.Models;
using app1API.Models.Owner;

namespace app1API.Services.Owner
{
    public interface IOwnerService
    {
        IEnumerable<OwnerDto> GetAll();
        OwnerDto Get(int Id);
        int Create(CreateOwnerDto createOwner);
        void Delete(int id);
        void Update(UpdateOwnerDto updateOwnerDto, int id);
    }
}
