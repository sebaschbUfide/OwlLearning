using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;


namespace mysqltest.Models
{
    public class Registro
    {
     
    [Required(ErrorMessage ="Ingrese su Cédula")]
    [Display(Name ="Cedúla")]
    [Key]
    public string dni { get; set; }
    
    public string nombre { get; set; }    

    public string apellido { get; set; }

    public string telefono { get; set;}

    [Required(ErrorMessage = "Ingrese su Correo")]
    [Display(Name = "Correo")]
    [DataType(DataType.EmailAddress)]
    public string email { get; set; }

    [Required(ErrorMessage = "Ingrese su Clave")]
    [Display(Name = "Clave")]
    [DataType(DataType.Password)]
        [MinLength(6)]
    public string clave { get; set; }
    
    
    }
}