using Application.Interfaces.IStatus;
using Application.Interfaces.IStatus.IStatusServices;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ProyectoRestaurante.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
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