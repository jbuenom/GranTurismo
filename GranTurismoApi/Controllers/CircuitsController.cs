using GranTurismoApi.GranTurismoApiDbContext;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GranTurismoApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CircuitsController : ControllerBase
    {

        private readonly GranTurismoDbContext _context;
        private readonly ILogger<CircuitsController> _logger;

        public CircuitsController(ILogger<CircuitsController> logger, GranTurismoDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<Circuit>>> GetAllCircuits()
        {
            var circuits = await _context.Circuits.ToListAsync();
            return Ok(circuits);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Circuit>> GetCircuit(int id)
        {
            var circuit = await _context.Circuits.FindAsync(id);
            if (circuit is null)
            {
                return NotFound("Circuit not found");
            }
            return Ok(circuit);
        }

        [HttpPost]
        public async Task<ActionResult> CreateCircuit(Circuit circuit)
        {

            _context.Circuits.Add(circuit);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPut]
        public async Task<ActionResult> UpdateCircuit(Circuit updatedCircuit)
        {
            var dbCircuit = await _context.Circuits.FindAsync(updatedCircuit.Id);
            if (dbCircuit is null)
            {
                return NotFound("Circuit not found");
            }

            dbCircuit.Name = updatedCircuit.Name;
            dbCircuit.Distance = updatedCircuit.Distance;
            dbCircuit.Location = updatedCircuit.Location;
            dbCircuit.Record = updatedCircuit.Record;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteCircuit(int id)
        {
            var dbCircuit = await _context.Circuits.FindAsync(id);
            if (dbCircuit is null)
            {
                return NotFound("Circuit not found");
            }

            _context.Circuits.Remove(dbCircuit);

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
