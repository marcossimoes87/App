using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace SeguroCaixa.Models
{
    public partial class Sdctb028TipoAcordo
    {
        public Sdctb028TipoAcordo()
        {
            Sdctb029Acordos = new HashSet<Sdctb029Acordo>();
        }

        public short NuTipoAcordo { get; set; }
        public string DeAcordo { get; set; }
        public string DeAcordoAbreviatura { get; set; }
        public DateTime? DhExclusao { get; set; }

        public virtual ICollection<Sdctb029Acordo> Sdctb029Acordos { get; set; }
    }
}
