using System;
using System.Collections.Generic;

#nullable disable

namespace SeguroCaixa.Models
{
    public partial class Sdctb006TipoDocumento
    {
        public Sdctb006TipoDocumento()
        {
            Sdctb003DocumentoExigidos = new HashSet<Sdctb003DocumentoExigido>();
            Sdctb015Participacaos = new HashSet<Sdctb015Participacao>();
        }

        public short NuTipoDocumento { get; set; }
        public string DeTipoDocumento { get; set; }
        public string DeAbreviaturaTipoDocumento { get; set; }
        public short? QtPaginas { get; set; }
        public DateTime? DhExclusao { get; set; }

        public virtual ICollection<Sdctb003DocumentoExigido> Sdctb003DocumentoExigidos { get; set; }
        public virtual ICollection<Sdctb015Participacao> Sdctb015Participacaos { get; set; }
    }
}
