using BaseLibrary.DTOs;
using BaseLibrary.Entities;
using BaseLibrary.Responses;
using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using serverLibrary.Data;
using serverLibrary.Helper;
using serverLibrary.Respositories.contract;
using System.Security.Cryptography;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Reflection.Metadata;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace serverLibrary.Respositories.Implementations
{
     public class UserAccountRepository(IOptions<JwtSection> config, AppDbContext appDbContext) : IuserAccount
    {
        public async Task<GeneralResponse> CreateAsync(Register user)
        {
            if (user is null)
                return new GeneralResponse(false, "Model is Empty");
            var checkUser = await FindUserByEmail(user.EmailAddress!);
            if (checkUser != null)
                return new GeneralResponse(false, "User Registered Already");
            
            //Save User
            var applicationUser = await AddToDatabase(new ApplicationUser()
            {
                Fullname = user.FullName,
                Email = user.EmailAddress,
                Password = BCrypt.Net.BCrypt.HashPassword(user.Password)


            });

            // Check, Create and assign role
            var checkAdminRole = await appDbContext.SystemRoles.FirstOrDefaultAsync(_ => _.Name!.Equals(Constants.Admin));
            if (checkAdminRole is null)
            {
                var createAdminRole = await AddToDatabase(new SystemRole() { Name = Constants.Admin });
                await AddToDatabase(new UserRole() { RoleId = createAdminRole.Id, UserId = applicationUser.id });
                return new GeneralResponse(true, "Account Created Successfully");
            }

            var checkUserRole = await appDbContext.SystemRoles.FirstOrDefaultAsync(_ => _.Name!.Equals(Constants.User));
            SystemRole response = new();
            if (checkUserRole is null)
            {
                response = await AddToDatabase(new SystemRole() { Name = Constants.User });
                await AddToDatabase(new UserRole() { RoleId = response.Id, UserId = applicationUser.id });
            }
            else
            {
                await AddToDatabase(new UserRole() { RoleId = checkUserRole.Id, UserId = applicationUser.id });
            }
            return new GeneralResponse(true, "Account Created Successfully");
        }

        public async Task<LoginResponse> SignInAsync(Login user)
        {
            if (user is null)
                return new LoginResponse(false, "Model is Empty");
            var applicationUser = await FindUserByEmail(user.EmailAddress);
            if (applicationUser is null)
                return new LoginResponse(false, "User Not found");

            //Verify Password
            if (!BCrypt.Net.BCrypt.Verify(user.Password, applicationUser.Password))
                return new LoginResponse(false, "Email/Password are invalid");
            var getUserRole = await FindUserRole( applicationUser.id);
            if (getUserRole is null)
                return new LoginResponse(false, "user role not found");
            var getRoleName = await FindRoleName(getUserRole.RoleId);
            if (getRoleName is null)
                return new LoginResponse(false, "User Role Not Found");

            string jwtToken = GenerateToken(applicationUser, getRoleName.Name);
            string refreshToken = GenerateRefreshToken();


            // save the refresh token
            var findRefreshToken = await appDbContext.RefreshTokenInfos.FirstOrDefaultAsync(_ => _.UserId == applicationUser.id);
            if(findRefreshToken is not null)
            {
                findRefreshToken!.Token = refreshToken;
                await appDbContext.SaveChangesAsync();
            }
            else
            {
                await AddToDatabase(new RefreshTokenInfo() { Token = refreshToken, UserId = applicationUser.id});
            }
            return new LoginResponse(true,"Login Successfully",jwtToken,refreshToken);


            
        }
        private string GenerateToken(ApplicationUser user, string role)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.Value.Key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var userClaims = new[]
            {
               new Claim(ClaimTypes.NameIdentifier,user.id.ToString()),
               new Claim(ClaimTypes.Name,user.Fullname!),
               new Claim(ClaimTypes.Email,user.Email!),
               new Claim(ClaimTypes.Role, role)
            };
            var token = new JwtSecurityToken(
                issuer : config.Value.Issuer,
                audience : config.Value.Audience,
                claims : userClaims,
                expires : DateTime.Now.AddSeconds(10),
                signingCredentials : credentials
                );
            return new JwtSecurityTokenHandler().WriteToken(token);

        }

        private async Task<UserRole> FindUserRole(int userId) => await appDbContext.UserRoles.FirstOrDefaultAsync(_ => _.UserId == userId);

        private async Task<SystemRole> FindRoleName(int roleId) => await appDbContext.SystemRoles.FirstOrDefaultAsync(_ => _.Id == roleId);

        
        private static string GenerateRefreshToken() => Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));  
        
        private async Task<ApplicationUser> FindUserByEmail(string email)=>
            await appDbContext.ApplicationUsers.FirstOrDefaultAsync(_ => _.Email!.ToLower().Equals(email!.ToLower()));
        
        private async Task<T> AddToDatabase<T>(T model)
        {
            var result = appDbContext.Add(model);
            await appDbContext.SaveChangesAsync();
            return (T)result.Entity;
        }

        public async Task<LoginResponse> RefreshTokenAsync(RefreshToken token)
        {
            if(token is null)
            {
                return new LoginResponse(false, "Model is Empty");
            }
            var findToken = await appDbContext.RefreshTokenInfos.FirstOrDefaultAsync(_ => _.Token!.Equals(token.Token));
            if (findToken is null) return new LoginResponse(false, "Token is null");

            //get user details
            var user = await appDbContext.ApplicationUsers.FirstOrDefaultAsync(_=>_.id== findToken.UserId);
            if (user is null) return new LoginResponse(false, "Refresh could not be generated because user not found");

            var userRole = await FindUserRole(user.id);
            var roleName = await FindRoleName(userRole.RoleId);
            string jwtToken = GenerateToken(user, roleName.Name);
            string refreshToken = GenerateRefreshToken();

            var updateRefreshToken = await appDbContext.RefreshTokenInfos.FirstOrDefaultAsync(_ => _.UserId == user.id);
            if (updateRefreshToken is null) return new LoginResponse(false, "Refresh Token cannot be generated");

            updateRefreshToken.Token = refreshToken;
            await appDbContext.SaveChangesAsync();
            return new LoginResponse(true, "Token refreshed successfully", jwtToken, refreshToken);




               

        }

        public async Task<List<ManageUsers>> GetUsers()
        {
            var allUsers = await GetApplicationUsers();
            var allUserRoles = await UserRoles();
            var allRoles = await SystemRoles();

            if(allUsers.Count ==0 || allRoles.Count ==0) return null;
            var users = new List<ManageUsers>();
            foreach(var user in allUsers)
            {
                var userRole = allUserRoles.FirstOrDefault(u => u.UserId == user.id);
                var roleName = allRoles.FirstOrDefault(u => u.Id == userRole!.RoleId);
                users.Add(new ManageUsers() { UserId=user.id,Name=user.Fullname,Email=user.Email!,Role = roleName!.Name!});

            }
            return users;
        }

        private async Task<List<SystemRole>> SystemRoles()
        {
            return await appDbContext.SystemRoles.AsNoTracking().ToListAsync();
        }

        private async Task<List<UserRole>> UserRoles()
        {
            return await appDbContext.UserRoles.AsNoTracking().ToListAsync();
        }

        private async Task<List<ApplicationUser>> GetApplicationUsers()
        {
           return await appDbContext.ApplicationUsers.AsNoTracking().ToListAsync();
        }

        public async Task<GeneralResponse> UpdateUser(ManageUsers user)
        {
            var getRole = (await SystemRoles()).FirstOrDefault(r => r.Name!.Equals(user.Role));
            var userRole = await appDbContext.UserRoles.FirstOrDefaultAsync(u => u.UserId == user.UserId);
            userRole!.RoleId = getRole.Id;
            await appDbContext.SaveChangesAsync();
            return new GeneralResponse(true, "Use Roles Updated Successfully");
        }

        public async Task<List<SystemRole>> GetRoles() => await SystemRoles();

        public async Task<GeneralResponse> DeleteUser(int id)
        {
            var user = await appDbContext.ApplicationUsers.FirstOrDefaultAsync(u => u.id == id);
            appDbContext.ApplicationUsers.Remove(user!);
            await appDbContext.SaveChangesAsync();
            return new GeneralResponse(true, "User Deleted SUccessfully");
        }
    }
}
