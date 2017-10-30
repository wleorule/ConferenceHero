using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DaniKarijera.Models;

namespace DaniKarijera.Common
{
    public static class Korisnici
    {
        public static korisnici DohvatiTrenutnogKorisnika()
        {
            HttpContext httpCtx = System.Web.HttpContext.Current;

            using (DKEntities db = new DKEntities())
            {
                HttpCookie cookie = httpCtx.Request.Cookies["Sessionid"];

                if (cookie != null)
                {
                    korisnici korisnik = db.korisnici.Where(l => l.token == cookie.Value).FirstOrDefault();
                    return korisnik;
                }
                else
                {
                    return null;
                }
            }
        }

        public static radnici DohvatiRadnik(int id_korisnika)
        {
            using (DKEntities db = new DKEntities())
            {
                radnici radnik = db.radnici.Where(l => l.id_korisnika == id_korisnika).FirstOrDefault();
                return radnik;
            }
        }

        public static radnici DohvatiRadnik()
        {            
            using (DKEntities db = new DKEntities())
            {
                korisnici korisnik = DohvatiTrenutnogKorisnika();
                if (korisnik != null)
                {
                    radnici radnik = db.radnici.Where(l => l.id_korisnika == korisnik.id_korisnika).FirstOrDefault();
                    return radnik;
                }
                else
                {
                    return null;
                }
            }
        }
    }
}