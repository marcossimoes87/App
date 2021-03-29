using SeguroCaixa.DTO.Response;
using SeguroCaixa.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SeguroCaixa.DTO.Request
{
    public class SituacaoPedidoRequest
    {
        public long NuSituacaoPedido { get; set; }
        public long NuPedidoIndenizacao { get; set; }
        public short NuTipoSituacaoPedido { get; set; }
        public DateTime DhSituacao { get; set; }
        public string DeObservacao { get; set; }
        [Required]
        [StringLength(8, MinimumLength = 0, ErrorMessage = "codigo de matricula tem que ter até 8 digitos.")]
        public string CoMatriculaCaixa { get; set; }
        public DateTime? DhExclusao { get; set; }
        public short? NuMotivoIndeferimento { get; set; }
        public virtual IList<SituacaoMotivoRequest> ListaSituacaoMotivoRequest { get; set; }

        public virtual IList<DocumentoExigidoRequest> ListaDocumentoExigidoRequest { get; set; }

        public virtual IList<IndenizacaoConfirmacaoProcessoRequest> indenizacoes { get; set; }
    }
}
