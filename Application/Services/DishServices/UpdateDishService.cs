using Application.Interfaces.ICategory;
using Application.Interfaces.IDish;
using Application.Interfaces.IDish.IDishServices;
using Application.Models.Request;
using Application.Models.Response;
using Application.Models.Response.DishResponse;
using Domain.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static System.Net.Mime.MediaTypeNames;

namespace Application.Services.DishServices
{
    public class UpdateDishService : IUpdateDishService
    {
        private readonly IDishCommand _command;
        private readonly IDishQuery _query;
        private readonly ICategoryQuery _categoryQuery;

        public UpdateDishService(IDishCommand command, IDishQuery query, ICategoryQuery categoryQuery)
        {
            _command = command;
            _query = query;
            _categoryQuery = categoryQuery;
        }

        public async Task<UpdateDishResult> UpdateDish(Guid id, DishUpdateRequest DishUpdateRequest)
        {
            var existingDish = await _query.GetDishById(id);

			if (existingDish == null)
			{
				return new UpdateDishResult { NotFound = true };
			}
			var alreadyExist = await _query.DishExists(DishUpdateRequest.Name,id);
			
            if (alreadyExist)
			{
				return new UpdateDishResult { NameConflict = true };
			}

            var category = await _categoryQuery.GetCategoryById(DishUpdateRequest.Category);

            existingDish.Name = DishUpdateRequest.Name;
			existingDish.Description = DishUpdateRequest.Description;
			existingDish.Price = DishUpdateRequest.Price;
			existingDish.Available = DishUpdateRequest.IsActive;
			existingDish.ImageUrl = DishUpdateRequest.Image;
            existingDish.CategoryId = DishUpdateRequest.Category;
            existingDish.UpdateDate = DateTime.Now;

			await _command.UpdateDish(existingDish);

			return new UpdateDishResult
			{
				Success = true,
                UpdatedDish = new DishResponse
                {
                    id = existingDish.DishId,               
                    name = existingDish.Name,              
                    description = existingDish.Description,    
                    price = existingDish.Price,             
                    isActive = existingDish.Available,
                    image = existingDish.ImageUrl,           
                    category = new GenericResponse { Id = existingDish.CategoryId, Name = category.Name }, 
                    createdAt = existingDish.CreateDate,
                    updatedAt = existingDish.UpdateDate
                } // tiene que retornar un dishresponse con las variables del exitingdish
            };
        }
    }
}
