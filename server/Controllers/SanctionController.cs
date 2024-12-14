using BaseLibrary.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using serverLibrary.Respositories.contract;

namespace server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SanctionController(IGenericRepositoryInterface<Sanction> genericRepositoryInterface) : GenericController<Sanction>(genericRepositoryInterface)
    {
    }
}
