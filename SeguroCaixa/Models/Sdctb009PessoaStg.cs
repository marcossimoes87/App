using System;
using System.Collections.Generic;

#nullable disable

namespace SeguroCaixa.Models
{
    public partial class Sdctb009PessoaStg
    {
        public long NuPessoa { get; set; }
        public decimal NuCpf { get; set; }
        public string NoPessoa { get; set; }
        public DateTime DtNascimento { get; set; }
        public string NoMae { get; set; }
        public string NoSocialPessoa { get; set; }
        public string CoGenero { get; set; }
    }
}
