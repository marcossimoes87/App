using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SeguroCaixa
{
    public static class Enums
    {
        public enum TipoSituacaoPedido
        {
            PendenteDeDocumentos = 1,
            SolicitaçãoRegistrada = 2,
            PedidoEmAnalise = 3,
            Pendente = 4,
            SolicitacaoDeferida = 5,
            SolicitacaoIndeferida = 6,
            DocumentosComplementaresIncluidos = 7,
            CreditoEfetuado = 8
        }
        public enum TipoIndenizacao
        {
            DESPESASMEDICASEHOSPITALARES = 1,
            INVALIDEZPERMANENTE = 2,
            INVALIDEZMAISDESPESASMEDICASEHOSPITALARES = 3,
            INDENIZACAOPORMORTE = 4
        }

    }
}
