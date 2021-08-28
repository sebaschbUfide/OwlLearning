using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace mysqltest.Models
{
    public class cursos
    {
        [Required(ErrorMessage = "Ingrese un nombre")]
  
        public String name { get; set; }
        [Required(ErrorMessage = "Ingrese una descripcion")]
        [MinLength(10, ErrorMessage = "Requiere un minimo de  10 caracteres")]
        public String description { get; set; }
        public int duracion { get; set; }
        public decimal cost { get; set; }
        public int type { get; set; }

    }
}