
using Microsoft.AspNetCore.Mvc;
using Npgsql;
class Item {
    public string name;
    public int price;
}

namespace BuggyApp.Controllers
{
    [ApiController]
    [Route("api/Invoice")]
    public class DataController : ControllerBase
    {
        [HttpGet("async")]
        public async Task<IActionResult> GetData()
        {
            List<Item> items = null;
            var builder = new NpgsqlConnectionStringBuilder
            {
                Host = "localhost",
                Port = 5442,
                Username = "postgres",
                Database = "postgres",
                SslMode = SslMode.Require,
                TrustServerCertificate = true
            };

            var connectionString = builder.ConnectionString;

            try
            {
                await using var connection = new NpgsqlConnection(connectionString);

                await connection.OpenAsync();

                var sql = "SELECT name, price FROM sys.InvoiceItems";
                await using var command = new NpgsqlCommand(sql, connection);
                await using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    Item item = new Item {name = reader.GetString(0), price = int.Parse(reader.GetString(1))};
                    items.Add(item);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return Ok(new {items});
            
        }
        
    }

}
