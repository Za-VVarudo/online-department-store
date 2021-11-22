using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository
{
    public class ProductRepository : IProductRepository
    {
        private ProductDAO instance = ProductDAO.Instance;
        public IEnumerable<ProductObject> GetProductList() => instance.GetProductList();
        public ProductObject GetProduct(int productID) => instance.GetProduct(productID);
        public bool AddProduct(ProductObject product) => instance.Insert(product);
        public bool UpdateProduct(ProductObject product) => instance.Update(product);
        public bool RemoveProduct(int productID) => instance.Delete(productID);
    }
}
