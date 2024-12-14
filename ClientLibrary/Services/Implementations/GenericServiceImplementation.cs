using BaseLibrary.Responses;
using ClientLibrary.Helpers;
using ClientLibrary.Services.contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace ClientLibrary.Services.Implementations
{
    public class GenericServiceImplementation<T>(GetHttpClient getHttpClient) : IGenericImplementation<T>
    {
        public async Task<GeneralResponse> DeleteById(int id, string baseUrl)
        {
            var httpCLient = getHttpClient.GetPublicHttpClient();
            var response = await httpCLient.DeleteAsync($"{baseUrl}/delete/{id}");
            var result = await response.Content.ReadFromJsonAsync<GeneralResponse>();
            return result;
        }
        //Read all
        public async Task<List<T>> GetAll(string baseUrl)
        {
            var httpCLient = getHttpClient.GetPublicHttpClient();
            var result =  await httpCLient.GetFromJsonAsync<List<T>>($"{baseUrl}/all");
            return result;
        }
        //read single
        public async Task<T> GetById(int id, string baseUrl)
        {
            var httpCLient = getHttpClient.GetPublicHttpClient();
            var result = await httpCLient.GetFromJsonAsync<T>($"{baseUrl}/single/{id}");
            return result;
        }
         //create
        public async Task<GeneralResponse> Insert(T item, string baseUrl)
        {
            var httpCLient = getHttpClient.GetPublicHttpClient();
            var response = await httpCLient.PostAsJsonAsync($"{baseUrl}/add", item);
            var result = await response.Content.ReadFromJsonAsync<GeneralResponse>();
            return result;

        }

        //update model
        public async Task<GeneralResponse> Update(T item, string baseUrl)
        {
            var httpCLient = getHttpClient.GetPublicHttpClient();
            var response = await httpCLient.PutAsJsonAsync($"{baseUrl}/update", item);
            return await response.Content.ReadFromJsonAsync<GeneralResponse>();
        }
    }
}
