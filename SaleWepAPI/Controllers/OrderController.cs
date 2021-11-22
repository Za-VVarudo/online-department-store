using BusinessObject.Models;
using SaleWebAPI.Models;
using DataAccess.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SaleWepAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private IOrderRepository orderRepo;
        public OrderController() : base()
        {
            orderRepo = new OrderRepository();
        }
        [HttpGet]
        public IActionResult GetOrders()
        {
            var list = orderRepo.GetOrderList();
            return new JsonResult(new { OrderList = list , Message = "" });
        }

        [HttpGet("detail/{orderId:int}")]
        public IActionResult GetOrderDetail(int orderId)
        {
            var list = orderRepo.GetOrderDetailList(orderId);
            return new JsonResult(new { detailList = list , Message = "" });
        }

        [HttpGet("{orderId:int}")]
        public IActionResult GetOrderInfo(int orderId)
        {
            OrderObject order = null;
            if (orderId > 0)
            {
                order = orderRepo.GetOrderInfo(orderId);
            }
            else this.HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            return new JsonResult(new { Order = order , Message = "" });
        }

        [HttpGet("history/{memberId:int}")]
        public IActionResult GetOrders(int memberId)
        {
            var list = orderRepo.GetOrderList(memberId);
            return new JsonResult(new { OrderList = list , Message = "" });
        }
        [HttpPost]
        public IActionResult CreateOrder(OrderInsertModel model)
        {
            int orderId = -1;
            if (ModelState.IsValid)
            {
                var order = new OrderObject()
                {
                    MemberId = model.MemberId,
                    RequiredDate = model.RequiredDate,
                    ShippedDate = model.ShippedDate,
                    Freight = model.Freight
                };
                orderId = orderRepo.CreateOrder(order);
            }
            else this.HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            return new JsonResult(new { OrderId = orderId , Message = "" });
        }
        [HttpPut]
        public IActionResult UpdateOrder(OrderUpdateModel model)
        {
            bool valid = false;
            string message = "Invalid Model";
            if (ModelState.IsValid)
            {
                var order = orderRepo.GetOrderInfo(model.OrderId);
                if (order != null)
                {
                    var requiredDate = model.RequiredDate.Date;
                    var shippedDate = model.ShippedDate.Date;
                    var orderDate = order.OrderDate.Date;
                    if (requiredDate.CompareTo(orderDate) >= 0 && shippedDate.CompareTo(orderDate) >= 0)
                    {
                        message = "Failed to update";
                        order.ShippedDate = shippedDate;
                        order.RequiredDate = requiredDate;
                        order.Freight = model.Freight;
                        valid = orderRepo.UpdateOrder(order);
                        if (valid)
                        { 
                            message = "Updated";
                        } 
                    }
                    else
                    {
                        message = "Required and Shipped Date mush more than or equals Order Date";
                        this.HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                    }
                }
                else
                {
                    message = "OrderId Not found";
                    this.HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                }
            }
            else this.HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            return new JsonResult(new { Valid = valid , Message = message });
        }
        [HttpDelete("{orderId:int}")]
        public IActionResult DeleteOrder(int orderId)
        {
            string message = "OrderId must > 0";
            bool valid = false;
            if (orderId > 0)
            {
                valid = orderRepo.DeleteOrder(orderId);
                if (valid)
                {
                    message = "Deleted";
                }
                else
                {
                    message = "OrderId not found";
                    this.HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                }
            }
            else
            {
                this.HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            }
            return new JsonResult(new { Valid = valid, Message = message });
        }

        [HttpPost("details")]
        public IActionResult InsertOrderDetail(OrderDetailObject[] detailList)
        {
            string message = "Model Invalid";
            bool valid = false;
            if (ModelState.IsValid)
            {
                if (detailList != null && detailList.Count() > 0)
                {
                    int productId = orderRepo.InsertOrderDetail(detailList);
                    if (valid = (productId == -1))
                    {
                        message = "Inserted";
                    }
                    else
                    {
                        message = $"ProductId {productId} is out of stock or not found";
                    }
                }
            }
            else
            {
                this.HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            }
            return new JsonResult(new { Valid = valid, Message = message });
        }

        [HttpPost("statistic")]
        public IActionResult GetStatistic(StatisticParamsModel model)
        {
            var list = orderRepo.GetStatisticSale(model.From, model.To);
            return new JsonResult(new { Statistic = list, Message = "Created" });
        }
    }
}
