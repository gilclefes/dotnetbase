using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace dotnetbase.Application.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UtilsController : ControllerBase
    {

        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _config;
        private readonly string _imagePath;
        private readonly string _mapKey;
        private readonly IHttpClientFactory _clientFactory;
        public UtilsController(
IHttpClientFactory clientFactory,
           IWebHostEnvironment env,
           IConfiguration configuration)
        {

            _env = env;
            _config = configuration;
            _clientFactory = clientFactory;
            _imagePath = _config.GetValue<string>("IMAGE_STORAGE_PATH") ?? "";
            _mapKey = _config.GetValue<string>("GOOGLE_MAP_KEY") ?? "";

        }


        [HttpGet("GetImage/{fileName}")]
        public IActionResult GetImage(string fileName)
        {


            var path = Path.Combine(_env.ContentRootPath, this._imagePath, fileName);

            return PhysicalFile(path, "image/jpeg");
        }

        [HttpGet("GetMap")]
        public async Task<IActionResult> Get([FromQuery] string address)
        {
            var request = new HttpRequestMessage(HttpMethod.Get,
                $"https://maps.googleapis.com/maps/api/geocode/json?address={address}&key={_mapKey}");

            var client = _clientFactory.CreateClient();
            var response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<object>();
                return Ok(result);
            }
            else
            {
                return BadRequest(new { message = "Error in fetching data from google map." });
            }
        }
    }
}