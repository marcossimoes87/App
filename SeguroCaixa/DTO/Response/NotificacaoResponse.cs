using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SeguroCaixa.DTO.Response
{
    public class NotificacaoResponse
    {
        public string Descricao { get; set; }
        public bool Visualizado { get; set; }       
        public DateTime Data { get; set; }
    }
}
