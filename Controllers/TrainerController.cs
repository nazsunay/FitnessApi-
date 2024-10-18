using FitnessApi.Data;
using FitnessApi.Dto;
using FitnessApi.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace FitnessApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TrainerController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TrainerController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public ActionResult<List<Trainer>> GetAllTrainers()
        {
            var trainers = _context.Trainers.ToList();
            return Ok(trainers);
        }

        [HttpGet("{id}")]
        public ActionResult<Trainer> GetTrainer(int id)
        {
            var trainer = _context.Trainers.Find(id);
            if (trainer == null)
                return NotFound("Eğitmen bulunamadı.");

            return Ok(trainer);
        }

        [HttpPost("{id}")]
        public ActionResult<Trainer> UpdateTrainer(int id, [FromBody] DtoAddTrainer model)
        {
            var data = new Trainer();
            data.Name = model.Name;
            data.Surname = model.Surname;
            data.Expertise = model.Expertise;

            if (model.Id is not 0)
            {
                data = _context.Trainers.Find(model.Id);
                data.Name = model.Name;
                data.Surname = model.Surname;
                data.Expertise = model.Expertise;
                _context.Update(data);

            }
            else
            {
                data.Name = model.Name;
                data.Surname = model.Surname;
                data.Expertise = model.Expertise;
                _context.Add(data);
            }

            _context.SaveChanges();
            return data; ;
        }

        
        [HttpDelete("{id}")]
        public IActionResult DeleteTrainer(int id)
        {
            var trainer = _context.Trainers.Find(id);
            if (trainer == null)
                return NotFound("Eğitmen bulunamadı.");

            _context.Trainers.Remove(trainer);
            _context.SaveChanges();

            return Ok("Başarıyla silindi.");
        }


        [HttpPost("trainer/{trainerId}")]
        public IActionResult GetTrainerProgramsWithStudents(int trainerId)
        {
            var trainerWithPrograms = _context.Trainers
                .Where(t => t.Id == trainerId)
                .Select(t => new
                {
                    TrainerName = t.Name,
                    TotalPrograms = t.Programs.Count,
                    ProgramsList = t.Programs.Select(p => new
                    {
                        ProgramName = p.Name,
                        StudentCount = p.UserPrograms.Count // Her program için öğrenci sayısı
                    }).ToList()
                })
                .FirstOrDefault(); // Senkron sorgu

            if (trainerWithPrograms == null)
            {
                return NotFound($" {trainerId} not found.");
            }

            return Ok(trainerWithPrograms);
        }


    }
}
