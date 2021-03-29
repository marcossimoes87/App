using System;

namespace SeguroCaixa.DTO.Response
{
    public class DadosAnaliseMeritoResponse
    {
        public string CabecalhoBloco { get; set; }
        public long? NuPedidoIndenizacao { get; set; }
        public short NuTipoIndenizacao { get; set; }
        public string DeAbreviaturaTipoIndenizaca { get; set; }
        public short NuTipoParticipacao { get; set; }
        public string DeTipoParticipacao { get; set; }
        public DateTime DhSituacao { get; set; }
        public string CoMatriculaCaixa { get; set; }
        public short NuTipoMotivoSituacao { get; set; }
        public string DeTipoMotivoSituacao { get; set; }
        public short NuTipoSituacaoPedido { get; set; }
        public long NuSituacaoPedido { get; set; }
        public string DeAbreviaturaTipoMotivoSituacao { get; set; }
        public StatusSitPedTipoMotivo StatusPedido { get; set; } = new StatusSitPedTipoMotivo();

    }
}
