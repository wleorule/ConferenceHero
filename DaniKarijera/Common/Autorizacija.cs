using DaniKarijera.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace DaniKarijera.Common
{
    public class Autorizacija : AuthorizeAttribute
    {
        private int privilegija { get; set; }

        public Autorizacija(int minPrivilegija)
        {
            privilegija = minPrivilegija;
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            HttpCookie cookie = httpContext.Request.Cookies["Sessionid"];

            if (cookie != null) {

                string token = cookie.Value.ToString();

                if (string.IsNullOrEmpty(token))
                {
                    return false;
                }

                using (DKEntities db = new DKEntities())
                {
                    korisnici korisnik = db.korisnici.Where(k => k.token == token).FirstOrDefault();

                    if (korisnik != null)
                    {
                        if (korisnik.privilegija >= privilegija)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            else
            {
                return false;
            }
        }
    }
}