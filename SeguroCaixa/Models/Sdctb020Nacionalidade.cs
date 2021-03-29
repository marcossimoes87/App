using System;
using System.Collections.Generic;

#nullable disable

namespace SeguroCaixa.Models
{
    public partial class Sdctb020Nacionalidade
    {
        public Sdctb020Nacionalidade()
        {
            Sdctb015Participacaos = new HashSet<Sdctb015Participacao>();
        }

        public short NuNacionalidade { get; set; }
        public short NuChaveIco { get; set; }
        public string NoNacionalidade { get; set; }

        public virtual ICollection<Sdctb015Participacao> Sdctb015Participacaos { get; set; }
    }
}
