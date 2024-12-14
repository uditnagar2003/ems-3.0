using client;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Blazored.LocalStorage;
using ClientLibrary.Helpers;
using Microsoft.AspNetCore.Components.Authorization;
using ClientLibrary.Services.contract;
using ClientLibrary.Services.Implementations;
//using Syncfusion.Blazor.Popups;
using Syncfusion.Blazor;
using client.ApplicationStates;
using BaseLibrary.Entities;
using Syncfusion.Blazor.Popups;
//using Syncfusion.Blazor.Popups;
//Register Syncfusion license


var builder = WebAssemblyHostBuilder.CreateDefault(args);
Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MzYyNTA1OUAzMjM3MmUzMDJlMzBPanYrSUhHUlNwaUN3T2YzdEloZjZRNkFib3hxOHRDVDdpYmhGdTBEM3hZPQ==");

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
builder.Services.AddTransient<CustomHttpHandler>();
builder.Services.AddHttpClient("SystemApiClient", client =>
{
    client.BaseAddress = new Uri("https://localhost:7089/");
}).AddHttpMessageHandler<CustomHttpHandler>(); ;
//builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:7089/") });
builder.Services.AddAuthorizationCore();
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddScoped<GetHttpClient>();
builder.Services.AddScoped<LocalStorageService>();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();
builder.Services.AddScoped<IuserAccountService, UserAccountService>();
//

//general deparmtnet / department / branch
builder.Services.AddScoped<IGenericImplementation<GeneralDepartment>, GenericServiceImplementation<GeneralDepartment>>();
builder.Services.AddScoped<IGenericImplementation<Department>, GenericServiceImplementation<Department>>();
builder.Services.AddScoped<IGenericImplementation<Branch>, GenericServiceImplementation<Branch>>();

//Country / town /city
builder.Services.AddScoped<IGenericImplementation<Country>, GenericServiceImplementation<Country>>();
builder.Services.AddScoped<IGenericImplementation<City>, GenericServiceImplementation<City>>();
builder.Services.AddScoped<IGenericImplementation<Town>, GenericServiceImplementation<Town>>();

//Employee
builder.Services.AddScoped<IGenericImplementation<Employee>, GenericServiceImplementation<Employee>>();


builder.Services.AddScoped<AllState>();

builder.Services.AddSyncfusionBlazor();
builder.Services.AddScoped<SfDialogService>();
 
//builder.Services.AddScoped<SfDialogService>();
//builder.Services.AddScoped<Serializations>();
await builder.Build().RunAsync();
