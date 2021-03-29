using System;

namespace SeguroCaixa.DTO.Request
{
    public class DadosAcidenteRequest
    {
        public DateTime DataHoraAcidente { get; set; }
        public string TipoLogradouro { get; set; }
        public string Logradouro { get; set; }
        public string Numero { get; set; }
        public short Municipio { get; set; }
        public string Complemento { get; set; }
        public string Bairro { get; set; }
        
    }
}
