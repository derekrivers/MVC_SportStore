﻿using System.Linq;
using System.Web;
using System.Web.Mvc;
using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;

namespace SportsStore.WebUI.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {

        private IProductRepository _repository;

        public AdminController(IProductRepository repository)
        {
            _repository = repository;
        }

        public ViewResult Index()
        {
            return View(_repository.Products);
        }

        public ViewResult Create(Product product)
        {
            return View("Edit", new Product());
        }

        [HttpPost]
        public ActionResult Delete(int productId)
        {
            Product deletedProduct = _repository.DeleteProduct(productId);

            if (deletedProduct != null)
            {
                TempData["message"] = string.Format("{0} was deleted", deletedProduct.Name);
            }

            return RedirectToAction("Index");
        }


        [HttpPost]
        public ActionResult Edit(Product product, HttpPostedFileBase image = null)
        {
            if (ModelState.IsValid)
            {
                if (image != null)
                {
                    product.ImageMimeType = image.ContentType;
                    product.ImageData = new byte[image.ContentLength];
                    image.InputStream.Read(product.ImageData, 0, image.ContentLength);
                }

                _repository.SaveProduct(product);

                TempData["message"] = string.Format("{0} has been saved", product.Name);

                return RedirectToAction("Index");
            }
            else
            {
                // Invalid data
                return View(product);
            }
        }

        public ViewResult Edit(int productId)
        {
            Product product = _repository.Products.FirstOrDefault(p => p.ProductId == productId);

            return View(product);
        }
    }
}