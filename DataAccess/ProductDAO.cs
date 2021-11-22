using BusinessObject;
using BusinessObject.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace DataAccess
{
    public class ProductDAO
    {
        private static ProductDAO instance;
        private static readonly object instanceLock = new object();
        public static ProductDAO Instance
        {
            get
            {
                lock (instanceLock)
                {
                    return instance == null ? instance = new ProductDAO() : instance;
                }
            }
        }
        private ProductDAO() { }

        public List<ProductObject> GetProductList()
        {
            List<ProductObject> list = new();
            using (var context = new SaleContext())
            {
                list = context.Products.ToList();
            }
            return list;
        }
        public ProductObject GetProduct(int productID)
        {
            ProductObject product = null;
            using (var context = new SaleContext())
            {
                product = context.Products.Find(productID);
            }
            return product;
        }
        public bool Insert(ProductObject product)
        {
            bool valid = false;
            using (var context = new SaleContext())
            {
                try
                {
                    context.Products.Add(product);
                    context.SaveChanges();
                    valid = true;
                }
                catch { }
            }
            return valid;
        }
        public bool Update(ProductObject product)
        {            
            bool valid = false;
            using (var context = new SaleContext())
            {
                try
                {
                    context.Products.Update(product);
                    context.SaveChanges();
                    valid = true;
                }
                catch { }
            }
            return valid;
        }
        public bool Delete(int productID)
        {
            bool valid = false;
            using (var context = new SaleContext())
            {
                using var transaction = context.Database.BeginTransaction();
                try
                {
                    context.Remove(new ProductObject { ProductId = productID });
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
    }
}
