using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace SeguroCaixa.Models
{
    public partial class Sdctb027Notificacao
    {
        public long NuNotificacao { get; set; }
        [Required]
        public long NuPessoaNotificada { get; set; }
        [Required]
        [MaxLength(100)]
        public string DeMensagem { get; set; }
        public long? NuSituacaoMotivo { get; set; }
        [Required]
        public DateTime DhEnvioNotificacao { get; set; }
        public DateTime? DhCienciaNotificacao { get; set; }

        public virtual Sdctb009Pessoa NuPessoaNotificadaNavigation { get; set; }
        public virtual Sdctb018SituacaoMotivo NuSituacaoMotivoNavigation { get; set; }
    }
}
