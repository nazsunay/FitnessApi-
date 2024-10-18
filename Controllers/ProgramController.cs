using FitnessApi.Data;
using FitnessApi.Dto;
using FitnessApi.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace FitnessApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProgramsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProgramsController(AppDbContext context)
        {
            _context = context;
        }
      
        [HttpGet("{id}")]
        public IActionResult GetProgramById(int id)
        {
            var program = _context.Programs
                .Include(p => p.UserPrograms)
                .Include(p => p.ProgramHistories)
                .Select(t => new { t.Id, t.Name, t.TrainerId, t.Trainer, t.Price, t.EntryLimit, t.UserPrograms, t.ProgramHistories })
                .FirstOrDefault(p => p.Id == id);

            if (program == null)
            {
                return NotFound();
            }

            return Ok(program);
        }

        [HttpPost]
        public IActionResult CreateProgram([FromBody] DtoAddProgram programDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Veritabanı için Program nesnesi oluşturma
            var program = new Programs
            {
                Name = programDto.Name,
                TrainerId = programDto.TrainerId,
                Price = programDto.Price,
                EntryLimit = programDto.EntryLimit
            };

            _context.Programs.Add(program);
            _context.SaveChanges();

            // Başarıyla oluşturulan programın ID’siyle dönüyoruz.
            return CreatedAtAction(nameof(GetProgramById), new { id = program.Id }, program);
        }

        
        [HttpPut("{id}")]
        public IActionResult UpdateProgram(int id, [FromBody] DtoAddProgram programDto)
        {
            
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            
            var program = _context.Programs.Find(id);
            if (program == null)
            {
                return NotFound($"!!! {id} not found.");
            }

           
            program.Name = programDto.Name;
            program.TrainerId = programDto.TrainerId;
            program.Price = programDto.Price;
            program.EntryLimit = programDto.EntryLimit;

            _context.Programs.Update(program);
            _context.SaveChanges();

            
            return Ok(program);
        }


        
        [HttpDelete("{id}")]
        public IActionResult DeleteProgram(int id)
        {
            var program = _context.Programs.Find(id);
            if (program == null)
                return NotFound();

            _context.Programs.Remove(program);
            _context.SaveChanges();
            return NoContent();
        }

        
        [HttpGet("AllPrograms")]
        public IActionResult GetAllPrograms(int? trainerId, int? programId, DateTime? minDate, DateTime? maxDate)
        {
            var query = _context.Programs.AsQueryable();

            if (trainerId.HasValue)
                query = query.Where(p => p.TrainerId == trainerId);

            if (programId.HasValue)
                query = query.Where(p => p.Id == programId);

            if (minDate.HasValue && maxDate.HasValue)
                query = query.Where(p => p.ProgramHistories
                    .Any(ph => ph.EntryDate >= minDate && ph.EntryDate <= maxDate));

            var result = query
                .Select(p => new
                {
                    TrainerName = $"{p.Trainer.Name} {p.Trainer.Surname}",
                    ProgramName = p.Name,
                    TotalEarnings = p.ProgramHistories.Count * p.Price,
                    StudentCount = p.UserPrograms.Count
                })
                .ToList();

            return Ok(result);
        }

        //!!
        [HttpGet("ProgramDetails/{programId}")]
        public IActionResult GetProgramDetails(int programId)
        {
            var program = _context.Programs
                .Include(p => p.Trainer)
                .Include(p => p.UserPrograms)
                .ThenInclude(up => up.User)
                .FirstOrDefault(p => p.Id == programId);

            if (program == null)
                return NotFound();

            var monthlyEarnings = _context.ProgramHistories
                .Where(ph => ph.ProgramId == programId)
                .GroupBy(ph => new { ph.EntryDate.Year, ph.EntryDate.Month })
                .Select(g => new
                {
                    Month = $"{g.Key.Year} {CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(g.Key.Month)}",
                    TotalEarnings = g.Count() * program.Price
                })
                .ToList();

            var registeredUsers = program.UserPrograms.Select(up => up.User.Name).ToList();

            return Ok(new
            {
                ProgramName = program.Name,
                TrainerName = $"{program.Trainer.Name} {program.Trainer.Surname}",
                Earnings = monthlyEarnings,
                RegisteredUsers = registeredUsers
            });
        }



    }


}
