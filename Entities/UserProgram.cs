using System.ComponentModel.DataAnnotations;

namespace FitnessApi.Entities
{
    public class UserProgram
    {
        [Key]
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }  // Foreign Key
        public int ProgramId { get; set; }
        public Programs Programs { get; set; }  // Foreign Key
        public DateTime CreatedDate { get; set; }
        public  int RemainingEntries {  get; set; }// kalan giriş hakkı
    }
}
