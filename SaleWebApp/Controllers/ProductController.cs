using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccess.Repository;
using BusinessObject.Models;
using Microsoft.AspNetCore.Http;
using SaleWebApp.Models;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Net.Http;
using SaleWebApp.Helpers;

namespace SaleWebApp.Controllers
{
    public class ProductController : Controller
    {
        private IProductRepository productRepo;
        private MemberObject loginMember;
        private readonly IConfiguration builder;
        public ProductController()
        {
            builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                                                .AddJsonFile("appsettings.json", true, true)
                                                .Build();
            productRepo = new ProductRepository();
        }
        
        public async Task<IActionResult> GetProducts(string search = "", int quantity = 2, decimal priceFrom = 0, decimal priceTo = 99999999999)
        {
            loginMember = GetSessionRole(HttpContext);
            if (loginMember == null || (loginMember != null && loginMember.MemberId > 0)) return RedirectToHomePage();
            if (priceFrom > priceTo)
            {
                var temp = priceTo;
                priceTo = priceFrom;
                priceFrom = temp;
            }
            search = search ?? "";
            IEnumerable<ProductObject> productList;
            string url = builder["APIController:Product"];
            using (var httpClient = new HttpClient())
            {
                var result = await httpClient.HttpGetHelper<List<ProductObject>>(url, "productList");
                productList = (productList = result.content) ??  new List<ProductObject>();
            }
            productList = from p in productList
                          where (p.ProductId.ToString().Equals(search) || p.ProductName.ToLower().Contains(search.ToLower()))
                                 && p.UnitPrice >= priceFrom && p.UnitPrice <= priceTo
                          orderby p.CategoryId
                          select p;
            if (quantity == 0) productList = productList.Where(p =>p.UnitsInStock == 0);
            if (quantity == 1) productList = productList.Where(p => p.UnitsInStock > 0);
            ViewBag.Quantity = quantity;
            ViewBag.Search = search;
            ViewBag.From = priceFrom;
            ViewBag.To = priceTo;
            return View(productList);
        }
        [HttpGet]
        public IActionResult UpdateProduct(int ProductId)
        {
            loginMember = GetSessionRole(HttpContext);
            if (loginMember == null || (loginMember != null && loginMember.MemberId > 0)) return RedirectToHomePage();
            var product = productRepo.GetProduct(ProductId);
            if (product != null)
            {
                return View(product);
            }
            return RedirectToHomePage();
        }
        [HttpPost]
        public async Task<IActionResult> UpdateProduct(ProductUpdateModel productModel)
        {
            loginMember = GetSessionRole(HttpContext);
            if (loginMember == null || (loginMember != null && loginMember.MemberId > 0)) return RedirectToHomePage();
            if (!ModelState.IsValid) return View();
            var productUpdate = new ProductObject()
            {
                ProductId = productModel.ProductId,
                ProductName = productModel.ProductName,
                CategoryId = productModel.CategoryId,
                Weight = productModel.Weight,
                UnitPrice = productModel.UnitPrice,
                UnitsInStock = productModel.UnitsInStock
            };
            bool valid = false;
            string url = builder["APIController:Product"];
            using (var httpClient = new HttpClient())
            {
                var task = await httpClient.HttpPutHelper<bool>(url, productUpdate, "valid");
                valid = task.content;
            }
            if (valid)
            {
                ViewBag.Message = "Update Successfully";
            }
            else
            {
                ViewBag.Message = "Update Failed";
            }
            return View(productUpdate);
        }
        [HttpGet]
        public IActionResult AddNewProduct()
        {
            loginMember = GetSessionRole(HttpContext);
            if (loginMember == null || (loginMember != null && loginMember.MemberId > 0)) return RedirectToHomePage();
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddNewProduct(ProductModel productModel)
        {
            loginMember = GetSessionRole(HttpContext);
            if (loginMember == null || (loginMember != null && loginMember.MemberId > 0)) return RedirectToHomePage();
            if (!ModelState.IsValid) return View();
            var productAdd = new ProductObject()
            {
                ProductName = productModel.ProductName,
                CategoryId = productModel.CategoryId,
                Weight = productModel.Weight,
                UnitPrice = productModel.UnitPrice,
                UnitsInStock = productModel.UnitsInStock
            };
            bool valid = false;
            string url = builder["APIController:Product"];
            using (var httpClient = new HttpClient())
            {
                var task = await httpClient.HttpPostHelper<bool>(url, productAdd, "valid");
                valid = task.content;
            }
            if (valid)
            {
                return RedirectToAction("GetProducts");
            }
            else
            {
                ViewBag.Message = "Failed to Add Product";
                return View(productAdd);
            }
        }
        public async Task<IActionResult> DeleteProduct(int ProductId)
        {
            loginMember = GetSessionRole(HttpContext);
            if (loginMember == null || (loginMember != null && loginMember.MemberId > 0)) return RedirectToHomePage();
            bool valid = false;
            string url = builder["APIController:Product"] + $"?productId={ProductId}";
            using (var httpClient = new HttpClient())
            {
                var task = await httpClient.HttpDeleteHelper<bool>(url, "valid");
                valid = task.content;
            }
            if (valid)
            {
                TempData["Message"] = $"Product ID: {ProductId} Removed ";
            }
            else
            {
                TempData["Message"] = "Failed to Delete Product (Someone had already bought this product or Product is not exist)";    
            }
            return RedirectToAction("GetProducts");
        }
        private MemberObject GetSessionRole(HttpContext context)
        {
            var session = context.Session;
            MemberObject member = null;
            string role = session.GetString("ROLE");
            if (role != null)
            {
                member = new MemberObject
                {
                    MemberId = session.GetInt32("MEMBER_ID") ?? 0
                };
            }
            return member;
        }

        private IActionResult RedirectToHomePage()
        {
            return RedirectToAction("Index", "Home");
        }
    }
}
