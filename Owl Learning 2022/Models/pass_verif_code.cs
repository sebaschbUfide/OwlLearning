//------------------------------------------------------------------------------
// <auto-generated>
//    Este código se generó a partir de una plantilla.
//
//    Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//    Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Owl_Learning_2022.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class pass_verif_code
    {
        public int pass_verif_code1 { get; set; }
        public int verification_code { get; set; }
        public int user_id { get; set; }
    
        public virtual users users { get; set; }
    }
}
