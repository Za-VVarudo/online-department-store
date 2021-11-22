using BusinessObject.Models;
using DataAccess.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SaleWebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Net.Http;
using SaleWebApp.Helpers;
using System.Threading.Tasks;

namespace SaleWebApp.Controllers
{
    public class OrderDetailController : Controller
    {
        private int orderId = 0;
        private Dictionary<int, OrderDetailObject> orderDetailList = new();
        private IProductRepository productRepo;
        private IOrderRepository orderRepo;
        private MemberObject loginMember;
        private IConfiguration builder;
        public OrderDetailController()
        {
            productRepo = new ProductRepository();
            orderRepo = new OrderRepository();
            builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true, true)
                .Build();
        }
        public IActionResult CreateOrder()
        {
            loginMember = GetSessionRole(HttpContext);
            if (loginMember == null || (loginMember != null && loginMember.MemberId > 0)) return RedirectToHomePage();
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateOrder(OrderInsertModel insertModel)
        {
            loginMember = GetSessionRole(HttpContext);
            if (loginMember == null || (loginMember != null && loginMember.MemberId > 0)) return RedirectToHomePage();
            else
            {
                var memberObject = new MemberRepository().GetMemberInfo(insertModel.MemberId);
                if (memberObject == null)
                {
                    ViewBag.Message = "Member not found";
                    return View();
                }
                else
                {
                    insertModel.RequiredDate = insertModel.RequiredDate.Date.AddHours(23).AddMinutes(59).AddSeconds(59);
                    insertModel.ShippedDate = insertModel.ShippedDate.Date.AddHours(23).AddMinutes(59).AddSeconds(59);
                    var today = DateTime.Now.Date;
                    if (insertModel.RequiredDate.CompareTo(today) < 0 || insertModel.ShippedDate.CompareTo(today) < 0)
                    {
                        ViewBag.Message = "Required and Shipped Date must >= today";
                        return View();
                    }
                    if (!ModelState.IsValid) return View();
                    var orderObject = new OrderObject()
                    {
                        MemberId = insertModel.MemberId,
                        RequiredDate = Convert.ToDateTime(insertModel.RequiredDate),
                        ShippedDate = Convert.ToDateTime(insertModel.ShippedDate),
                        Freight = insertModel.Freight
                    };
                    int orderIdReturn = -1;
                    string url = builder["APIController:Order"];
                    using (var httpClient = new HttpClient())
                    {
                        var task = await httpClient.HttpPostHelper<int>(url, orderObject, "orderId");
                        orderIdReturn = task.content;
                    }
                    if (orderIdReturn > 0)
                    {
                        return AddProductToOrder(orderIdReturn);
                    }
                    ViewBag.Message = "Create Failed";
                    return View();
                }
            }
        }
        [HttpGet]
        private IActionResult AddProductToOrder(int orderId)
        {
            loginMember = GetSessionRole(HttpContext);
            if (loginMember == null || (loginMember != null && loginMember.MemberId > 0)) return RedirectToHomePage();
            
            var session = HttpContext.Session;
            this.orderId = GetOrderId(session);
            orderDetailList = GetDetailList(session);

            if ( this.orderId != orderId)
            {
                orderDetailList = new();
                SetOrderId(session, orderId);
                SetDetailList(session);
            }
            ViewBag.OrderId = orderId;
            ViewBag.DetailList = orderDetailList.Values.ToList();
            return View("AddProductToOrder");
        }
        [HttpPost]
        public async Task<IActionResult> AddProductToOrder(OrderDetailModel detailModel)
        {
            loginMember = GetSessionRole(HttpContext);
            if (loginMember == null || (loginMember != null && loginMember.MemberId > 0)) return RedirectToHomePage();

            var session = HttpContext.Session;
            this.orderId = GetOrderId(session);
            orderDetailList = GetDetailList(session);

            if (ModelState.IsValid)
            {
                ProductObject productObject = null;
                string url = builder["APIController:Product"] + $"/{detailModel.ProductId}";
                using (var httpClient = new HttpClient())
                {
                    var task = await httpClient.HttpGetHelper<ProductObject>(url, "product");
                    productObject = task.content;
                }
                if (productObject != null && productObject.UnitsInStock > 0)
                {
                    var detailObject = new OrderDetailObject()
                    {
                        OrderId = this.orderId,
                        ProductId = productObject.ProductId,
                        UnitPrice = productObject.UnitPrice,
                        Quantity = detailModel.Quantity,
                        Discount = detailModel.Discount
                    };
                    if (detailObject.Quantity > productObject.UnitsInStock) detailObject.Quantity = productObject.UnitsInStock;
                    if (orderDetailList.ContainsKey(detailObject.ProductId))
                    {
                        var valueOfKey = orderDetailList[productObject.ProductId];
                        valueOfKey.Quantity += detailObject.Quantity;
                        if (valueOfKey.Quantity > productObject.UnitsInStock) valueOfKey.Quantity = productObject.UnitsInStock;
                    }
                    else
                    {
                        orderDetailList.Add(detailObject.ProductId, detailObject);
                    }
                    SetDetailList(session);
                }
                else
                {
                    ViewBag.Message = "Product not found or Out of stock";
                }
            }
            ViewBag.OrderId = this.orderId;
            ViewBag.DetailList = orderDetailList.Values.ToList();
            return View();
        }
        public IActionResult DeleteProductFromOrder(int productId)
        {
            loginMember = GetSessionRole(HttpContext);
            if (loginMember == null || (loginMember != null && loginMember.MemberId > 0)) return RedirectToHomePage();
            
            var session = HttpContext.Session;
            this.orderId = GetOrderId(session);
            orderDetailList = GetDetailList(session);

            if (orderDetailList.ContainsKey(productId))
            {
                orderDetailList.Remove(productId);
                SetDetailList(session);
            }
            else
            {
                ViewBag.Message = "Product Id not found";
            }
            ViewBag.OrderId = this.orderId;
            ViewBag.DetailList = orderDetailList.Values.ToList();
            return AddProductToOrder(this.orderId);
        }

        public async Task<IActionResult> ConfirmOrder()
        {
            loginMember = GetSessionRole(HttpContext);
            if (loginMember == null || (loginMember != null && loginMember.MemberId > 0)) return RedirectToHomePage();
            
            var session = HttpContext.Session;
            this.orderId = GetOrderId(session);
            orderDetailList = GetDetailList(session);

            if (orderDetailList.Count() > 0)
            {
                string urlProduct = builder["APIController:Product"];
                string urlOrder = builder["APIController:Order"] + "/details";
                using (var httpClient = new HttpClient()) 
                {
                    var productList = (await httpClient.HttpGetHelper<List<ProductObject>>(urlProduct, "productList")).content;
                    int invalIdId = -1;
                    foreach (var pair in orderDetailList)
                    {
                        var productCheck = productList.Where(p => p.ProductId == pair.Key).FirstOrDefault();
                        if (productCheck == null || (productCheck != null && productCheck.UnitsInStock < pair.Value.Quantity))
                        {
                            invalIdId = pair.Key;
                            ViewBag.Message = $"Product {invalIdId} is not enough or not exist";
                            break;
                        }
                    }
                    if (invalIdId < 0)
                    {
                        bool valId = false;
                        string message = "";
                        var task = await httpClient.HttpPostHelper<bool>(urlOrder, orderDetailList.Values.ToArray(), "valid");
                        message = task.message;
                        valId = task.content;
                        if (valId)
                        {
                            SetOrderId(session, -1);
                            return RedirectToAction("GetOrders", "Order");
                        }
                        else
                        {
                            ViewBag.Message = message;
                        }
                    }
                }
            } 
            else
            {
                ViewBag.Message = "Empty order - added failed";
            }
            ViewBag.OrderId = this.orderId;
            ViewBag.DetailList = orderDetailList.Values.ToList();
            return AddProductToOrder(this.orderId);
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
                    MemberId = session.GetInt32("MEMBER_Id") ?? 0
                };
            }
            return member;
        }
        private int GetOrderId(ISession session)
        {
            return session.GetInt32("ORDER_Id") ?? 0;
        }
        private void SetOrderId(ISession session, int orderId)
        {
            session.SetInt32("ORDER_Id", orderId);
        }
        private Dictionary<int, OrderDetailObject> GetDetailList(ISession session)
        {
            var jsonString = session.GetString("DETAIL_LIST");
            return jsonString == null ? new Dictionary<int, OrderDetailObject>() : JsonConvert.DeserializeObject<Dictionary<int, OrderDetailObject>>(jsonString);
        }
        private void SetDetailList(ISession session)
        {
            var jsonString = JsonConvert.SerializeObject(orderDetailList);
            session.SetString("DETAIL_LIST", jsonString);
        }
        private IActionResult RedirectToHomePage()
        {
            return RedirectToAction("Index", "Home");
        }
    }
}
