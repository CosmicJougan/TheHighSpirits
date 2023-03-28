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

        [Authorize]
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

        [Authorize]
        [HttpPost]
        public IActionResult Index()
        {
            return RedirectToAction("Winkelmandje");
        }

        [Authorize]
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

        [Authorize]
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
                    ViewBag.Fout = "Positief geheel getal in voeren!, dit is geen wiskunde.";
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

        [Authorize]
        [HttpGet]
        public IActionResult Winkelmandje(VMwinkelmandje vmWinkelmandje)
        {
            Winkelmandje winkelmandje = new Winkelmandje();
            winkelmandje.KlantId = Convert.ToInt32(User.Identity.Name);

            LoginCredentials klant = new LoginCredentials();
            klant = pc.laadKlant(Convert.ToInt32(User.Identity.Name));
            vmWinkelmandje.Klant = klant;

            ProductRepository productRepository = new ProductRepository();
            productRepository.Producten = pc.laadWinkelmandjeProducten(Convert.ToInt32(User.Identity.Name));
            vmWinkelmandje.productRepository = productRepository;


            Totalen totalen = new Totalen();

            if (productRepository.Producten.Count>0)
            {
                
                totalen = pc.haalTotalen(winkelmandje);
                
            }
            else
            {
                totalen.PrijsExclusief = 0.00;
                totalen.BTW = 0.00;
                totalen.PrijsInclusief = 0.00;
                ViewBag.leeg = "Winkemandje is leeg, koop meer.";
            }

            vmWinkelmandje.totalen = totalen;

            DateTime BestelDatum = DateTime.Now.Date;
            ViewBag.Datum = BestelDatum;

            return View(vmWinkelmandje);
        }

        [Authorize]
        [HttpPost]
        public IActionResult Winkelmandje()
        {
            ProductRepository productRepository = new ProductRepository();
            productRepository.Producten = pc.laadWinkelmandjeProducten(Convert.ToInt32(User.Identity.Name));

            if (productRepository.Producten.Count > 0)
            {
                return RedirectToAction("BestelBevestiging");
            }
            else
            {
                return RedirectToAction("Winkelmandje");
            }
        }

        [Authorize]
        [HttpGet]
        public IActionResult BestelBevestiging(VMbestellingen vmBestellingen)
        {
            Bestelling bestelling = new Bestelling();
            bestelling.KlantId = Convert.ToInt32(User.Identity.Name);
            bestelling.Datum = DateTime.Now;
            pc.BestelItems(bestelling);
            bestelling = pc.laadLaatsteBestelling(Convert.ToInt32(User.Identity.Name));

            vmBestellingen.bestelling = bestelling;

            Winkelmandje winkelmandje = new Winkelmandje();
            winkelmandje.KlantId = Convert.ToInt32(User.Identity.Name);

            Totalen totalen = new Totalen();
            totalen = pc.haalTotalen(winkelmandje);
            vmBestellingen.totalen = totalen;

            ProductRepository productRepository = new ProductRepository();
            productRepository.Producten = pc.laadWinkelmandjeProducten(Convert.ToInt32(User.Identity.Name));

            foreach(var product in productRepository.Producten)
            {
                Bestellijn bestellijn = new Bestellijn();
                bestellijn.BestellingId = bestelling.BestellingId;
                bestellijn.ProductId = product.ProductID;
                bestellijn.HistorischePrijs = product.Verkoopprijs;
                bestellijn.Aantalstuks = product.Voorraad;
                pc.insertIntoBestelLijn(bestellijn);
            }
            pc.stuurMail(vmBestellingen);
            pc.clearWinkelmandje(Convert.ToInt32(User.Identity.Name));

            return View(vmBestellingen);
        }

    }
}


