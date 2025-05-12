using System.ComponentModel.DataAnnotations;

namespace FerramentariaTest.Models
{
    public class FerramentariaViewModel
    {
        [Display(Name = "Id")]
        public int? Id { get; set; }

        [Required(ErrorMessage = "Nome is Required")]
        [Display(Name = "Nome")]
        public string? Nome { get; set; }

        [Display(Name = "DataRegistro")]
        public DateTime? DataRegistro { get; set; }

        [Display(Name = "Ativo")]
        public int? Ativo { get; set; }
    }

    public class SimpleFerramentariaViewModel
    {
        public int? Id { get; set; }
        public string? Nome { get; set; }
    }
}
