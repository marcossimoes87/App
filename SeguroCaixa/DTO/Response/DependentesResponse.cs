using System;

namespace SeguroCaixa.DTO.Response
{
    public class DependentesResponse
    {
        public decimal NuCpf { get; set; }
        public string NoPessoa { get; set; }
        public short NuParentesco { get; set; }
        public string NoParentesco { get; set; }
        public long NuDeclaracaoHerdeiro { get; set; }
        public long? NuPessoaHerdeiro { get; set; }
    }
}
