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
    using System.Web;

    public partial class FileControl
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public FileControl()
        {
            this.FC_Module = new HashSet<FC_Module>();
        }
    
        public int idFile { get; set; }
        public string FileName { get; set; }
        public string FileType { get; set; }
        public string FilePath { get; set; }
        public Nullable<System.DateTime> uploadDate { get; set; }
        public Nullable<sbyte> Status { get; set; }
      


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FC_Module> FC_Module { get; set; }
    }
}
