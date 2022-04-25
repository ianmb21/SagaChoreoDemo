using System;
using System.Collections.Generic;
using System.Text;

namespace InventoryService.Models
{
    public class InventoryResponse
    {
        public int OrderId { get; set; }
        public bool IsSuccess { get; set; }
    }
}
