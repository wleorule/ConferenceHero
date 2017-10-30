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
    public class DizajnController : Controller
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
        public ActionResult Dodaj(dizajn dizajn)
        {
            if(dizajn != null)
            { 
                using(DKEntities db = new DKEntities())
                {  
                    db.dizajn.Add(dizajn);
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