using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Entities
{
    public class Product_Order
    {
        [Key]
        public int Id { get; set; }

        public virtual Product product { get; set; }

        public ushort count { get; set; }

        public double PriceEach { get; set; }
    }
}
