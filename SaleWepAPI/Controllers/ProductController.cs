using BusinessObject.Models;
using DataAccess.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SaleWebAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SaleWepAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private IProductRepository productRepo;
        public ProductController() : base()
        {
            productRepo = new ProductRepository();
        }
        [HttpGet]
        public IActionResult GetProducts()
        {
            var list = productRepo.GetProductList();
            return new JsonResult(new { productList = list, Message = "" });
        }

        [HttpGet("{productId:int}")]
        public IActionResult GetProductInfo(int productId)
        {
            var product = productRepo.GetProduct(productId);
            return new JsonResult( new { Product = product , Message = "" });
        }
        [HttpPost]
        public IActionResult CreateProduct(ProductModel model)
        {
            bool valid = false;
            if (ModelState.IsValid)
            {
                var product = new ProductObject
                {
                    ProductName = model.ProductName,
                    CategoryId = model.CategoryId,
                    Weight = model.Weight,
                    UnitPrice = model.UnitPrice,
                    UnitsInStock = model.UnitsInStock
                };
                valid = productRepo.AddProduct(product);
            }
            else this.HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            return new JsonResult(new { Valid = valid , Message = "" });
        }

        [HttpPut]
        public IActionResult UpdateProduct(ProductUpdateModel model)
        {
            bool valid = false;
            if (ModelState.IsValid)
            {
                var product = new ProductObject
                {
                    ProductId = model.ProductId,
                    ProductName = model.ProductName,
                    CategoryId = model.CategoryId,
                    Weight = model.Weight,
                    UnitPrice = model.UnitPrice,
                    UnitsInStock = model.UnitsInStock
                };
                valid = productRepo.UpdateProduct(product);
            }
            else this.HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            return new JsonResult(new { Valid = valid , Message = "" });
        }

        [HttpDelete]
        public IActionResult DeleteProduct(int productId)
        {
            bool valid = false;
            if (productId > 0)
            {
                valid = productRepo.RemoveProduct(productId);
            }
            else this.HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            return new JsonResult(new { Valid = valid , Message = "" });
        }
    }
}
