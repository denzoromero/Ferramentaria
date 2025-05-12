using X.PagedList.Mvc.Core;
using X.PagedList;
using System.ComponentModel.DataAnnotations;

namespace FerramentariaTest.Models
{
    public class ObraViewModel
    {
        [Display(Name = "Id")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Nome is Required")]
        [Display(Name = "Nome")]
        public string? Nome { get; set; }

        [Required(ErrorMessage = "Codigo is Required")]
        [Display(Name = "Codigo")]
        public string? Codigo { get; set; }

        [Display(Name = "DataRegistro")]
        public DateTime DataRegistro { get; set; }

        [Display(Name = "Ativo")]
        public int Ativo { get; set; }
    }

    public class ObraViewModelSearch
    {
        [Display(Name = "Id")]
        public int? Id { get; set; }

        [Required(ErrorMessage = "Nome is Required")]
        [Display(Name = "Nome")]
        public string? Nome { get; set; }

        [Required(ErrorMessage = "Codigo is Required")]
        [Display(Name = "Codigo")]
        public string? Codigo { get; set; }

        [Display(Name = "DataRegistro")]
        public DateTime? DataRegistro { get; set; }

        [Display(Name = "Ativo")]
        public int? Ativo { get; set; }

        public int? Status { get; set; }

        public IPagedList<ObraViewModel>? ObraPagedList { get; set; }
    }

}
