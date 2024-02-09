using AutoMapper;
using FirstApi.Models;
using FirstAPI.Data;
using FirstAPI.Data.Dtos;
using FirstAPI.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace FirstAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class CinemaController : ControllerBase
{
    private MovieAppContext _context;
    private IMapper _mapper;

    public CinemaController(MovieAppContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    [HttpPost]
    public IActionResult CreateCinema([FromBody] CreateCinemaDto cinemaDto)
    {
        Cinema cinema = _mapper.Map<Cinema>(cinemaDto);
        _context.Cinemas.Add(cinema);
        _context.SaveChanges();
        return CreatedAtAction(nameof(RetrieveCinemaByID), new { id = cinema.ID }, cinema);
    }

    [HttpGet]
    public IEnumerable<ReadCinemaDto> ListCinemas([FromQuery] int skip = 0, [FromQuery] int limit = 50)
    {
        var cinemasFromDb = _context.Cinemas.Skip(skip).Take(limit);
        var cinemasMapped = _mapper.Map<List<ReadCinemaDto>>(cinemasFromDb);
        return cinemasMapped;
    }

    [HttpGet("{id}")]
    public ReadCinemaDto RetrieveCinemaByID(int id)
    {
        var cinemaFromDb = _context.Cinemas.FirstOrDefault(cinema => cinema.ID == id);
        var cinemasMapped = _mapper.Map<ReadCinemaDto>(cinemaFromDb);
        return cinemasMapped;
    }

    [HttpPut("{id}")]
    public IActionResult UpdateCinema(int id, [FromBody] UpdateCinemaDto cinemaDto)
    {
        var cinemaFromDb = _context.Cinemas.FirstOrDefault(cinema => cinema.ID == id);
        if (cinemaFromDb == null) return NotFound();
        _mapper.Map(cinemaDto, cinemaFromDb);
        _context.SaveChanges();
        return NoContent();
    }

    [HttpPatch("{id}")]
    public IActionResult UpdateCinemaPartially(int id, JsonPatchDocument<UpdateCinemaDto> patch)
    {
        var cinemaFromDb = _context.Cinemas.FirstOrDefault(cinema => cinema.ID == id);
        if (cinemaFromDb == null) return NotFound();

        var cinemaToUpdate = _mapper.Map<UpdateCinemaDto>(cinemaFromDb);
        patch.ApplyTo(cinemaToUpdate, ModelState);

        if (!TryValidateModel(cinemaToUpdate))
        {
            return ValidationProblem(ModelState);
        }

        _mapper.Map(cinemaToUpdate, cinemaFromDb);
        _context.SaveChanges();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteCinema(int id)
    {
        Cinema? cinemaFromDb = _context.Cinemas.FirstOrDefault(cinema => cinema.ID == id);
        if (cinemaFromDb == null) return NotFound();
        _context.Remove(cinemaFromDb);
        _context.SaveChanges();
        return NoContent();
    }

}