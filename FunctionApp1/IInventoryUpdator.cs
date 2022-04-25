using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace InventoryService
{
    public interface IInventoryUpdator
    {
        Task Update(int productId, int quantity);
    }
}
