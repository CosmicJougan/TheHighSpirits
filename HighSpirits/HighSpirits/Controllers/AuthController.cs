using Microsoft.AspNetCore.Mvc;
using HighSpirits.Models;
using HighSpirits.Persistence;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Collections.Generic;
using System;

namespace HighSpirits.Controllers
{
    public class AuthController : Controller
    {
        PersistenceCode pc = new PersistenceCode();

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Login(LoginCredentials loginCr)
        {
            if (ModelState.IsValid)
            {
                if (pc.checkCredentials(loginCr) != -1)
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, Convert.ToString(loginCr.KlantId))
                    };
                    var userIdentity = new ClaimsIdentity(claims, "SecureLogin");
                    var userPrincipal = new ClaimsPrincipal(userIdentity);
                    HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                        userPrincipal,
                        new AuthenticationProperties { ExpiresUtc = DateTime.Now.AddDays(2), IsPersistent = false });

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ViewBag.fout = "Ongeldige login, probeer opnieuw.";
                    return View();
                }
            }
            else
            {
                return View();
            }
        }
        [HttpGet]
        public IActionResult Afmelden()
        {
            HttpContext.SignOutAsync();
            return RedirectToAction("login");
        }
        [HttpGet]
        public IActionResult Registreer()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Registreer(Registratie re)
        {
            if (ModelState.IsValid)
            {
                if (pc.checkNaam(re.Gebruikersnaam) == false)
                {
                    if (re.Wachtwoord == re.BevestigWW)
                    {
                        pc.maakUserAan(re);
                        return RedirectToAction("Login");
                    }
                    else
                    {
                        ViewBag.fout = "Wachtwoord komt niet overeen.";
                        return View();
                    }
                }
                else
                {
                    ViewBag.fout = "Deze gebruikersnaam is al in gebruik.";
                    return View();
                }
            }
            else
            {
                return View();
            }
        }
    }
}
