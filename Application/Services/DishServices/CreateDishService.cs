using Application.Interfaces.ICategory;
using Application.Interfaces.IDish;
using Application.Interfaces.IDish.IDishServices;
using Application.Models.Request;
using Application.Models.Response;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.DishServices
{
    public class CreateDishService : ICreateDishService
    {
        private readonly IDishCommand _command;
        private readonly IDishQuery _query;
        private readonly ICategoryQuery _categoryQuery;

        public CreateDishService (IDishCommand command, IDishQuery query, ICategoryQuery categoryQuery)
        {
            _command = command;
            _query = query;
            _categoryQuery = categoryQuery;
        }

        public async Task<DishResponse?> CreateDish(DishRequest dishRequest)
        {
			var existingDish = await _query.DishExists(dishRequest.Name);

			if (existingDish)
			{
				return null;
			}
			var category = await _categoryQuery.GetCategoryById(dishRequest.Category);

			var dish = new Dish
			{
				Name = dishRequest.Name,
				Description = dishRequest.Description,
				Price = dishRequest.Price,
                CategoryId = dishRequest.Category,
                Available = true,
				ImageUrl = dishRequest.Image,
				CreateDate = DateTime.Now,
				UpdateDate = DateTime.Now,
			};
			var creadorDish = await _command.InsertDish(dish);
			return new DishResponse
			{
				id = creadorDish.DishId,
				name = creadorDish.Name,
				description = creadorDish.Description,
				price = creadorDish.Price,
				isActive = creadorDish.Available,
				image = creadorDish.ImageUrl,
				category = new GenericResponse { Id = creadorDish.CategoryId, Name = category.Name },
				createdAt = creadorDish.CreateDate,
				updatedAt = creadorDish.UpdateDate
			};
        }
    }
}
