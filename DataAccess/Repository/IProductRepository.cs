using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository
{
    public interface IProductRepository
    {
        IEnumerable<ProductObject> GetProductList();
        ProductObject GetProduct(int productID);
        bool AddProduct(ProductObject product);
        bool UpdateProduct(ProductObject product);
        bool RemoveProduct(int productID);
    }
}
