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
    public class RasporedController : Controller
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
            using (DKEntities db = new DKEntities())
            {
                ViewData["poslodavci"] = db.poslodavci.ToList();
                ViewData["prostorije"] = db.prostorije.ToList();
            }

            return View();
        }
        [HttpPost]
        public ActionResult Dodaj(raspored raspored)
        {
            if(raspored != null)
            { 
                using(DKEntities db = new DKEntities())
                {  
                    db.raspored.Add(raspored);
                    db.SaveChanges();
                }
                    
                return View("Index");
            }
            else
            {
                return View();
            }
        }
        #endregion
    }
}