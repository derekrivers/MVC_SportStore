using System.Linq;
using System.Web.Mvc;
using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;
using SportsStore.WebUI.Models;

namespace SportsStore.WebUI.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductRepository _repository;
        public int PageSize = 4;

        public ProductController(IProductRepository repository)
        {
            _repository = repository;
        }

        public FileContentResult GetImage(int productId)
        {
            Product prod = _repository.Products.FirstOrDefault(p => p.ProductId == productId);

            if (prod != null)
            {
                return File(prod.ImageData, prod.ImageMimeType);
            }
            else
            {
                return null;
            }
        }


        public ViewResult List(string category,int page = 1)
        {
            ProductsListViewModel model = new ProductsListViewModel
            {
                Products = _repository.Products.Where(p=> category == null || p.Category == category).OrderBy(p => p.ProductId).Skip((page - 1)*PageSize).Take(PageSize),

                PagingInfo = new PagingInfo()
                {
                    CurrentPage = page,
                    ItemsPerPage = PageSize,
                    TotalItems =  category == null ? _repository.Products.Count() : _repository.Products.Count(p => p.Category == category)
                },

                CurrentCategory = category

            };

            return View(model);
        }
    }
}