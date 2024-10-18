using FitnessApi.Data;
using FitnessApi.Dto;
using FitnessApi.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FitnessApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsersController(AppDbContext context)
        {
            _context = context;
        }
       
        [HttpGet]
        public ActionResult<List<User>> GetAllUser()
        {
            var users = _context.Trainers.ToList();
            return Ok(users);
        }
       
        [HttpGet("{id}")]
        public ActionResult<Trainer> GetUser(int id)
        {
            var user = _context.Trainers.Find(id);
            if (user == null)
                return NotFound("Kullanıcı bulunamadı.");

            return Ok(user);
        }

        [HttpPost("{id}")]
        public ActionResult<User> AddUpdateUser(int id, [FromBody] DtoAddUser model)
        {
            
            var data = _context.Users.Find(id);

            if (data != null)  // Kullanıcı varsa güncelle
            {
                data.Name = model.Name;
                data.Surname = model.Surname;
                data.Email = model.Email;
                _context.Update(data);
            }
            else  // yoksa kullanıcı ekle
            {
                var newUser = new User
                {
                    Name = model.Name,
                    Surname = model.Surname,
                    Email = model.Email
                };
                _context.Users.Add(newUser);
                data = newUser; 
            }

            _context.SaveChanges();
            return Ok(data);
        }


        [HttpDelete("{id}")]
        public IActionResult DeleteUser(int id)
        {
            var user = _context.Trainers.Find(id);
            if (user == null)
                return NotFound("Kullanıcı bulunamadı.");

            _context.Trainers.Remove(user);
            _context.SaveChanges();

            return Ok("Başarıyla silindi.");
        }

        //kullanıcının kaç giriş hakı kaldı?
        [HttpGet("CheckEntry/{userId}/{programId}")]
        public async Task<IActionResult> CheckEntryPermission(int userId, int programId)
        {
            var userProgram = await _context.UserPrograms
                .FirstOrDefaultAsync(up => up.UserId == userId && up.ProgramId == programId);

            if (userProgram == null || userProgram.RemainingEntries <= 0)
                return Unauthorized("Giriş hakkı yok veya program bulunamadı.");

            return Ok($"Kalan giriş hakkı: {userProgram.RemainingEntries}");
        }

        //Giriş İzni Sorgulama:// true false dönecek
        [HttpGet("CheckEntry/{userId}")]
        public async Task<ActionResult<bool>> CheckEntryPermission(int userId)
        {
            var userProgram = await _context.UserPrograms
                .Where(up => up.UserId == userId && up.RemainingEntries > 0)
                .FirstOrDefaultAsync();

            return userProgram != null;
        }
        

        //Günün tarihi ile giriş yap
        [HttpGet("entries/{date}")]
        public async Task<IActionResult> GetEntriesByDate(DateTime date)
        {
            // İlgili tarih için giriş işlemlerini alıyoruz.
            var entries = await _context.UserPrograms
                .Where(up => up.CreatedDate.Date == date.Date)
                .OrderByDescending(up => up.CreatedDate) // En son işlem en üstte
                .Select(up => new
                {
                    UserName = up.User.Name,
                    ProgramName = up.Programs.Name,
                    TrainerName = up.Programs.Trainer.Name,
                    EntryDateTime = up.CreatedDate,
                    RemainingEntries = up.RemainingEntries
                })
                .ToListAsync();

            if (!entries.Any())
            {
                return NotFound($"No entries found for the date {date.ToShortDateString()}.");
            }

            return Ok(entries);
        }


    }

}
