using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using SportsStore.Domain.Abstract;

namespace SportsStore.WebUI.Controllers
{
    public class NavController : Controller
    {

        private readonly IProductRepository _repository;

        public NavController(IProductRepository repository)
        {
            _repository = repository;
        }

        public PartialViewResult Menu(string category = null, bool horizontalLayout = false)
        {
            ViewBag.SelectedCategory = category;

            IEnumerable<string> categories = _repository.Products.Select(x => x.Category).Distinct().OrderBy(x => x);

            string viewName = horizontalLayout ? "MenuHorizontal" : "Menu";

            return PartialView(viewName, categories);
        }
    }
}