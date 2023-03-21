using HighSpirits.Models;
using HighSpirits.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using System.Diagnostics;


namespace HighSpirits.Controllers
{
    public class HomeController : Controller
    {
        PersistenceCode pc = new PersistenceCode();

        //[Authorize]
        [HttpGet]
        public IActionResult Index()
        {
            ProductRepository productRepository = new ProductRepository();
            productRepository.Producten = pc.loadProducten();
            return View(productRepository);
        }

        //[Authorize]
        [HttpPost]
        public IActionResult Index(ProductRepository productRepository)
        {
            productRepository.Producten = pc.loadProducten();
            return View(productRepository);
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
    }
}