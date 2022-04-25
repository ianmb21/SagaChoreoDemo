using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace InventoryService
{
    public class InventoryUpdator : IInventoryUpdator
    {
        private readonly string connectionString;

        public InventoryUpdator(string connectionString)
        {
            this.connectionString = connectionString;
        }
        public async Task Update(int productId, int quantity)
        {
            //using var connection = new SqlConnection(Environment.GetEnvironmentVariable("SqlConnectionString"));
            using var connection = new SqlConnection(connectionString);
            connection.Open();

            await connection.ExecuteAsync("spUpdateInventory", new { productId, quantity }, commandType: System.Data.CommandType.StoredProcedure);
            //await connection.ExecuteAsync("spUpdateInventory", new { productId, quantity });
        }
    }
}
