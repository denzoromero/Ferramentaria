using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FerramentariaTest.Entities
{
    public class LogCancellationTransfers
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int? Id { get; set; }
        public int? IdReservation { get; set; }
        public int? ActionType { get; set; }
        public int? IdFerramentariaFrom { get; set; }
        public int? IdFerramentariaTo { get; set; }
        public string? Obs { get; set; }
        public int? Status { get; set; }
        public int? PostedBy { get; set; }
        public int? ApprovedBy { get; set; }
        public DateTime? DataRegistro { get; set; }
    }
}
