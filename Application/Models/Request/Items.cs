﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models.Request
{
    public class Items
    {
        
        public Guid id { get; set; } //id del dish
        public int quantity { get; set; }
        public string? notes { get; set; }
    }
}
