using Application.Interfaces.ICategory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.CategoryServices
{
    public class CategoryExistUseCase : ICategoryExists
    {
        private readonly ICategoryQuery _categoryQuery;
        public CategoryExistUseCase(ICategoryQuery categoryQuery)
        {
            _categoryQuery = categoryQuery;
        }
        public async Task<bool> CategoryExists(int id)
        {
            var category = await _categoryQuery.CategoryExistAsync(id);
            return category;
        }

       
    }
}
