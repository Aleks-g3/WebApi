using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Entities;
using WebApi.Helpers;
using WebApi.Values;

namespace WebApi.Services
{
    public interface IProductService
    {
        IEnumerable<Product> GetAll();
        Product GetByName(string name);
        IEnumerable<Product> GettoOrder();
        Product Create(Product product);
        void Update(Product product);
        void Delete(int id);
    }

    public class ProductService : IProductService
    {
        private DataContext _context;

        public ProductService(DataContext context)
        {
            _context = context;
        }

        public IEnumerable<Product> GetAll()
        {
            return _context.Products.Where(p => p.Type == typeProduct.Product);
        }
        public Product GetByName(string name)
        {
            return _context.Products.FirstOrDefault(p => p.Name == name);
        }
        public IEnumerable<Product> GettoOrder()
        {
            return _context.Products.Where(p => p.Amount > 0);
        }
        public Product Create(Product product)
        {
            if (string.IsNullOrWhiteSpace(product.Name))
                throw new AppException("Nazwa jest wymagana");
            if (_context.Products.Any(p => p.Name == product.Name))
                throw new AppException("Nazwa \"" + product.Name + "\" już istnieje");

            if (product.Price == 0)
                throw new AppException("Cena jest wymagana");
            if (string.IsNullOrWhiteSpace(product.Amount.ToString()))
                throw new AppException("Stan jest wymagany");
            _context.Products.Add(product);
            _context.SaveChanges();
            product.Type = typeProduct.Product;
            return product;
        }
        public void Update(Product product)
        {
            var products = _context.Products.FirstOrDefault(p => p.Name == product.Name&& p.Type==typeProduct.Product);
            if (products == null)
                throw new AppException("Produkt nie istnieje");
            if (products.Name != product.Name && !string.IsNullOrWhiteSpace(product.Name))
                products.Name = product.Name;

            if (products.Price != product.Price && !string.IsNullOrWhiteSpace(product.Price.ToString()))
                products.Price = product.Price;
            if (products.Amount != product.Amount && !string.IsNullOrWhiteSpace(product.Amount.ToString()))
                products.Amount = product.Amount;
            _context.Products.Update(products);
            _context.SaveChanges();
        }
        public void Delete(int id)
        {
            var products = _context.Products.Find(id);

            if (products != null)
            {
                _context.Products.Remove(products);
                _context.SaveChanges();
            }
        }

    }
}
