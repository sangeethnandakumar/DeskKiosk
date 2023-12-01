using DeskKiosk.Server.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace DeskKiosk.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly SignalRHub hub;

        public WeatherForecastController(SignalRHub hub)
        {
            this.hub = hub;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            await hub.SendNotificationAsync(new BridgeCommand
            {
                Command = "loadState",
                Params = true
            });
            return Ok();
        }
    }
}
