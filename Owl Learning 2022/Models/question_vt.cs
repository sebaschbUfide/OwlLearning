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
    
    public partial class question_vt
    {
        public question_vt()
        {
            this.answer_vt = new HashSet<answer_vt>();
        }
    
        public int question_vt_id { get; set; }
        public int question { get; set; }
        public int virtual_test { get; set; }
    
        public virtual ICollection<answer_vt> answer_vt { get; set; }
        public virtual questions questions { get; set; }
        public virtual virtual_tests virtual_tests { get; set; }
    }
}
