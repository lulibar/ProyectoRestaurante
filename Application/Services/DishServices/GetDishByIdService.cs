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
    public class GetDishByIdService : IGetDishByIdService
    {
        private readonly IDishCommand _command;
        private readonly IDishQuery _query;
        private readonly ICategoryQuery _categoryQuery;

        public GetDishByIdService(IDishCommand command, IDishQuery query, ICategoryQuery categoryQuery)
        {
            _command = command;
            _query = query;
            _categoryQuery = categoryQuery;
        }

        public async Task<DishResponse?> GetDishById(Guid id)
        {
            var dish = await _query.GetDishById(id);
			if (dish == null)
			{
				throw new Exception("Dish not found"); 
			}
			return new DishResponse
			{
				id = dish.DishId,
				name = dish.Name,
				description = dish.Description,
				price = dish.Price,
				category = new GenericResponse { Id = dish.CategoryId, Name = dish.Category.Name },
				isActive = dish.Available,
				image = dish.ImageUrl,  
				createdAt = dish.CreateDate,
				updatedAt = dish.UpdateDate
			};
        }
    }
}
