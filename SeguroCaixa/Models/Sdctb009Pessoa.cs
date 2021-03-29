using System;
using System.Collections.Generic;

#nullable disable

namespace SeguroCaixa.Models
{
    public partial class Sdctb009Pessoa
    {
        public Sdctb009Pessoa()
        {
            Sdctb008PedidoIndenizacaos = new HashSet<Sdctb008PedidoIndenizacao>();
            Sdctb015Participacaos = new HashSet<Sdctb015Participacao>();
            Sdctb023DeclaraHerdeiros = new HashSet<Sdctb023DeclaraHerdeiro>();
            Sdctb027Notificacaos = new HashSet<Sdctb027Notificacao>();
        }

        public long NuPessoa { get; set; }
        public decimal NuCpf { get; set; }
        public string NoPessoa { get; set; }
        public DateTime DtNascimento { get; set; }
        public string NoMae { get; set; }
        public string NoSocialPessoa { get; set; }
        public string CoGenero { get; set; }

        public virtual ICollection<Sdctb008PedidoIndenizacao> Sdctb008PedidoIndenizacaos { get; set; }
        public virtual ICollection<Sdctb015Participacao> Sdctb015Participacaos { get; set; }
        public virtual ICollection<Sdctb023DeclaraHerdeiro> Sdctb023DeclaraHerdeiros { get; set; }
        public virtual ICollection<Sdctb027Notificacao> Sdctb027Notificacaos { get; set; }
    }
}
