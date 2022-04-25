using Dapper;
using OrderService.Models;
using OrderService.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace OrderService.Repositories.Classes
{
    public class OrderCreator : IOrderCreator
    {
        private readonly string connectionString;

        public OrderCreator(string connectionString)
        {
            this.connectionString = connectionString;
        }
        public async Task<int> Create(OrderDetail orderDetail)
        {
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            using var transaction = await connection.BeginTransactionAsync();
            try
            {
                var id = await connection.QuerySingleAsync<int>("spCreateOrder", new { userId = 1, userName = orderDetail.User }, transaction: transaction, commandType: System.Data.CommandType.StoredProcedure);
                await connection.ExecuteAsync("spCreateOrderDetails",
                    new { orderId = id, productId = orderDetail.ProductId, quantity = orderDetail.Quantity, productName = orderDetail.Name }, transaction: transaction, commandType: System.Data.CommandType.StoredProcedure);
                transaction.Commit();
                return id;
            }
            catch (Exception exc)
            {
                transaction.Rollback();
                return -1;
            }
        }
    }
}
