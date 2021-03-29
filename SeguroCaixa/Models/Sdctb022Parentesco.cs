using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace SeguroCaixa.Models
{
    public partial class Sdctb022Parentesco
    {
        public Sdctb022Parentesco()
        {
            Sdctb015Participacaos = new HashSet<Sdctb015Participacao>();
            Sdctb023DeclaraHerdeiros = new HashSet<Sdctb023DeclaraHerdeiro>();
        }

        public short NuParentesco { get; set; }
        [Required]
        [MaxLength(100)]
        public string NoParentesco { get; set; }

        public virtual ICollection<Sdctb015Participacao> Sdctb015Participacaos { get; set; }
        public virtual ICollection<Sdctb023DeclaraHerdeiro> Sdctb023DeclaraHerdeiros { get; set; }
    }
}
