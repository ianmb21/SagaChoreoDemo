using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderService.Models
{
    public class OrderDetail
    {
        public string User { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public int ProductId { get; set; }
    }
}
