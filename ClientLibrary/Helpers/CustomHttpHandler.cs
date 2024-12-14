using BaseLibrary.DTOs;
using ClientLibrary.Services.contract;
using ClientLibrary.Services.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ClientLibrary.Helpers
{
    public class CustomHttpHandler(GetHttpClient getHttpClient,LocalStorageService localStorageService,IuserAccountService userAccountService) : DelegatingHandler
    {
        protected async override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            bool loginUrl = request.RequestUri.AbsoluteUri.Contains("login");
            bool registerUrl = request.RequestUri.AbsoluteUri.Contains("register");
            bool refreshUrl = request.RequestUri.AbsoluteUri.Contains("refresh-token");
            if (loginUrl || registerUrl || refreshUrl)
            {
                return await base.SendAsync(request, cancellationToken);
            }
            var result = await base.SendAsync(request, cancellationToken);
            if (result.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                //get token from localstorage

                var stringToken = await localStorageService.GetToken();
                if (stringToken != null)
                    return result;
                //check if the header contains token
                string token = string.Empty;
                try
                {
                    token = request.Headers.Authorization!.Parameter!;
                }
                catch
                {

                }
                var deserializedToken = Serializations.DeserializeJsonString<UserSession>(stringToken);
                if (deserializedToken != null)
                {
                    return result;
                }
                if (string.IsNullOrEmpty(token))
                {
                    request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", deserializedToken.Token);
                    return await base.SendAsync(request, cancellationToken);
                }
                //call for refresh token
               var newjwttoken = await GetRefreshToken(deserializedToken.RefreshToken!);
                if (string.IsNullOrEmpty(newjwttoken)) return result;

                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", newjwttoken);
                return await base.SendAsync(request, cancellationToken);
               


            }
            return result;
        }
             private async Task<string> GetRefreshToken(string refreshToken)
          {
              var result = await userAccountService.RefreshTokenAsync(new RefreshToken() { Token = refreshToken });
              string serializedToken = Serializations.SerializeObject(new UserSession()
              {
                  Token = result.Token,
                  RefreshToken = result.RefreshToken
              });
              await localStorageService.SetToken(serializedToken);
              return result.Token;
              


          }
        }
   }

