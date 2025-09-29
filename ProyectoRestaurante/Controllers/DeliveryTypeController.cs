using Application.Interfaces.IDeliveryType.IDeliveryTypeServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ProyectoRestaurante.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeliveryTypeController : ControllerBase
    {
        private readonly IGetAllDeliveryTypesService _getAllDeliveryTypesService;

        public DeliveryTypeController(IGetAllDeliveryTypesService getAllDeliveryTypesService)
        {
            _getAllDeliveryTypesService = getAllDeliveryTypesService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _getAllDeliveryTypesService.GetAllDeliveryTypes();
            return Ok(result);
        }
    }
}
