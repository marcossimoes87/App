using System;
using System.Collections.Generic;

#nullable disable

namespace SeguroCaixa.Models
{
    public partial class Sdctb025EstadoCivil
    {
        public Sdctb025EstadoCivil()
        {
            Sdctb015Participacaos = new HashSet<Sdctb015Participacao>();
        }

        public short NuEstadoCivil { get; set; }
        public string NoEstadoCivil { get; set; }

        public virtual ICollection<Sdctb015Participacao> Sdctb015Participacaos { get; set; }
    }
}
