using DaniKarijera.Common;
using DaniKarijera.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DaniKarijera.Controllers
{
    public class GlasanjeController : Controller
    {
        [Autorizacija(minPrivilegija: 1)]
        public ActionResult Index()
        {
            DKEntities db = new DKEntities();
            List<poslodavci> poslodavci = db.poslodavci.Where(l => l.glasanje1 == true).OrderBy(l => l.naziv).ToList();
            ViewData["raspored"] = poslodavci;

            korisnici k = Korisnici.DohvatiTrenutnogKorisnika();

            if (k != null) {
                
                List<glasanje> glasanje = db.glasanje.Where(l => l.id_korisnika == k.id_korisnika).ToList();
                ViewData["glasanje"] = glasanje;
                }

            List<poslodavci> pl = db.poslodavci.Where(l => l.glasanje1 == true).ToList();

            poslodavci naj = null;
            int max = -999;

            foreach (poslodavci p in pl)
            {
                int temp = db.glasanje.Where(l => l.id_poslodavca == p.id_poslodavca).Sum(l => l.ocjena) ?? 0;

                if (temp > max)
                {
                    max = temp;
                    naj = p;
                }
            }
            ViewBag.Vodeci = naj.naziv;

            return View("Index");
        }

        
        public int Ocijeni(int id, int ocjena)
        {
            DKEntities db = new DKEntities();

            poslodavci p = db.poslodavci.Where(l => l.id_poslodavca == id).FirstOrDefault();
            korisnici k = Korisnici.DohvatiTrenutnogKorisnika();

            if (p != null && k != null)
            {
                if(ocjena < 1 || ocjena > 5)
                {
                    ocjena = 1;
                }
                
                glasanje g = db.glasanje.Where(l => l.id_korisnika == k.id_korisnika && l.id_poslodavca == p.id_poslodavca).FirstOrDefault();

                if(g != null)
                {
                    g.ocjena = ocjena;
                    db.Entry(g).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    return ocjena;
                }
                else
                {
                    g = new glasanje();

                    g.id_poslodavca = p.id_poslodavca;
                    g.id_korisnika = k.id_korisnika;
                    g.ocjena = ocjena;

                    db.glasanje.Add(g);
                    db.SaveChanges();
                    return ocjena;
                }
            }

            return 0;
        }

      
    }
}