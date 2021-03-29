using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SeguroCaixa.DTO.Response
{
    public class CamposDocumentoResponse
    {
        public int? NuItemDocumentoConteudo { get; set; }
        public string Titulo { get; set; }
        public string Conteudo { get; set; }
        public int? NuItemConteudo { get; set; }
    }
}
