using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SeguroCaixa.DTO.Request
{
    public class SituacaoMotivoRequest
    {
        public long NuSituacaoMotivo { get; set; }
        public long NuSituacaoPedido { get; set; }
        public short NuTipoMotivoSituacao { get; set; }
        public DateTime DhSituacao { get; set; }
        public DateTime? DhExclusao { get; set; }
    }
}
