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
    
    public partial class course_assignment
    {
        public int course_assign_id { get; set; }
        public int user_id { get; set; }
        public int course_id { get; set; }
        public Nullable<int> schedule_id { get; set; }
        public string status { get; set; }
        public Nullable<System.DateTime> enrollTime { get; set; }
    
        public virtual users users { get; set; }
        public virtual courses courses { get; set; }
        public virtual schedules schedules { get; set; }
    }
}
