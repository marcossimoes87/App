using System;
using System.Collections.Generic;

namespace SeguroCaixa.DTO.Response
{
    public class DadosPessoaisResponse
    {
        public long IdPessoa { get; set; }
        public decimal Cpf { get; set; }
        public string Nome { get; set; }
        public DateTime DataNascimento { get; set; }
        public string Genero { get; set; }
        public string NomeMae { get; set; }
        public int status { get; set; }
        public string Mensagem { get; set; }
    }
}
