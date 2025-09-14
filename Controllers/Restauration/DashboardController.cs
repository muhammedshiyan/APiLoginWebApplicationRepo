using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace APiLoginWebApplication.Controllers.Restauration
{
    [ApiController]
    [Route("api/restauration/[controller]")]
    public class DashboardController : ControllerBase
    {
        private readonly IConfiguration _config;

        public DashboardController(IConfiguration config)
        {
            _config = config;
        }

        private string ConnStr => _config.GetConnectionString("DefaultConnection");

        // ---------------------------
        // 📊 PIE CHART - Sales by Product
        // ---------------------------
        [HttpGet("pie")]
        public IActionResult GetPieChartData()
        {
            var labels = new List<string>();
            var values = new List<decimal>();

            using var conn = new SqlConnection(ConnStr);
            conn.Open();
            var cmd = new SqlCommand("SELECT ProductName, TotalAmount FROM vw_Dashboard_PieChart", conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                labels.Add(reader["ProductName"].ToString());
                values.Add((decimal)reader["TotalAmount"]);
            }

            return Ok(new
            {
                labels,
                datasets = new[] {
                    new {
                        data = values,
                        backgroundColor = new[] { "#FF6384", "#36A2EB", "#FFCE56" }
                    }
                }
            });
        }

        // ---------------------------
        // 📈 BAR CHART - Monthly Sales
        // ---------------------------
        [HttpGet("bar")]
        public IActionResult GetBarChartData()
        {
            var labels = new List<string>();
            var values = new List<decimal>();

            using var conn = new SqlConnection(ConnStr);
            conn.Open();
            var cmd = new SqlCommand("SELECT MonthName, TotalAmount FROM vw_Dashboard_BarChart ORDER BY FirstSaleDate", conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                labels.Add(reader["MonthName"].ToString());
                values.Add((decimal)reader["TotalAmount"]);
            }

            return Ok(new
            {
                labels,
                datasets = new[] {
                    new {
                        label = "Monthly Sales",
                        data = values,
                        backgroundColor = "#36A2EB"
                    }
                }
            });
        }

        // ---------------------------
        // 📅 GANTT CHART - Project Tasks
        // ---------------------------
        [HttpGet("gantt")]
        public IActionResult GetGanttChartData()
        {
            var tasks = new List<object>();

            using var conn = new SqlConnection(ConnStr);
            conn.Open();
            var cmd = new SqlCommand("SELECT Id, Name, Start, [End], Progress FROM vw_Dashboard_GanttChart", conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                tasks.Add(new
                {
                    id = reader["Id"].ToString(),
                    name = reader["Name"].ToString(),
                    start = ((DateTime)reader["Start"]).ToString("yyyy-MM-dd"),
                    end = ((DateTime)reader["End"]).ToString("yyyy-MM-dd"),
                    progress = (int)reader["Progress"]
                });
            }

            return Ok(tasks);
        }
    }
}
