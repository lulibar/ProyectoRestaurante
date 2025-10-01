using Application.Interfaces.IStatus;
using Application.Interfaces.IStatus.IStatusServices;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ProyectoRestaurante.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class StatusController : ControllerBase
    {
        private readonly IGetAllStatusesService _getAllStatusService;

        public StatusController(IGetAllStatusesService getAllStatusService)
        {
            _getAllStatusService = getAllStatusService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _getAllStatusService.GetAllStatus();
            return Ok(result);
        }
    }
}