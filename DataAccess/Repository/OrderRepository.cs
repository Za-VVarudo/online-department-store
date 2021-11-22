using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObject;
using BusinessObject.Models;
namespace DataAccess.Repository
{
    public class OrderRepository : IOrderRepository
    {
        private OrderDAO instance = OrderDAO.Instance;
        public OrderObject GetOrderInfo(int orderID) => instance.GetOrderInformation(orderID);
        public IEnumerable<OrderObject> GetOrderList(int memberID) => instance.GetOrderList(memberID);
        public IEnumerable<OrderObject> GetOrderList() => instance.GetOrderList();
        public IEnumerable<OrderDetailObject> GetOrderDetailList(int orderID) => instance.GetOrderDetailList(orderID);
        public int CreateOrder(OrderObject order) => instance.CreateOrder(order);
        public bool UpdateOrder(OrderObject order) => instance.UpdateOrder(order);
        public bool DeleteOrder(int orderID) => instance.DeleteOrder(orderID);
        public int InsertOrderDetail(OrderDetailObject[] orderDetail) => instance.InsertOrderDetail(orderDetail);
        public IEnumerable<DailySaleStatisticResult> GetStatisticSale(DateTime from, DateTime to) => instance.GetStatisticSale(from, to);
    }
}
