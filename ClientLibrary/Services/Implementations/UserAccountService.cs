using BaseLibrary.DTOs;
using BaseLibrary.Entities;
using BaseLibrary.Responses;
using ClientLibrary.Helpers;
using ClientLibrary.Services.contract;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace ClientLibrary.Services.Implementations
{
    public class UserAccountService(GetHttpClient getHttpClient) : IuserAccountService
    {
        public const string AuthUrl = "api/Authentication";

        public async Task<GeneralResponse> CreateAsync(Register user)
        {
           var httpCLient  = getHttpClient.GetPublicHttpClient();
            var result = await httpCLient.PostAsJsonAsync($"{AuthUrl}/register", user);
            if (!result.IsSuccessStatusCode) return new GeneralResponse(false, "An Error Occured");

            return await result.Content.ReadFromJsonAsync<GeneralResponse>();
        }

        
        public async Task<LoginResponse> RefreshTokenAsync(RefreshToken token)
        {
            //weather implemented
            var httpCLient = getHttpClient.GetPublicHttpClient();
            var result = await httpCLient.PostAsJsonAsync($"{AuthUrl}/register", token);
            if (!result.IsSuccessStatusCode) return new LoginResponse(false, "An Error Occured");

            return await result.Content.ReadFromJsonAsync<LoginResponse>();
        }

        public async Task<LoginResponse> SignInAsync(Login user)
        {
            var httpCLient = getHttpClient.GetPublicHttpClient();
            var result = await httpCLient.PostAsJsonAsync($"{AuthUrl}/Login",user);
            if (!result.IsSuccessStatusCode) return new LoginResponse(false, "An Error Occured");

            return await result.Content.ReadFromJsonAsync<LoginResponse>()!;
        }

        //user roles
        public async Task<GeneralResponse> DeleteUser(int id)
        {
            var httpCLient = await getHttpClient.GetPrivateHttpClient();
            var result = await httpCLient.DeleteAsync($"{AuthUrl}/delete-user/{id}");
            if (!result.IsSuccessStatusCode)
                return new GeneralResponse(false, "Error Occured");
            return await result.Content.ReadFromJsonAsync<GeneralResponse>();

        }

        public async Task<List<SystemRole>> GetRoles()
        {
            var httpCLient = await getHttpClient.GetPrivateHttpClient();
            var result = await httpCLient.GetFromJsonAsync<List<SystemRole>>($"{AuthUrl}/roles");
            return result!;

        }

        public async Task<List<ManageUsers>> GetUsers()
        {
            var httpClient = await getHttpClient.GetPrivateHttpClient();
            var result = await httpClient.GetFromJsonAsync<List<ManageUsers>>($"{AuthUrl}/users");
            return result!;
        }

        public async Task<GeneralResponse> UpdateUser(ManageUsers user)
        {
            var httpCLient = getHttpClient.GetPublicHttpClient();
            var result = await httpCLient.PutAsJsonAsync($"{AuthUrl}/update-user", user);
            if (!result.IsSuccessStatusCode)
                return new GeneralResponse(false, "Error Occured");
            return await result.Content.ReadFromJsonAsync<GeneralResponse>();


        }


    }
}
