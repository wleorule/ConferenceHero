using DaniKarijera.Common;
using DaniKarijera.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DaniKarijera.Controllers
{
    [Autorizacija(minPrivilegija:5)]
    public class ProstorijeController : Controller
    {
        // GET: Poslodavci
        public ActionResult Index()
        {
            return View();
        }

        #region Dodaj
        // GET: Poslodavci
        public ActionResult Dodaj()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Dodaj(prostorije prostorija)
        {
            if (prostorija != null)
            {
                prostorije newPro = new prostorije()
                {
                    naziv = prostorija.naziv,
                    lokacija = prostorija.lokacija,
                    boja = prostorija.boja
                };
               

                using(DKEntities db = new DKEntities())
                {  
                    db.prostorije.Add(newPro);
                    db.SaveChanges();
                }
                    
                return View();
            }
            else
            {
                return View();
            }
        }
        #endregion
    }
}