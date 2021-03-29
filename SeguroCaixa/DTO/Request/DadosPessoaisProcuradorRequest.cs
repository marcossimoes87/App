using System;

namespace SeguroCaixa.DTO.Request
{
    public class DadosPessoaisProcuradorRequest
    {
        public decimal? Cpf { get; set; }
        public string Nome { get; set; }
        public DateTime? DataNascimento { get; set; }
        public string Genero { get; set; }
        public string NomeMae { get; set; }
        public short? IdTipoParticipacao { get; set; }

    }
}
