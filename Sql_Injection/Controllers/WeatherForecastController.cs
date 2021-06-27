using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Sql_Injection.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };
        private readonly IConfiguration _con;
        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger,IConfiguration con)
        {
            _con = con;
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpGet("getdata/{id}")]
        public IActionResult GetData(string id)
        {
            List<Authors> authors = new List<Authors>();
            Authors auth;
            string ConnectionString = _con.GetConnectionString("MyConnection");
            using(SqlConnection con=new SqlConnection(ConnectionString))
            {
                using(SqlCommand cmd=new SqlCommand())
                {
                    string SqlQuery = $"select * from Authors Where AId = {id} union select 1, table_schema,table_name,'number','email' from information_schema.tables";
                    cmd.CommandText =SqlQuery;
                    cmd.Connection = con;
                    using(SqlDataAdapter sda=new SqlDataAdapter())
                    {
                        con.Open();
                        sda.SelectCommand = cmd;
                        using(SqlDataReader sdr=cmd.ExecuteReader())
                        {
                            while(sdr.Read())
                            {
                                auth = new Authors();
                                auth.AId = Convert.ToInt32(sdr["AId"]);
                                auth.FirstName = sdr["FirstName"].ToString();
                                auth.LastName = sdr["LastName"].ToString();
                                auth.PhoneNumber = sdr["PhoneNumber"].ToString();
                                auth.Emailaddress = sdr["Emailaddress"].ToString();
                                authors.Add(auth);
                            }
                        }
                    }
                }
            }
            return Ok(authors);
        }
    }
}
