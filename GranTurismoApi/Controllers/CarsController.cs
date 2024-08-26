using GranTurismoApi.GranTurismoApiDbContext;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GranTurismoApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CarsController : ControllerBase
    {
        private readonly GranTurismoDbContext _context;
        private readonly ILogger<CarsController> _logger;

        public CarsController(ILogger<CarsController> logger, GranTurismoDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<Car>>> GetAllCars()
        {
            var cars = await _context.Cars.ToListAsync();
            return Ok(cars);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Car>> GetCar(int id)
        {
            var car = await _context.Cars.FindAsync(id);
            if (car is null)
            {
                return NotFound("Car not found");
            }
            return Ok(car);
        }

        [HttpPost]
        public async Task<ActionResult> CreateCar(Car car)
        {

            _context.Cars.Add(car);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPut]
        public async Task<ActionResult> UpdateCar(Car updatedCar)
        {
            var dbCar = await _context.Cars.FindAsync(updatedCar.Id);
            if (dbCar is null)
            {
                return NotFound("Car not found");
            }

            dbCar.Brand = updatedCar.Brand;
            dbCar.Model = updatedCar.Model;
            dbCar.Power = updatedCar.Power;
            dbCar.Weight = updatedCar.Weight;
            dbCar.Category = updatedCar.Category;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteCar(int id)
        {
            var dbCar = await _context.Cars.FindAsync(id);
            if (dbCar is null)
            {
                return NotFound("Car not found");
            }

            _context.Cars.Remove(dbCar);

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
