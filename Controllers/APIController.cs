
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
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
            var builder = new SqlConnectionStringBuilder
            {
                DataSource = "localhost,5442",
                UserID = "postgres",
                InitialCatalog = "postgres",
                TrustServerCertificate = true,
                IntegratedSecurity = true,
                Encrypt=true
            };

            var connectionString = builder.ConnectionString;

            try
            {
                await using var connection = new SqlConnection(connectionString);

                await connection.OpenAsync();

                var sql = "SELECT name, price FROM sys.InvoiceItems";
                await using var command = new SqlCommand(sql, connection);
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
