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
    
    public partial class roles
    {
        public roles()
        {
            this.role_user = new HashSet<role_user>();
        }
    
        public int role_id { get; set; }
        public string name { get; set; }
    
        public virtual ICollection<role_user> role_user { get; set; }
    }
}