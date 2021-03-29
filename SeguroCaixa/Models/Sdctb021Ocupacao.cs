using System;
using System.Collections.Generic;

#nullable disable

namespace SeguroCaixa.Models
{
    public partial class Sdctb021Ocupacao
    {
        public Sdctb021Ocupacao()
        {
            Sdctb015Participacaos = new HashSet<Sdctb015Participacao>();
        }

        public short NuOcupacao { get; set; }
        public int NuChaveCli { get; set; }
        public string NoOcupacao { get; set; }
        public string TpOcupacao { get; set; }
        public DateTime? DhExclusao { get; set; }

        public virtual ICollection<Sdctb015Participacao> Sdctb015Participacaos { get; set; }
    }
}
