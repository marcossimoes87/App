using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SeguroCaixa.DTO.Response
{
    public class EnvolvidosProcessoResponse
    {
        public long IdPessoa { get; set; }
        public decimal Cpf { get; set; }
        public string Nome { get; set; }
        public DateTime DataNascimento { get; set; }
        public string Genero { get; set; }
        public string NomeMae { get; set; }
        public long NuTipoParticipacao { get; set; }
        public long IdPedido { get; set; }
    }
}
