using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FerramentariaTest.Entities
{
    public class ReservationControl
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int? Id { get; set; }
        public int? IdLeaderData { get; set; }
        public string? ControlNumber { get; set; }
        public string? Chave { get; set; }
        public int? Status { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public DateTime? DataRegistro { get; set; }
    }
}
