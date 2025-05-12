using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FerramentariaTest.Entities
{
    public class Obra
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Id")]
        public int Id { get; set; }

        [Display(Name = "Nome")]
        public string? Nome { get; set; }

        [Display(Name = "Codigo")]
        public string? Codigo { get; set; }

        [Display(Name = "DataRegistro")]
        public DateTime DataRegistro { get; set; }

        [Display(Name = "Ativo")]
        public int Ativo { get; set; }
    }
}
