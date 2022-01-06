using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using app1API.Models.Login;
using app1API.Models.User;

namespace app1API.Services.User
{
    public interface IAccountService
    {
        void RegisterUser(RegisterUserDto registerUser);

        string GenerateJwt(LoginDto dto);
    }
}
