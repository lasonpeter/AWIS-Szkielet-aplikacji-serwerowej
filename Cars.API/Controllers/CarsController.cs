using Cars.Domain;
using Cars.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cars.API.Controllers;

public class CarsController : BaseApiController
{
    private readonly DataContext _context;

    public CarsController(DataContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<List<Car>>> GetCars()
    {
        return await _context.Cars.AsNoTracking().ToListAsync();
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<Car>> GetCar(Guid id)
    {
        var car = await _context.Cars.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);

        if (car is null) return NotFound();

        return car;
    }

    [HttpPost]
    public async Task<ActionResult<Car>> CreateCar(Car car)
    {
        car.Id = Guid.NewGuid();

        _context.Cars.Add(car);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetCar), new { id = car.Id }, car);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateCar(Guid id, Car car)
    {
        if (id != car.Id) return BadRequest();

        var exists = await _context.Cars.AnyAsync(x => x.Id == id);
        if (!exists) return NotFound();

        _context.Entry(car).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteCar(Guid id)
    {
        var car = await _context.Cars.FirstOrDefaultAsync(x => x.Id == id);
        if (car is null) return NotFound();

        _context.Cars.Remove(car);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
