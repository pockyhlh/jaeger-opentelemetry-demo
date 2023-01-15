namespace Demo.Client.Controllers
{
    using Demo.Server.Models;
    using Microsoft.AspNetCore.Mvc;
    using System.Text.Json;

    [ApiController]
    [Route("[controller]")]
    public class WeatherController : ControllerBase
    {
        private readonly ILogger<WeatherController> logger;
        private readonly IHttpClientFactory httpClientFactory;

        public WeatherController(ILogger<WeatherController> logger, IHttpClientFactory httpClientFactory)
        {
            this.logger = logger;
            this.httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public async Task<IEnumerable<WeatherForecast>?> Get()
        {
            var httpClient = httpClientFactory.CreateClient("DemoServer");
            var httpResponseMessage = await httpClient.GetAsync("/WeatherForecast");
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                using var contentStream = await httpResponseMessage.Content.ReadAsStreamAsync();
                try
                {
                    IEnumerable<WeatherForecast>? weatherInfo = await JsonSerializer.DeserializeAsync<IEnumerable<WeatherForecast>>(contentStream);
                    return weatherInfo;
                }
                catch (Exception ex)
                {
                    logger.LogError("Error happens while getting weather information: ", ex);
                }
            }
            return null;
        }
    }
}