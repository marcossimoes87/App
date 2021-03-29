using System;

namespace SeguroCaixa.DTO.Response
{
    public class DadosConclusaoAnaliseResponse
    {
        public string CabecalhoBloco { get; set; }
        public long? NuValorIndenizacao { get; set; }
        public long NuPedidoIndenizacao { get; set; }
        public short? NuTipoIndenizacaoPgto { get; set; }
        public string DeTipoIndenizacaoPgto { get; set; }
        public string DeAbreviaturaTipoIndenizacaoPgto { get; set; }
        public decimal? VrIndenizacao { get; set; }
        public decimal VrDisponivelIndenizacao { get; set; }
        public DateTime? DtPrevistaCredito { get; set; }
        public DateTime? DtEfetivaCredito { get; set; }
        public DateTime? DhExclusao { get; set; }

    }
}
