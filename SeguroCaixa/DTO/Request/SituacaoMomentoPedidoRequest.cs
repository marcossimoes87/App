using SeguroCaixa.DTO.Response;
using SeguroCaixa.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SeguroCaixa.DTO.Request
{
    public class SituacaoMomentoPedidoRequest
    {
        public virtual IList<DocumentoConfirmacaoProcessoRequest> documentos { get; set; }
        public List<DadosEmitentesRequest> emitentes { get; set; }
        public CamposDocumentoRequest dadosPessoais { get; set; }
        public CamposDocumentoRequest dadosPessoaisVitima { get; set; }
        public virtual IList<IndenizacaoConfirmacaoProcessoRequest> indenizacoes { get; set; }
        public bool? necessitaPericia { get; set; }
        public short nuPedidoIndenizacao { get; set; }
        public bool IcConcluirAnalise { get; set; }
        public string coMatriculaCaixa { get; set; }
        public decimal nuCpf { get; set; }

    }
}
