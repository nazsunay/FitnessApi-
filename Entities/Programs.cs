using System.ComponentModel.DataAnnotations;

namespace FitnessApi.Entities
{
    public class Programs
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public int TrainerId { get; set; }
        public Trainer Trainer { get; set; }
        public int Price { get; set; }
        public int EntryLimit { get; set; }
        public ICollection<UserProgram> UserPrograms { get; set; }
        public ICollection<ProgramHistory> ProgramHistories { get; set; }
    }
}
