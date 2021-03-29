using System;
using System.Collections.Generic;

#nullable disable

namespace SeguroCaixa.Models
{
    public partial class Sdctb013Uf
    {
        public Sdctb013Uf()
        {
            Sdctb001Municipios = new HashSet<Sdctb001Municipio>();
            Sdctb015Participacaos = new HashSet<Sdctb015Participacao>();
        }

        public string CoUf { get; set; }
        public string NoUf { get; set; }

        public virtual ICollection<Sdctb001Municipio> Sdctb001Municipios { get; set; }
        public virtual ICollection<Sdctb015Participacao> Sdctb015Participacaos { get; set; }
    }
}
