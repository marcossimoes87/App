using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SeguroCaixa.DTO.Response
{
    public class TipoPedidoResponse
    {
        public short Id { get; set; }
        public string Titulo { get; set; }
        public string Descricao { get; set; }
    }
}
