using System.Linq;
using System.Web.Mvc;
using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;
using SportsStore.WebUI.Models;

namespace SportsStore.WebUI.Controllers
{
    public class CartController : Controller
    {
        private readonly IProductRepository _repository;
        private IOrderProcessor _orderProcessor;

        public CartController(IProductRepository repository, IOrderProcessor orderProcessor)
        {
            _repository = repository;
            _orderProcessor = orderProcessor;
        }


        [HttpPost]
        public ViewResult Checkout(ShoppingCart cart, ShippingDetails shippingDetails)
        {
            if (!cart.Lines.Any())
            {
                ModelState.AddModelError("", "Sorry, your cart is empty");
            }

            if (ModelState.IsValid)
            {
                _orderProcessor.ProcessOrder(cart, shippingDetails);
                cart.Clear();
                return View("Completed");
            }
            else
            {
                return View(shippingDetails);
            }
        }

        public ViewResult Index(ShoppingCart cart,string returnUrl)
        {
            return View(new CartIndexViewModel
            {
                Cart = cart,
                ReturnUrl = returnUrl
            });
        }

        //private ShoppingCart GetCart()
        //{
        //    ShoppingCart cart = (ShoppingCart)Session["Cart"];
        //    if (cart == null)
        //    {
        //        cart = new ShoppingCart();
        //        Session["Cart"] = cart;
        //    }

        //    return cart;
        //}

        public ViewResult Checkout()
        {
            return View(new ShippingDetails());
        }

        public PartialViewResult Summary(ShoppingCart cart)
        {
            return PartialView(cart);
        }


        public RedirectToRouteResult AddToCart(ShoppingCart cart, int productId, string returnUrl)
        {
            Product product = _repository.Products.FirstOrDefault(p => p.ProductId == productId);

            if (product != null)
            {
               cart.AddItem(product, 1);
            }

            return RedirectToAction("Index", new { returnUrl });
        }

        public RedirectToRouteResult RemoveFromCart(ShoppingCart cart, int productId, string returnUrl)
        {
            Product product = _repository.Products.FirstOrDefault(p => p.ProductId == productId);

            if (product != null)
            {
                cart.RemoveItem(product);
            }

            return RedirectToAction("Index", new {returnUrl});
        }
    }
}