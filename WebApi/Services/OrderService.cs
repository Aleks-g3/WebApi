using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Entities;
using WebApi.Helpers;
using WebApi.Values;

namespace WebApi.Services
{
    public interface IOrderService
    {
        Order Create(Order order);
        Order Update(int id, string status);
        IEnumerable<Order> GetByStatus();
        IEnumerable<Order> GetByUser(int id);

    }

    public class OrderService : IOrderService
    {
        DataContext _context;


        public OrderService(DataContext context)
        {
            _context = context;
        }
        public Order Create(Order order)
        {
            var user = _context.Users.First(u => u.Id == order.User.Id);
            order.Sum = 0;
            //if (order.Products_order == null)
            //    order.Products_order = new ObservableCollection<Product_Order>();
            foreach (var product_Order in order.Products_order)
            {
                product_Order.product.Type = typeProduct.orderProduct;
                var product = _context.Products.First(p => p.Name == product_Order.product.Name);
                order.Sum += product_Order.PriceEach;
                product.Amount -= product_Order.count;
                _context.Products.Update(product);
            }
            if (user.Role == "User")
            {
                user.Credit -= order.Sum;
                order.Payment = Payment.Credit;
            }
            order.orderTimes = DateTime.Now;

            order.Status = Status.Preparing;
            order.User = user;
            _context.Orders.Add(order);
            _context.Users.Update(user);
            _context.SaveChanges();

            return order;
        }

        public Order Update(int id, string status)
        {
            var order = _context.Orders.Find(id);
            if (order != null)
            {
                order.orderTimes = DateTime.Now;
                if (order.Status == Status.Preparing)
                    order.Status = Status.ready_to_receive;
                else
                    order.Status = Status.received;

                _context.Orders.Update(order);
                _context.SaveChanges();
            }
            
            return order;

        }

        public IEnumerable<Order> GetByStatus()
        {
            return _context.Orders.Include(o=>o.User).Include(o=>o.Products_order).ThenInclude(product_Order=>product_Order.product)
                .Where(o => o.Status == Status.Preparing || o.Status == Status.ready_to_receive);
            
            
        }
        public IEnumerable<Order> GetByUser(int id)
        {
            return _context.Orders.Where(o => o.User.Id == id && o.Status!=Status.received);
        }
    }
}
