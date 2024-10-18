using System.ComponentModel.DataAnnotations;

namespace FitnessApi.Entities
{
    public class ProgramHistory
    {
        [Key]
        public int Id { get; set; }  // Primary Key
        public int UserId { get; set; }
        public User User { get; set; }  // Foreign Key

        public int ProgramId { get; set; }
        public Programs Programs { get; set; }  // Foreign Key

        public DateTime EntryDate { get; set; }  // Giriş tarihi
    }
}
