using Microsoft.AspNetCore.Mvc;

namespace GranTurismoApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CircuitsController : ControllerBase
    {
        private static readonly Circuit[] Circuits =
        [
            new Circuit
            {
                Name = "Autumn Ring",
                Location = "Germany",
                Distance = 1876,
                Record = "01:21:124"
            },
            new Circuit 
            {
                Name = "Cape Ring",
                Location = "EEUU",
                Distance = 2129,
                Record = "01:56:21"
            }
        ];

        private readonly ILogger<CircuitsController> _logger;

        public CircuitsController(ILogger<CircuitsController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<Circuit> Get()
        {
            return Circuits;
        }
    }
}
