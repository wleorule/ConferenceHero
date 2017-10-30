using DaniKarijera.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DaniKarijera.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            DKEntities db = new DKEntities();
            List<raspored> raspored = db.raspored.ToList();
            ViewData["raspored"] = raspored;
            return View("Index");
        }


    }
}