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
    
    public partial class role_user
    {
        public int role_user_id { get; set; }
        public int user_id { get; set; }
        public int role_id { get; set; }
    
        public virtual roles roles { get; set; }
        public virtual users users { get; set; }
    }
}
