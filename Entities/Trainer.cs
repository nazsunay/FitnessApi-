using System.ComponentModel.DataAnnotations;

namespace FitnessApi.Entities
{
    public class Trainer
    {
        internal object _context;

        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Expertise{ get; set; }//UZMANLIK ALANI
        public ICollection<Programs> Programs { get; set; }
    }
}
