namespace FerramentariaTest.Models
{
    public class HistoricoViewModel
    {
        public int? IdHistoricoAlocacao { get; set; }
        public int? IdProdutoAlocado { get; set; }
        public int? IdProduto { get; set; }
        public int? Solicitante_IdTerceiro { get; set; }
        public int? Solicitante_CodColigada { get; set; }
        public string? Solicitante_Chapa { get; set; }
        public int? Liberador_IdTerceiro { get; set; }
        public int? Liberador_CodColigada { get; set; }
        public string? Liberador_Chapa { get; set; }
        public int? Balconista_Emprestimo_IdLogin { get; set; }
        public string? Balconista_EmprestimoChapa { get; set; }
        public int? Balconista_Devolucao_IdLogin { get; set; }
        public string? Balconista_DevolucaoChapa { get; set; }
        public string? Observacao { get; set; }
        public DateTime? DataEmprestimo { get; set; }
        public DateTime? DataPrevistaDevolucao { get; set; }
        public DateTime? DataDevolucao { get; set; }
        public int? IdObra { get; set; }
        public int? Quantidade { get; set; }
        public int? QuantidadeEmprestada { get; set; }
        public int? IdFerrOndeProdRetirado { get; set; }
        public int? IdFerrOndeProdDevolvido { get; set; }
        public string? CodigoCatalogo { get; set; }
        public string? NomeCatalogo { get; set; }
        public string? FerrOrigem { get; set; }
        public string? FerrDevolucao { get; set; }
        public string? AFProduto { get; set; }
        public string? Serie { get; set; }
        public int? PATProduto { get; set; }
        public int? IdControleCA { get; set; }
        public int? QuantidadeExtraviada { get; set; }
        public int? UploadedFile { get; set; }
    }

    public class SearchHistoricoViewModel
    {
        public string? Codigo { get; set; }
        public string? AF { get; set; }
        public int? PAT { get; set; }
        public string? Catalogo { get; set; }
        public int? Classe { get; set; }
        public int? Ferramenta { get; set; }
        public int? EPI { get; set; }
        public int? Consumiveis { get; set; }
        public int? EmprestadoFerramenta { get; set; }
        public int? EmprestadoEPI { get; set; }
        public int? CatalogoList { get; set; }
        //public DateTime? DataDeValidade { get; set; }
        public DateTime? TransacoesDe { get; set; }
        public DateTime? TransacoesAte { get; set; }
        public DateTime? PrevisaoDe { get; set; }
        public DateTime? PrevisaoAte { get; set; }
        public string? Observacao { get; set; }
        public bool? ItensExtraviados { get; set; }
        
    }


    public class CombinedHistoricoModel
    {
        public UserViewModel? UserViewModel { get; set; } = new UserViewModel();
        public SearchHistoricoViewModel? SearchHistoricoModel { get; set; } = new SearchHistoricoViewModel();
        public List<HistoricoViewModel?>? HistoricoListModel { get; set; } = new List<HistoricoViewModel?>();
    }

}
