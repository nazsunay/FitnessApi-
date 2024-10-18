using System.ComponentModel.DataAnnotations;

namespace FitnessApi.Entities
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname {  get; set; }
        public string Email { get; set; }
        public ICollection<UserProgram> UserPrograms { get; set; }
        public ICollection<ProgramHistory> ProgramHistories { get; set; }
    }
}
