﻿using BaseLibrary.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using serverLibrary.Respositories.contract;

namespace server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountryController(IGenericRepositoryInterface<Country> genericRepositoryInterface) : GenericController<Country>(genericRepositoryInterface)
    {
    }
}
