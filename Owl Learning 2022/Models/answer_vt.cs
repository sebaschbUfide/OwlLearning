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
    
    public partial class answer_vt
    {
        public int answer_vt_id { get; set; }
        public int user_id { get; set; }
        public int question_vt_id { get; set; }
        public int question_id { get; set; }
        public int got_answer { get; set; }
    
        public virtual users users { get; set; }
        public virtual question_vt question_vt { get; set; }
        public virtual questions questions { get; set; }
    }
}