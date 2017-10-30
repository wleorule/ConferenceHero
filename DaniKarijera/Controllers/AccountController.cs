using DaniKarijera.Common;
using DaniKarijera.Models;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DaniKarijera.Controllers
{
    public class AccountController : Controller
    {

        #region Login
        // GET: Login
        public ActionResult Login()
        {
            if (TempData["Registracija"] != null)
            {
                ViewBag.Registracija = 1;
            }

            if (TempData["Aktivacija"] != null)
            {
                ViewBag.UspjesnoAktiviran = 1;
            }
            ViewBag.Navigacija = 1;
            return View();
        }

        //POST: Login
        [HttpPost]
        public ActionResult Login(Login login)
        {
            return Prijava(login);            
        }

        private ActionResult Prijava(Login login)
        {
            using (DKEntities db = new DKEntities())
            {
                string hashZaporka = CalculateHash.Hash(login.Password);
                korisnici korisnik = db.korisnici.Where(k => k.email == login.Email & k.zaporka == hashZaporka).FirstOrDefault();
                
                if (korisnik != null)
                {
                    if (korisnik.privilegija != 0)
                    {
                        // Dodjeljivanje sessije
                        HttpContext ctx = System.Web.HttpContext.Current;

                        HttpCookie cookie = new HttpCookie("Sessionid");
                        cookie.HttpOnly = true;
                        string token = CalculateHash.Hash(login.Email + (DateTime.Now.Day * 45).ToString() + "dasdalčkč");
                        cookie.Value = token;
                        cookie.Expires = DateTime.Now.AddDays(1);

                        ctx.Response.SetCookie(cookie);

                        korisnik.token = token;
                        db.Entry(korisnik).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();

                        return RedirectToAction("Index", "Home");
                    }
                    else if (korisnik.privilegija == 0)
                    {
                        ViewBag.Navigacija = 1;
                        ViewBag.Aktivacija = 1;
                        return View();
                    }
                }               
                else
                {
                    ViewBag.Navigacija = 1;
                    ViewBag.Error = 1;
                    return View();
                }

            }

            return View();
        }
        #endregion

        #region Register
        // GET: Register
        public ActionResult Register()
        {
            ViewBag.Navigacija = 1;
            return View();
        }

        //POST: Register
        [HttpPost]
        public async System.Threading.Tasks.Task<ActionResult> Register(Registracija registracija)
        {
            using (DKEntities db = new DKEntities())
            {
                korisnici k = db.korisnici.Where(l => l.email == registracija.Email).FirstOrDefault();
                if (k == null)
                {
                    korisnici korisnik = new korisnici()
                    {
                        email = registracija.Email,
                        zaporka = CalculateHash.Hash(registracija.Zaporka),
                        ime = registracija.Ime,
                        prezime = registracija.Prezime,
                        privilegija = 1
                    };


                    string token = CalculateHash.Hash(registracija.Email + (DateTime.Now.Day * 45).ToString() + "dasdalčkč");
                    korisnik.token = token;

                    // Provjera korisnika u bazi


                    db.korisnici.Add(korisnik);
                    db.SaveChanges();

                    var apiKey = "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";
                    var client = new SendGridClient(apiKey);
                    var from = new EmailAddress("noreply@tdk.cpsrk.foi.hr", "Tjedan karijera");
                    var subject = "Aktivacija korisničkog računa";
                    var to = new EmailAddress(korisnik.email, korisnik.ime + " " + korisnik.prezime);
                    var plainTextContent = "";
                    string link = "http://tdk.cpsrk.foi.hr/Account/Activate/"+token;
                    var htmlContent = @"Pozdrav,<p>Vaša registracija je uspjela. <br> Molimo Vas da otvorite sljedeći link kako bi aktivirali Vaš račun: <p> <a href=""" + link + @""">Aktivacija</a>";
                    var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
                    //var response = await client.SendEmailAsync(msg);
                }
                else
                {
                    ViewBag.Navigacija = 2;
                    ViewBag.Postoji = 1;
                    return View();
                }
            }

            TempData["Registracija"] = 1;
            return RedirectToAction("Login", "Account");
        }

        public ActionResult Activate(string id)
        {
            DKEntities db = new DKEntities();
            korisnici k = db.korisnici.Where(l => l.token == id).FirstOrDefault();

            if (k != null)
            {
                k.privilegija = 1;
                db.Entry(k).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            TempData["Aktivacija"] = 1;
            
            return RedirectToAction("Login", "Account");
        }
        #endregion

            #region My

        [Autorizacija(minPrivilegija: 1)]
        public ActionResult Profil()
        {
            korisnici korisnik = Korisnici.DohvatiTrenutnogKorisnika();

            if(korisnik == null)
            {
                return RedirectToAction("Login", "Account");
            }

            Korisnik temp = new Korisnik()
            {
                id_korisnika = korisnik.id_korisnika,
                ime = korisnik.ime,
                prezime = korisnik.prezime,
                broj_telefona = korisnik.broj_telefona,
                interesi = korisnik.interesi,
                smjer = korisnik.smjer                
            };

            
            return View(temp);
        }

        [HttpPost]
        [Autorizacija(minPrivilegija: 1)]
        public ActionResult Profil(Korisnik korisnik)
        {
            korisnici trenutniKorisnik = Korisnici.DohvatiTrenutnogKorisnika();

            if (korisnik == null)
            {
                return RedirectToAction("Login", "Account");
            }

            trenutniKorisnik.ime = korisnik.ime;
            trenutniKorisnik.prezime = korisnik.prezime;
            trenutniKorisnik.godina = korisnik.godina;
            trenutniKorisnik.interesi = korisnik.interesi;
            trenutniKorisnik.smjer = korisnik.smjer;
            trenutniKorisnik.broj_telefona = korisnik.broj_telefona;

            if (korisnik.slika != null && korisnik.slika.ContentLength > 0)
            {
                var contentName = korisnik.slika.FileName;
                FileInfo info = new FileInfo(contentName);
                var contentLength = korisnik.slika.ContentLength;
                var contentType = korisnik.slika.ContentType;

                // Get file data
                byte[] data = new byte[] { };
                using (var binaryReader = new BinaryReader(korisnik.slika.InputStream))
                {
                    data = binaryReader.ReadBytes(korisnik.slika.ContentLength);
                }

                if (contentType.Contains("image"))
                {
                    
                        string url = AppDomain.CurrentDomain.BaseDirectory + "/Content/Images/Profile/" + trenutniKorisnik.id_korisnika + info.Extension;
                        System.IO.File.WriteAllBytes(url, data);
                        trenutniKorisnik.slika = info.Extension;
                   
                }
            }

            using(DKEntities db = new DKEntities())
            {
                db.Entry(trenutniKorisnik).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }

            ViewBag.Uspjesno = "Uspješno spremljeni podaci.";

                return View(korisnik);
        }
        #endregion

    }
}