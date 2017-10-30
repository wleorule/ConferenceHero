using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DaniKarijera.Models
{
    public class Korisnik
    {
        public int id_korisnika { get; set; }
        public string ime { get; set; }
        public string prezime { get; set; }
        public HttpPostedFileBase slika { get; set; }
        public string smjer { get; set; }
        public string godina { get; set; }
        public string interesi { get; set; }
        public string broj_telefona { get; set; }
    }
}