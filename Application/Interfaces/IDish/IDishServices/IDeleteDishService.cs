﻿using Application.Models.Response.DishResponse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.IDish.IDishServices
{
    public interface IDeleteDishService
    {
        Task<DishResponse> DeleteDish(Guid id);
    }
}
