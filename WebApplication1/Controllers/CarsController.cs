using Microsoft.AspNetCore.Mvc;

namespace GranTurismoApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CarsController : ControllerBase
    {
        private static readonly Car[] Cars =
        [
          new Car
          {
              Brand = "Ferrari",
              Category = "Gr.3",
              Model = "458 Italia",
              Power = 570,
              Weight = 1430
          },
          new Car
          {
              Brand = "Mercedes",
              Category = "Gr.2",
              Model = "C63 AMG",
              Power = 510,
              Weight = 1730
          }
        ];

        private readonly ILogger<CarsController> _logger;

        public CarsController(ILogger<CarsController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<Car> Get()
        {
            return Cars;
        }
    }
}
