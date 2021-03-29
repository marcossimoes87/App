using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace SeguroCaixa.Models
{
    public partial class Sdctb023DeclaraHerdeiro
    {
        public long NuDeclaracaoHerdeiro { get; set; }
        [Required]
        public long NuPedidoIndenizacao { get; set; }
        [Required]
        public short NuParentesco { get; set; }
        public long? NuPessoaHerdeiro { get; set; }
        public DateTime? DhExclusao { get; set; }

        public virtual Sdctb022Parentesco NuParentescoNavigation { get; set; }
        public virtual Sdctb008PedidoIndenizacao NuPedidoIndenizacaoNavigation { get; set; }
        public virtual Sdctb009Pessoa NuPessoaHerdeiroNavigation { get; set; }
    }
}
