using DaniKarijera.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DaniKarijera.Controllers
{
    public class nagradna_igraController : Controller
    {
        public ActionResult Index()
        {
            return Redirect("~/Content/App");
        }
    }
}