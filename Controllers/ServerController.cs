using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace Galytix.WebApi.Controllers
{
    [ApiController]
    [Route("/api/gwp")]
    public class ServerController : ControllerBase
    {
        [AllowAnonymous]
        [HttpGet]
        [Route("ping")]
        public async Task<IActionResult> Ping()
        {
            return Ok("pong");
        }

        private const string FilePath = "Data/gwpByCountry.csv";

        private async Task<List<GwpData>> ReadCsvDataAsync()
{
    var lines = await System.IO.File.ReadAllLinesAsync(FilePath);
    var data = new List<GwpData>();

    var headers = lines[0].Split(',');
    for (int i = 1; i < lines.Length; i++)
    {
        var values = lines[i].Split(',');

        for (int j = 4; j < values.Length; j++)
        {
            if (!string.IsNullOrEmpty(values[j]) && decimal.TryParse(values[j], out decimal gwpValue))
            {
                var gwpData = new GwpData
                {
                    CountryCode = values[0],
                    LineOfBusiness = values[3],
                    Year = headers[j],
                    Gwp = gwpValue
                };
                data.Add(gwpData);
            }
            else
            {
                // logs
                System.Diagnostics.Debug.WriteLine($"Unable to parse {values[j]} at line {i}, column {j}.");
            }
        }
    }
    return data;
}

        [HttpPost("avg")]
        public async Task<ActionResult<Dictionary<string, decimal>>> GetAverageGwpByLobAsync([FromBody] GwpRequest request)
        {
            var data = await ReadCsvDataAsync();
            var filteredData = data.Where(d => d.CountryCode.ToLower() == request.Country.ToLower() && request.Lob.Contains(d.LineOfBusiness.ToLower()));
            var result = new Dictionary<string, decimal>();
            foreach (var lob in request.Lob)
            {
                result[lob] = filteredData.Where(d => d.LineOfBusiness.ToLower() == lob.ToLower()).Average(d => d.Gwp);
            }
            return result;
        }
    }

    public class GwpRequest
    {
        public string Country { get; set; }
        public List<string> Lob { get; set; }
    }

    public class GwpData
    {
        public string CountryCode { get; set; }
        public string LineOfBusiness { get; set; }
        public string Year { get; set; }
        public decimal Gwp { get; set; }
    }
}
