﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modelos.Tuneflow.Models
{
    public class Genre
    {
        [Key] public int Id { get; set; }
        public string Name { get; set; }
        public string FilePath { get; set; } 
    }
}
