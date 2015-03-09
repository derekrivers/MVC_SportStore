using System.Web.Mvc;
using SportsStore.Domain.Entities;


namespace SportsStore.WebUI.Infrastructure.Binders
{
    public class CartModelBinder : IModelBinder
    {
        private const string sessionKey = "Cart";

        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            // get the cart from the session

            ShoppingCart cart = null;

            if (controllerContext.HttpContext.Session != null)
            {
                cart = (ShoppingCart) controllerContext.HttpContext.Session[sessionKey];
            }

            if (cart == null)
            {
                cart = new ShoppingCart();

                if (controllerContext.HttpContext.Session != null)
                {
                    controllerContext.HttpContext.Session[sessionKey] = cart;
                }
            }

            return cart;
        }
    }
}