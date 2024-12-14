using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BaseLibrary.Responses;
using BaseLibrary.DTOs;
using BaseLibrary.Entities;

namespace ClientLibrary.Services.contract
{
    public interface IuserAccountService
    {
        Task<GeneralResponse> CreateAsync(Register user);
        Task<LoginResponse> SignInAsync(Login user);
        Task<LoginResponse> RefreshTokenAsync(RefreshToken token);
        Task<List<ManageUsers>> GetUsers();
        Task<GeneralResponse> UpdateUser(ManageUsers user);
        Task<List<SystemRole>> GetRoles();
        Task<GeneralResponse> DeleteUser(int id);

    }
}
