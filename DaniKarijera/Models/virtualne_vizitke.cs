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
    
    public partial class virtualne_vizitke
    {
        public int id_vizitke { get; set; }
        public Nullable<int> id_korisnika { get; set; }
        public Nullable<int> id_poslodavnca { get; set; }
        public Nullable<int> id_dizajna { get; set; }
    
        public virtual dizajn dizajn { get; set; }
        public virtual korisnici korisnici { get; set; }
        public virtual poslodavci poslodavci { get; set; }
    }
}
