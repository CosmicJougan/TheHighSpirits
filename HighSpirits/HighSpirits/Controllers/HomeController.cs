using HighSpirits.Models;
using HighSpirits.Persistence;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using System.Security.Claims;
using System.Diagnostics;


namespace HighSpirits.Controllers
{
    public class HomeController : Controller
    {
        PersistenceCode pc = new PersistenceCode();

        //[Authorize]
        [HttpGet]
        public IActionResult Index(ProductRepository productRepository)
        {
            var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, 1.ToString())
                    };
            var userIdentity = new ClaimsIdentity(claims, "SecureLogin");
            var userPrincipal = new ClaimsPrincipal(userIdentity);
            HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                userPrincipal,
                new AuthenticationProperties { ExpiresUtc = DateTime.Now.AddDays(2), IsPersistent = false });

            productRepository.Producten = pc.loadProducten();
            return View(productRepository);
        }

        //[Authorize]
        [HttpPost]
        public IActionResult Index()
        {
            return RedirectToAction("Winkelmandje");
        }

        //[Authorize]
        [HttpGet]
        public IActionResult Toevoegen(Winkelmandje winkelmandje, int ProductID)
        {
            VMtoevoegen vmToevoegen = new VMtoevoegen();

            winkelmandje.KlantId = Convert.ToInt32(User.Identity.Name);
            winkelmandje.ProductId = ProductID;

            Product product = new Product();
            product = pc.laadProduct(winkelmandje.ProductId);
            vmToevoegen.product = product;
            HttpContext.Session.SetInt32("ProductId", winkelmandje.ProductId);
            if (pc.zoekVoorraad(winkelmandje) == 0)
            {
                ViewBag.Voorraad = "Out of stock.";
                return View(vmToevoegen);
            }
            else
            {
                ViewBag.Voorraad = pc.zoekVoorraad(winkelmandje);
            }

                return View(vmToevoegen);
        }

        //[Authorize]
        [HttpPost]
        public IActionResult Toevoegen(VMtoevoegen vmToevoegen)

        {
            Product product = new Product();
            product = pc.laadProduct(Convert.ToInt32(HttpContext.Session.GetInt32("ProductId")));
            vmToevoegen.product = product;

            Winkelmandje winkelmandje = new Winkelmandje();
            winkelmandje.KlantId = Convert.ToInt32(User.Identity.Name);
            winkelmandje.ProductId = product.ProductID;
            winkelmandje.Aantalstuks = vmToevoegen.winkelmandje.Aantalstuks;
            vmToevoegen.winkelmandje = winkelmandje;

            if (pc.zoekVoorraad(winkelmandje) == 0)
            {
                ViewBag.Voorraad = "Out of stock.";
                return View(vmToevoegen);
            }
            else
            {
                ViewBag.Voorraad = pc.zoekVoorraad(winkelmandje);
                if(winkelmandje.Aantalstuks > 0)
                {
                    if (pc.zoekVoorraad(winkelmandje) >= winkelmandje.Aantalstuks)
                    {
                        if (pc.checkProduct(winkelmandje))
                        {
                            pc.updateWinkelmandje(winkelmandje);
                        }
                        else
                        {
                            pc.insertInWinkelmandje(vmToevoegen.winkelmandje);
                        }
                        return RedirectToAction("Winkelmandje");
                    }
                    else
                    {
                        ViewBag.Fout = "Niet genoeg voorraad over. Pak minder, alchoholist.";
                        return View(vmToevoegen);
                    }
                }
                else
                {
                    ViewBag.Fout = "Positief getal in voeren!";
                    return View(vmToevoegen);
                }
            }
            
            
        }

        [HttpGet]
        public IActionResult Delete(int ProductID)
        {
            Winkelmandje winkelmandje = new Winkelmandje();
            winkelmandje.ProductId = ProductID;
            winkelmandje.KlantId = Convert.ToInt32(User.Identity.Name);
            winkelmandje = pc.laadWinkelmandje(winkelmandje);
            pc.deleteUitWinkelmandje(winkelmandje);

            return RedirectToAction("Winkelmandje");
        }

        //[Autherize]
        [HttpGet]
        public IActionResult Winkelmandje()
        {
            VMwinkelmandje vmWinkelmandje = new VMwinkelmandje();

            Winkelmandje winkelmandje = new Winkelmandje();
            winkelmandje.KlantId = Convert.ToInt32(User.Identity.Name);

            LoginCredentials klant = new LoginCredentials();
            klant = pc.laadKlant(Convert.ToInt32(User.Identity.Name));
            vmWinkelmandje.Klant = klant;

            ProductRepository productRepository = new ProductRepository();
            productRepository.Producten = pc.laadWinkelmandjeProducten(winkelmandje);
            vmWinkelmandje.productRepository = productRepository;

            Totalen totalen = new Totalen();
            totalen = pc.haalTotalen(winkelmandje);
            vmWinkelmandje.totalen = totalen;

            DateTime BestelDatum = DateTime.Now.Date;
            ViewBag.Datum = BestelDatum;

            return View(vmWinkelmandje);
        }

    }
}


