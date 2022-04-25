using OrderService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderService.Repositories.Interfaces
{
    public interface IOrderCreator
    {
        Task<int> Create(OrderDetail orderDetail);
    }
}
