using Application.Interfaces.ICategory;
using Application.Interfaces.IDish;
using Application.Interfaces.IDish.IDishServices;
using Application.Models.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.DishServices
{
    public class GetAllDishesAsyncService : IGetAllDishesAsyncService
    {

        private readonly IDishCommand _command;
        private readonly IDishQuery _query;
        private readonly ICategoryQuery _categoryQuery;

        public GetAllDishesAsyncService(IDishCommand command, IDishQuery query, ICategoryQuery categoryQuery)
        {
            _command = command;
            _query = query;
            _categoryQuery = categoryQuery;
        }

        public async Task<List<DishResponse>> GetAllDishesAsync()
        {
            var dishes = await _query.GetAllDishes();

			return dishes.Select(dishes => new DishResponse
			{
				id = dishes.DishId,
				name = dishes.Name,
				description = dishes.Description,
				price = dishes.Price,
				category = new GenericResponse { Id = dishes.CategoryId, Name = dishes.Category.Name },
			    isActive = dishes.Available,
				image = dishes.ImageUrl,
				createdAt = dishes.CreateDate,
				updatedAt = dishes.UpdateDate
			}).ToList();
        }
    }
}
