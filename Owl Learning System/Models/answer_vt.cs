//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace mysqltest.Models
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
