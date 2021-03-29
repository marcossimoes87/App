using System;
using System.Collections.Generic;

namespace SeguroCaixa.DTO.Response
{
    public class DadosDependentesResponse
    {
        public string CabecalhoBloco { get; set; }
        public List<DependentesResponse> Dependentes { get; set; }

    }
}
