    using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObject;
using Microsoft.Data.SqlClient;
using System.Data;
using BusinessObject.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccess
{
    public class OrderDAO
    {
        private OrderDAO() { }
        private static readonly object instanceLock = new object();
        private static OrderDAO instance = null;
        public static OrderDAO Instance
        {
            get
            {
                lock (instanceLock)
                {
                    return instance == null ? instance = new OrderDAO() : instance;
                }
            }
        }

        public List<OrderObject> GetOrderList(int memberID)
        {
            List<OrderObject> list = new();
            using (var context = new SaleContext())
            {
                list = context.Orders.Where(o => o.MemberId == memberID).ToList();
            }
            return list;
        }

        public List<OrderObject> GetOrderList()
        {
            List<OrderObject> list = new();
            using (var context = new SaleContext())
            {
                list = context.Orders.ToList();
            }
            return list;
        }

        public OrderObject GetOrderInformation(int orderID)
        {
            OrderObject order = null;
            using (var context = new SaleContext())
            {
                order = context.Orders.Where(o => o.OrderId == orderID).FirstOrDefault();
            }
            return order;
        }
        public List<OrderDetailObject> GetOrderDetailList(int orderID)
        {
            List<OrderDetailObject> list = new();
            using (var context = new SaleContext())
            {
                list = context.OrderDetails.Where(d => d.OrderId == orderID).ToList();
            }
            return list;
        }

        public int CreateOrder(OrderObject order)
        {
            int orderID = -1;
            using (var context = new SaleContext())
            {
                try
                {
                    context.Orders.Add(order);
                    context.SaveChanges();
                    orderID = order.OrderId;
                }
                catch { }
            }          
            return orderID;
        }

        public bool UpdateOrder(OrderObject order)
        {
            bool valid = false;
            using (var context = new SaleContext())
            {
                try
                {
                    var orderUpdate = context.Orders.Find(order.OrderId);
                    if (orderUpdate != null)
                    {
                        orderUpdate.RequiredDate = order.RequiredDate;
                        orderUpdate.ShippedDate = order.ShippedDate;
                        orderUpdate.Freight = order.Freight;
                        context.SaveChanges();
                        valid = true;
                    }
                }
                catch { }
            }
            return valid;
        }

        public bool DeleteOrder(int orderID)
        {
            bool valid = false;
            using (var context = new SaleContext())
            {
                using var transaction = context.Database.BeginTransaction();
                try
                {
                    context.Orders.Remove(new OrderObject { OrderId = orderID });
                    context.SaveChanges();
                    transaction.Commit();
                    valid = true;
                }
                catch
                {
                    transaction.Rollback();
                }
            }
            return valid;
        }

        public int InsertOrderDetail(OrderDetailObject[] detailList)
        {
            int productId = -1;
            using (var context = new SaleContext())
            {
                using var transaction = context.Database.BeginTransaction();
                try
                {
                    var productRepo = ProductDAO.Instance;
                    bool valid = true;
                    foreach (var orderDetail in detailList)
                    {
                        _ = orderDetail.OrderId;
                        productId = orderDetail.ProductId;
                        var product = context.Products.Find(productId);
                        if ((valid = (product != null && product.UnitsInStock >= orderDetail.Quantity)))
                        {
                            orderDetail.UnitPrice = product.UnitPrice;
                            context.OrderDetails.Add(orderDetail);
                            product.UnitsInStock -= orderDetail.Quantity;
                        }
                        else break;
                    }
                    
                    if (valid)
                    {
                        productId = -1;
                        context.SaveChanges();
                        transaction.Commit();
                    }
                }
                catch
                {
                    transaction.Rollback();
                }
            }
            return productId;
        }
        public List<DailySaleStatisticResult> GetStatisticSale(DateTime fromDate, DateTime toDate)
        {
            List<DailySaleStatisticResult> list = new();
            using (var context = new SaleContext())
            {
                var orderList = context.Orders;
                var detailList = context.OrderDetails;
                list = (from o in orderList
                        join d in detailList on o.OrderId equals d.OrderId
                        where o.OrderDate.Date.CompareTo(fromDate) >= 0 && o.OrderDate.Date.CompareTo(toDate) <= 0
                        group d by o.OrderDate.Date into gr
                        select new DailySaleStatisticResult { Date = gr.Key, Total = gr.Sum(d => d.Quantity * d.UnitPrice * Convert.ToDecimal(1 - d.Discount))})
                        .OrderByDescending(s => s.Total)
                        .ToList();
            }
            return list;
        }
    }
}
