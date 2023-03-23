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
        public IActionResult Toevoegen(int ProductID)
        {
            VMtoevoegen vmToevoegen = new VMtoevoegen();

            Product product = new Product();
            product = pc.laadProduct(ProductID);
            vmToevoegen.product = product;
            HttpContext.Session.SetInt32("ProductId", ProductID);


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

            if(pc.zoekVoorraad(winkelmandje) > winkelmandje.Aantalstuks)
            {
                if (pc.checkProduct(winkelmandje))
                {
                    pc.updateWinkelmandje(winkelmandje);
                }
                else
                {
                    pc.insertInWinkelmandje(vmToevoegen.winkelmandje);
                }
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.Fout = "Niet genoeg voorraad over. Pak minder, alchoholist.";
                return View(vmToevoegen);
            }
            
        }

        //[Autherize]
        [HttpGet]
        public IActionResult Winkelmandje()
        {
            VMwinkelmandje vmWinkelmandje = new VMwinkelmandje();

            Winkelmandje winkelmandje = new Winkelmandje();
            winkelmandje = pc.laadWinkelmandje(Convert.ToInt32(User.Identity.Name));

            LoginCredentials klant = new LoginCredentials();
            klant = pc.laadKlant(Convert.ToInt32(User.Identity.Name));
            vmWinkelmandje.Klant = klant;

            ProductRepository productRepository = new ProductRepository();
            productRepository.Producten = pc.laadWinkelmandjeProducten(winkelmandje);
            vmWinkelmandje.productRepository = productRepository;



            return View(vmWinkelmandje);
        }

    }
}