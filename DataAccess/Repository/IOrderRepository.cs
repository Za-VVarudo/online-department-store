using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObject;
using BusinessObject.Models;
namespace DataAccess.Repository
{
    public interface IOrderRepository
    {
        OrderObject GetOrderInfo(int orderID);
        IEnumerable<OrderObject> GetOrderList(int memberID);
        IEnumerable<OrderDetailObject> GetOrderDetailList(int orderID);
        IEnumerable<OrderObject> GetOrderList();
        int CreateOrder(OrderObject order);
        bool UpdateOrder(OrderObject order);
        bool DeleteOrder(int orderID);
        int InsertOrderDetail(OrderDetailObject[] orderDetail);
        IEnumerable<DailySaleStatisticResult> GetStatisticSale(DateTime from, DateTime to);
    }
}
