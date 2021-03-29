using System.Collections.Generic;

namespace SeguroCaixa.DTO.Request
{
    public class DadosRequisicaoSeguroRequest
    {
        public long? NuPedido { get; set; }
        public short IdTipoPedido { get; set; }
        public short? IdTipoParticipacao { get; set; }
        public short IdTipoVeiculo { get; set; }
        public DadosAcidenteRequest DadosAcidente { get; set; }
        public DadosPessoaisVitimaRequest DadosPessoa { get; set; }
        public DadosPessoaisProcuradorRequest DadosProcurador { get; set; }
        public DadosPessoaisRepresentanteLegalRequest DadosRepresentanteLegal { get; set; }
        public List<DadosDependentesRequest> DadosDependentes { get; set; }
        public short? Canal { get; set; }
        public decimal? Telefone { get; set; }
        public short? Ddd { get; set; }
        public int? Cep { get; set; }
        public string Email { get; set; }
    }
}
