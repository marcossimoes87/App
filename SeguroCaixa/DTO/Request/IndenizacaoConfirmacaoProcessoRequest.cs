using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SeguroCaixa.DTO.Response
{
    public class IndenizacaoConfirmacaoProcessoRequest
    {
        public short? nuValorIndenizacao { get; set; }
        public short nuTipoIndenizacaoPgto { get; set; }
        public decimal vrIndenizacao { get; set; }
    }
}
