using System.Data;
using System.Data.Common;
using KOL1.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace KOL1.Services;

public class DbService : IDbService
{
    private readonly IConfiguration _configuration;

    public DbService(IConfiguration configuration)
    {
        _configuration = configuration;
    }


    public async Task<DeliverDTO> GetDelivery(int deliveryId)
    {
        var dictionary = new Dictionary<int, DeliverDTO>();

        string sql =
            "SELECT date, C.first_name AS cn, C.last_name as cl, date_of_birth, D.first_name as dn, D.last_name as dl, licence_number, name, price, amount\nFROM Delivery\nJOIN s29826.Customer C on C.customer_id = Delivery.customer_id\nJOIN s29826.Driver D on D.driver_id = Delivery.driver_id\nJOIN s29826.Product_Delivery PD on Delivery.delivery_id = PD.delivery_id\nJOIN s29826.Product P on P.product_id = PD.product_id\nWHERE Delivery.delivery_id = @deliveryId\n";

        await using SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using SqlCommand cmd = new SqlCommand(sql, conn);

        await conn.OpenAsync();

        cmd.Parameters.AddWithValue("@deliveryId", deliveryId);
        var reader = await cmd.ExecuteReaderAsync();

        DeliverDTO? deliverDto = null;

        while (await reader.ReadAsync())
        {
            string name = reader.GetString(reader.GetOrdinal("name"));
            Decimal price = reader.GetDecimal(reader.GetOrdinal("price"));
            int amount = reader.GetInt32(reader.GetOrdinal("amount"));

            if (!dictionary.TryGetValue(deliveryId, out deliverDto))
            {
                deliverDto = new DeliverDTO
                {
                    date = reader.GetDateTime(reader.GetOrdinal("date")),
                    customer = new CustomerDTO()
                    {
                        firstName = reader.GetString(reader.GetOrdinal("cn")),
                        lastName = reader.GetString(reader.GetOrdinal("cl")),
                        dateOfBirth = reader.GetDateTime(reader.GetOrdinal("date_of_birth")),
                    },
                    driver = new DriverDTO()
                    {
                        firstName = reader.GetString(reader.GetOrdinal("dn")),
                        lastName = reader.GetString(reader.GetOrdinal("dl")),
                        licenceNumber = reader.GetString(reader.GetOrdinal("licence_number")),
                    },
                    products = new List<ProductDTO>()
                };
                dictionary.Add(deliveryId, deliverDto);
            }
            deliverDto.products.Add(new ProductDTO() {name = name, price = price, amount = amount});
        }

        if (deliverDto == null)
        {
            throw new NullReferenceException("Delivery not found");
        }
        
        return deliverDto;
    }

    public async Task AddDelivery([FromBody] DeliveryDTO delivery)
    {
        string sql =
            "INSERT INTO Delivery (delivery_id, customer_id, driver_id, date)\nVALUES (@deliveryId, @customerId, @driverId, getdate())";
        string findDriverId = "SELECT driver_id\nFROM Driver\nWHERE licence_number = @licence";
        string getProductId = "SELECT product_id\nFROM Product\nWHERE name = @name";
        
        await using SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using SqlCommand cmd = new SqlCommand(sql, conn);
        
        await conn.OpenAsync();
        
        DbTransaction transaction = await conn.BeginTransactionAsync();
        cmd.Transaction = transaction as SqlTransaction;

        try
        {
            cmd.Parameters.Clear();
            cmd.CommandText = findDriverId;
            cmd.Parameters.AddWithValue("@licence", delivery.licenceNumber);
            var driverId = await cmd.ExecuteScalarAsync();
            if (driverId is null)
            {
                throw new NullReferenceException("Driver not found");
            }
            
            cmd.Parameters.Clear();
            cmd.CommandText = getProductId;
            cmd.Parameters.AddWithValue("@name", delivery.products[0].name);
            var productId = await cmd.ExecuteScalarAsync();
            if (driverId is null)
            {
                throw new NullReferenceException("Driver not found");
            }
            
            
            
            cmd.Parameters.Clear();
            cmd.CommandText = sql;
            cmd.Parameters.AddWithValue("@deliveryId", delivery.deliveryId);
            cmd.Parameters.AddWithValue("@customerId", delivery.countromerId);
            cmd.Parameters.AddWithValue("@driverId", driverId);
            
            



        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}