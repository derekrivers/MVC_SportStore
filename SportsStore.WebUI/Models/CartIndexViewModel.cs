using SportsStore.Domain.Entities;

namespace SportsStore.WebUI.Models
{
    public class CartIndexViewModel
    {
        public ShoppingCart Cart { get; set; }
        public string ReturnUrl { get; set; }
    }
}