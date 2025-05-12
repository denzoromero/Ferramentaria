using FerramentariaTest.Entities;

namespace FerramentariaTest.Models
{

    public class ReservationsControlModel
    {
        public int? ControlId { get; set; }
        public int? Status { get; set; }
        public DateTime? ExpiryDate { get; set; }
    }
    public class ReservationsModel
    {
        public int? ControlId { get; set; }
        public int? itemCount { get; set; }
        public int? RegisteredCount { get; set; }
        public int? PreparingCount { get; set; }
        public int? PreparedCount { get; set; }
        public string? ActualStatus { get; set; }
    }

    public class ItemReservationDetailModel
    {
        public int? IdFerramentaria { get; set; }
        public int? IdCategoria { get; set; }
        public int? IdCatalogo { get; set; }
        public int? IdReservation { get; set; }
        public int? intClasse { get; set; }
        public string? Classe { get; set; }
        public string? Type { get; set; }
        public string? Codigo { get; set; }
        public string? itemNome { get; set; }
        public int? QuantidadeResquested { get; set; }
        public string? DataRegistro { get; set; }
        public string? Status { get; set; }
        public int? intStatus { get; set; }
        public bool IsSelected { get; set; }
        public int? DataDeRetornoAutomatico { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string? ExpiryDateString { get; set; }
        public int? ferramentariacount { get; set; }
        public int? MemberCodPessoa { get; set; }
        public int? LeaderCodPessoa { get; set; }
        public int? IdObra { get; set; }
        public string? ObraName { get; set; }
        public employeeNewInformationModel? MemberInfo { get; set; } = new employeeNewInformationModel();
        public employeeNewInformationModel? LeaderInfo { get; set; } = new employeeNewInformationModel();
        public List<ControleCA>? listCA { get; set; } = new List<ControleCA>();
        public List<Ferramentaria>? listFerramentaria { get; set; } = new List<Ferramentaria>();
        public List<productDetail>? listProduto { get; set; } = new List<productDetail>();

    }

    public class employeeNewInformationModel
    {
        public int? Id { get; set; }
        public int? CodPessoa { get; set; }
        public int? IdTerceiro { get; set; }
        public byte[]? Image { get; set; }
        public string? Imagebase64 { get; set; }
        public string? Chapa { get; set; }
        public string? Nome { get; set; }
        public string? CodSituacao { get; set; }
        public DateTime? DataDemissao { get; set; }
        public DateTime? DataAdmissao { get; set; }
        public short? CodColigada { get; set; }
        public string? Funcao { get; set; }
        public string? Secao { get; set; }

        public List<MensagemSolicitanteViewModel>? blockMessage { get; set; } = new List<MensagemSolicitanteViewModel>();
        public string? blockSolicitanteMessage { get; set; }
        public string? CellphoneNo { get; set; }

    }

    public class productDetail
    {
        public int? Id { get; set; }
        public int? IdCatalogo { get; set; }
        public int? Quantidade { get; set; }
        public string? AF { get; set; }
        public int? PAT { get; set; }
        public DateTime? DataVencimento { get; set; }
        public List<ControleCA>? listCA { get; set; } = new List<ControleCA>();
        public DateTime? DataPrevistaDevolucao { get; set; }
        public bool allowedtoborrow { get; set; }
        public string? reason { get; set; }
    }


    public class reserveSubmission
    {
        public int? IdProduto { get; set; }
        public int? IdReservation { get; set; }
        public int? IdControleCA { get; set; }
        public DateTime? dataPrevista { get; set; }
        public string? observacao { get; set; }
    }

    public class cancellationSubmission
    {
        public int? IdReservation { get; set; }
        public string? obsCancel { get; set; }
    }

    public class transferSubmission
    {

        public int? IdReservation { get; set; }
        public int? IdFerramentariaOrigin { get; set; }
        public int? IdFerramentariaDestination { get; set; }
        public string? obsTransfer { get; set; }

    }

}
