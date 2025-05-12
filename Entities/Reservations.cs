using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace FerramentariaTest.Entities
{
    public class Reservations
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int? Id { get; set; }
        public int? IdReservationControl { get; set; }
        public int? IdLeaderMemberRel { get; set; }
        public int? IdCatalogo { get; set; }
        public int? IdFerramentaria { get; set; }
        public int? Quantidade { get; set; }
        public int? Status { get; set; }
        public string? Chave { get; set; }
        public int? IdObra { get; set; }
        public DateTime? DataRegistro { get; set; }

        [NotMapped]
        public string? StatusString
        {
            get
            {
                return Status switch
                {
                    0 => "Registrado",
                    1 => "Preparing",
                    2 => "Ready for Pickup",
                    3 => "Concluded",
                    _ => string.Empty // Default case for undefined values
                };
            }
        }
    }
}
