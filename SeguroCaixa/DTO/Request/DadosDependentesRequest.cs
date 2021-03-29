using System;

namespace SeguroCaixa.DTO.Request
{
    public class DadosDependentesRequest
    {
        public short IdParentesco { get; set; }
        public decimal? CpfParente { get; set; }
    }
}
