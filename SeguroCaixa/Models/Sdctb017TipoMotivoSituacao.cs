using System;
using System.Collections.Generic;

#nullable disable

namespace SeguroCaixa.Models
{
    public partial class Sdctb017TipoMotivoSituacao
    {
        public Sdctb017TipoMotivoSituacao()
        {
            Sdctb018SituacaoMotivos = new HashSet<Sdctb018SituacaoMotivo>();
        }

        public short NuTipoMotivoSituacao { get; set; }
        public string DeTipoMotivoSituacao { get; set; }
        public string DeAbreviaturaMotivoSituacao { get; set; }
        public bool IcExibe { get; set; }
        public DateTime? DhExclusao { get; set; }

        public virtual ICollection<Sdctb018SituacaoMotivo> Sdctb018SituacaoMotivos { get; set; }
    }
}
