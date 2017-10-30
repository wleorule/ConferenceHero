using DaniKarijera.Common;
using DaniKarijera.Models;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DaniKarijera.Controllers
{

    public class PoslodavciController : Controller
    {
        [Autorizacija(minPrivilegija: 1)]
        // GET: Poslodavci
        public ActionResult Index(string id = "")
        {
            using (DKEntities db = new DKEntities())
            {
                ViewData["poslodavci"] = db.poslodavci.Where(l => l.popis == true).OrderBy(l => l.naziv).ToList();
            }

            if (!string.IsNullOrEmpty(id))
            {
                ViewBag.Poslano = id;
            }
            return View();
        }

        [Autorizacija(minPrivilegija: 1)]
        public ActionResult Vizitka(int id)
        {
            using (DKEntities db = new DKEntities())
            {
                poslodavci poslodavac = db.poslodavci.AsNoTracking().Where(l => l.id_poslodavca == id).FirstOrDefault();

                if (poslodavac == null)
                {
                    return HttpNotFound();
                }

                korisnici korisnik = Korisnici.DohvatiTrenutnogKorisnika();

                if (korisnik == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                dizajn diz = db.dizajn.FirstOrDefault();

                if (diz != null)
                {
                    virtualne_vizitke v = db.virtualne_vizitke.Where(l => l.id_korisnika == korisnik.id_korisnika & l.id_poslodavnca == poslodavac.id_poslodavca).FirstOrDefault();

                    if (v == null)
                    {
                        virtualne_vizitke vv = new virtualne_vizitke()
                        {
                            id_korisnika = korisnik.id_korisnika,
                            id_poslodavnca = poslodavac.id_poslodavca,
                            id_dizajna = diz.id_dizajna
                        };

                        db.virtualne_vizitke.Add(vv);
                        db.SaveChanges();
                    }
                }
            }
            ViewBag.Poslano = "";
            return RedirectToAction("Index/Vizitka je poslana poslodavcu", "Poslodavci");
        }

        [Autorizacija(minPrivilegija:1)]
        public ActionResult Prvi()
        {
            using (DKEntities db = new DKEntities())
            {
                List<poslodavci> pl = db.poslodavci.Where(l => l.glasanje1 == true).ToList();

                poslodavci naj = null;
                int max = -999;

                foreach(poslodavci p in pl)
                {
                    int temp = db.glasanje.Where(l => l.id_poslodavca == p.id_poslodavca).Sum(l => l.ocjena) ?? 0;

                    if(temp > max)
                    {
                        max = temp;
                        naj = p;
                    }
                }
                
                return PartialView(naj);
            }
        }

        [Autorizacija(minPrivilegija: 3)]
        // GET: Poslodavci
        public ActionResult MojeVizitke()
        {


            radnici radnik = Korisnici.DohvatiRadnik();

            if (radnik == null) // ako uopce je radnik
            {
                return HttpNotFound();
            }
            List<korisnici> korisnici = new List<Models.korisnici>();
            using (DKEntities db = new DKEntities())
            {
                List<virtualne_vizitke> vizitke = db.virtualne_vizitke.AsNoTracking().Where(l => l.id_poslodavnca == radnik.id_poslodavca).ToList();

                foreach(virtualne_vizitke vv in vizitke)
                {
                    korisnici.Add(vv.korisnici);
                }
            }

            ViewData["korisnici"] = korisnici;

            return View();
        }


        public ActionResult Prijava(string id)
        {
            if(id != null)
            {
                using (DKEntities db = new DKEntities())
                {
                    korisnici k = db.korisnici.Where(l => l.token == id).FirstOrDefault();

                    if (k != null)
                    {
                        HttpCookie cookie = new HttpCookie("Sessionid");
                        cookie.HttpOnly = true;
                        cookie.Value = id;
                        cookie.Expires = DateTime.Now.AddDays(1);

                        HttpContext ctx = System.Web.HttpContext.Current;
                        ctx.Response.SetCookie(cookie);

                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        return HttpNotFound();
                    }
                }
                
            }

            return HttpNotFound();
        }

        #region Dodaj
        // GET: Poslodavci
        [Autorizacija(minPrivilegija: 5)]
        public ActionResult Dodaj()
        {
            using (DKEntities db = new DKEntities())
            {
                ViewData["poslodavci"] = db.poslodavci.Where(l => l.glasanje1 == true).ToList();
            return View();
            }
        }
        [HttpPost]
        [Autorizacija(minPrivilegija: 5)]
        public async System.Threading.Tasks.Task<ActionResult> Dodaj(PoslodavciAdd poslodavac)
        {
            if (poslodavac != null)
            {
               
                //TODO: popuniti link

                korisnici korisnik = new korisnici()
                {
                    ime = poslodavac.Ime,
                    prezime = poslodavac.Prezime,
                    email = poslodavac.Email,
                    privilegija = 3,
                    zaporka = CalculateHash.Hash(poslodavac.Email+"sa6d54sad8as97dsa7d9as87d9asd78as98d8a7sd87as9d7"),
                    token = CalculateHash.Hash(poslodavac.Email + (DateTime.Now.Day * 45).ToString() + "dasdalčkč")
            };

                radnici odgovrnaOsoba = new radnici()
                {
                    link = "",
                    pozicija = ""
                };

                using (DKEntities db = new DKEntities())
                {
                    poslodavci p = db.poslodavci.Where(l => l.id_poslodavca == poslodavac.id_poslodavca).FirstOrDefault();

                    if (p != null)
                    {
                        odgovrnaOsoba.id_poslodavca = poslodavac.id_poslodavca;
                        db.korisnici.Add(korisnik);
                        odgovrnaOsoba.id_korisnika = korisnik.id_korisnika;
                        db.radnici.Add(odgovrnaOsoba);
                        db.SaveChanges();

                        var apiKey = "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";
                        var client = new SendGridClient(apiKey);
                        var from = new EmailAddress("noreply@tdk.cpsrk.foi.hr", "Tjedan karijera CPSRK");
                        var subject = "Tjedna karijera korisnički račun";
                        var to = new EmailAddress(korisnik.email, korisnik.ime + " " + korisnik.prezime);
                        var plainTextContent = "";
                        string link = "http://tdk.cpsrk.foi.hr/Poslodavci/Prijava/" + korisnik.token;
                        var htmlContent = @"Poštovani,<p>
                                        Kako bi Vaše sudjelovanje na Karijernom španciru na ovogodišnjem Tjednu karijera bilo što produktivnije,<br>
                                        omogućili smo Vam pristup sustavu koji Vas spaja sa studentima koji su iskazali poseban interes za baš Vašu tvrtku. <p>

                                        Naime, studenti preko službene web aplikacije Tjedna karijera mogu vidjeti popis svih poslodavaca (uključujući i Vas!)<br>
                                        koji sudjeluju na ovogodišnjem Karijernom španciru. Ukoliko im se neka od navedenih tvrtki posebno svidi,<br>
                                        klikom na gumb joj mogu poslati svoju personaliziranu vizitku na kojoj se nalaze njihovi osobni podaci, smjer i godina studija<br>
                                        te njihovi interesi u struci. <p>

                                        Pomoću poveznice navedene ispod, možete se prijaviti u ranije spomenuti sustav, te popuniti sve podatke o svojoj tvrtki<br>
                                        kako bi privukli što više zainteresiranih studenata. Također možete pregledati vizitke svih studenata koji su iskazali poseban<br>
                                        interes za baš Vašu tvrtku.<p>

                                        <a href=""" + link + @""">- Prijava na sustav -</a><p>

                                        Nakon prijave pomoću gore navedene poveznice, pomoću izbornika posjetite stranicu „Moj poslodavac“,<br>
                                        gdje možete započeti sa uređivanjem opisa Vaše tvrtke i pregledom dobivenih vizitki.<p>

                                        Hvala Vam na suradnji, i nadamo se da ćete susresti puno zainteresiranih studenata u petak!";
                        var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
                        var response = await client.SendEmailAsync(msg);

                        ViewBag.Success = "Uspješno poslan login";
                    }
                    else
                    {
                        ViewBag.Error = "Došlo je do pogreške!";
                    }

                    ViewData["poslodavci"] = db.poslodavci.Where(l => l.glasanje1 == true).ToList();
                    return View();
                }
            }
            else
            {
                return View();
            }
        }
        #endregion

        #region Uredi
        [Autorizacija(minPrivilegija: 3)]
        // GET: Poslodavci
        public ActionResult Uredi(int id)
        {
            if (id == 0)
            {
                return HttpNotFound();
            }

            poslodavci poslodavac = null;
            using (DKEntities db = new DKEntities())
            {
                poslodavac = db.poslodavci.AsNoTracking().Where(l => l.id_poslodavca == id).FirstOrDefault();
                if (poslodavac == null)
                {
                    return HttpNotFound();
                }
            }

            radnici radnik = Korisnici.DohvatiRadnik();

            if (radnik == null) // ako uopce je radnik
            {
                return HttpNotFound();
            }

            if (radnik.id_poslodavca != id) // ako je radnik kod tog poslodavca
            {
                return HttpNotFound();
            }

            return View(poslodavac);
        }

        [HttpPost]
        [Autorizacija(minPrivilegija: 3)]
        // GET: Poslodavci
        public ActionResult Uredi(poslodavci poslodavac)
        {
            if (poslodavac == null)
            {
                return HttpNotFound();
            }
            poslodavac.link = "";
            using (DKEntities db = new DKEntities())
            {
                db.Entry(poslodavac).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            ViewBag.Uspjesno = "Uspješno spremljeni podaci.";
            return View(poslodavac);
        }
        #endregion
    }
}