//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DaniKarijera.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class dizajn
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public dizajn()
        {
            this.virtualne_vizitke = new HashSet<virtualne_vizitke>();
        }
    
        public int id_dizajna { get; set; }
        public string naziv { get; set; }
        public string slika { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<virtualne_vizitke> virtualne_vizitke { get; set; }
    }
}
