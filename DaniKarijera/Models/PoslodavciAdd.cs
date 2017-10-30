using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DaniKarijera.Models
{
    public class PoslodavciAdd
    {
        public int id_poslodavca { get; set; }
        public string Ime { get; set; }
        public string Prezime { get; set; }
        public string Email { get; set; }
    }
}