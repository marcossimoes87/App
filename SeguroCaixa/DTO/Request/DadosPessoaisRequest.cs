using System;

namespace SeguroCaixa.DTO.Request
{
    public class DadosPessoaisRequest
    {
        public decimal Cpf { get; set; }
        public string Nome { get; set; }
        public DateTime DataNascimento { get; set; }
        public string Genero { get; set; }
        public string NomeMae { get; set; }
        public bool? IcVitima { get; set; }
        public bool? IcProcurador { get; set; }
        public bool? IcRepresentanteLegal { get; set; }
    }
}
