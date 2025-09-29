using Application.Interfaces.ICategory.ICategoryServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ProyectoRestaurante.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly IGetAllCategoriesService _getAllCategoriesService;

        public CategoryController (IGetAllCategoriesService getAllCategoriesService)
        {
            _getAllCategoriesService = getAllCategoriesService; 
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var categories = await _getAllCategoriesService.GetAllCategories();
            return Ok(categories);
        }

    }
}
