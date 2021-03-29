using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SeguroCaixa.DTO.Response
{
    public class MensagemProcessoResponse
    {
        public int CodigoRetorno { get; set; }
        public MensagemRetornoResponse MensagemRetorno { get; set; }

    }
    public class MensagemRetornoResponse
    {
        public string NumeroProcesso { get; set; }
        public string Mensagem { get; set; }
    }
}
