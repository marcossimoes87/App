using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SeguroCaixa.DTO.Response
{
    public class ConfirmarProcessoRequest
    {
        public long NuPedidoIndenizacao { get; set; }
        public string CoMatricula { get; set; }
    }
}
