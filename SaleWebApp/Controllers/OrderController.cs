using Microsoft.AspNetCore.Mvc;
using BusinessObject.Models;
using DataAccess.Repository;
using Microsoft.AspNetCore.Http;
using SaleWebApp.Models;
using System;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using SaleWebApp.Helpers;

namespace SaleWebApp.Controllers
{
    public class OrderController : Controller
    {
        private IOrderRepository orderRepo;
        private MemberObject loginMember;
        private readonly IConfiguration builder;
        public OrderController()
        {
            builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                                                .AddJsonFile("appsettings.json", true, true)
                                                .Build();
            orderRepo = new OrderRepository();
        }
        public async Task<IActionResult> GetOrders()
        {
            loginMember = GetSessionRole(HttpContext);
            if (loginMember == null || (loginMember != null && loginMember.MemberId > 0)) return RedirectToHomePage();
            List<OrderObject> orderList;
            string url = builder["APIController:Order"];
            using (var httpClient = new HttpClient())
            {
                var result = await httpClient.HttpGetHelper<List<OrderObject>>(url, "orderList");
                orderList = result.content;
            }
            
            return View(orderList);
        }
        public async Task<IActionResult> GetOrderDetails(int orderId, int memberId)
        {
            loginMember = GetSessionRole(HttpContext);
            if (loginMember == null) return RedirectToHomePage();
            else if (loginMember.MemberId > 0 && memberId != loginMember.MemberId) return RedirectToHomePage();
            var url = builder["APIController:Order"] + "/detail/" + orderId;
            List<OrderDetailObject> detailList;
            using (var httpClient = new HttpClient())
            {
                var result = await httpClient.HttpGetHelper<List<OrderDetailObject>>(url, "detailList");
                detailList = result.content;
            }
            return View(detailList);
        }
        public async Task<IActionResult> GetOrdersHistory()
        {
            loginMember = GetSessionRole(HttpContext);
            if (loginMember == null || (loginMember != null && loginMember.MemberId == 0)) return RedirectToHomePage();
            List<OrderObject> orderList;
            string url = builder["APIController:Order"] + "/history/" + loginMember.MemberId;
            using (var httpClient = new HttpClient())
            {
                var result = await httpClient.HttpGetHelper<List<OrderObject>>(url, "orderList");
                orderList = result.content;
            }
            return View(orderList);
        }
        public async Task<IActionResult> DeleteOrder(int OrderId)
        {
            loginMember = GetSessionRole(HttpContext);
            if (loginMember == null || (loginMember != null && loginMember.MemberId > 0)) return RedirectToHomePage();
            bool valid = false;
            string url = builder["APIController:Order"] + "/" + OrderId;
            string message = "";
            using (var httpClient = new HttpClient())
            {
                var result = await httpClient.HttpDeleteHelper<bool>(url, "valid");
                valid = result.content;
                message = result.message;
            }
            if (valid)
            {
                TempData["Message"] = $" Order: {OrderId} Removed ";
            }
            else
            {
                TempData["Message"] = $"Can not remove Order: {OrderId} or order not found ";
            }
            return RedirectToAction("GetOrders");
        }
        public IActionResult UpdateOrder(int OrderId)
        {
            loginMember = GetSessionRole(HttpContext);
            if (loginMember == null || (loginMember != null && loginMember.MemberId > 0)) return RedirectToHomePage();
            var orderObject = orderRepo.GetOrderInfo(OrderId);
            if (orderObject != null)
            {
                return View(orderObject);
            }
            return RedirectToHomePage();
        }
        [HttpPost]
        public async Task<IActionResult> UpdateOrder(OrderUpdateModel updateModel)
        {
            loginMember = GetSessionRole(HttpContext);
            if (loginMember == null || (loginMember != null && loginMember.MemberId > 0)) return RedirectToHomePage();
            else
            {
                if (updateModel.RequiredDate.CompareTo(updateModel.OrderDate) < 0 || updateModel.ShippedDate.CompareTo(updateModel.OrderDate) < 0)
                {
                    ViewBag.Message = "Required and Shipped Date must >= OrderDate";
                    return View();
                }
                if (!ModelState.IsValid) return View();
                var orderObject = new OrderObject()
                {
                    OrderId = updateModel.OrderId,
                    RequiredDate = updateModel.RequiredDate.AddHours(23).AddMinutes(59).AddSeconds(59),
                    ShippedDate = updateModel.ShippedDate.Date.AddHours(23).AddMinutes(59).AddSeconds(59),
                    OrderDate = updateModel.OrderDate,
                    Freight = updateModel.Freight
                };
                bool valid = false;
                string url = builder["APIController:Order"];
                string message = "";
                using (var httpClient = new HttpClient())
                {
                    var task = await httpClient.HttpPutHelper<bool>(url, orderObject, "valid");
                    valid = task.content;
                    message = task.message;
                }
                if (valid)
                {
                    ViewBag.Message = "Update Successfully";
                }
                else
                {
                    ViewBag.Message = "Update Failed " + message;
                }
                return View();
            }
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
