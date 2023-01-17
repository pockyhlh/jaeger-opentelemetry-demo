namespace Demo.Client.Controllers
{
    using Demo.Server.Models;
    using Microsoft.AspNetCore.Mvc;
    using System.Diagnostics;
    using System.Text.Json;

    [ApiController]
    [Route("[controller]")]
    public class WeatherController : ControllerBase
    {
        private readonly ILogger<WeatherController> logger;
        private readonly IHttpClientFactory httpClientFactory;
        private static readonly ActivitySource Activity = new(nameof(WeatherController));

        public WeatherController(ILogger<WeatherController> logger, IHttpClientFactory httpClientFactory)
        {
            this.logger = logger;
            this.httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public async Task<IEnumerable<WeatherForecast>?> Get()
        {
            using (var activity = Activity.StartActivity("Log Records", ActivityKind.Producer))
            {
                logger.LogInformation("Get the forecast of weather");
                activity?.SetTag("Weather country", "China");
            }
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