using System;
using System.Collections.Generic;

#nullable disable

namespace SeguroCaixa.Models
{
    public partial class Sdctb047TipoItemDocumento
    {
        public Sdctb047TipoItemDocumento()
        {
            Sdctb048ItemDocumentoExigidos = new HashSet<Sdctb048ItemDocumentoExigido>();
            Sdctb049ItemDocumentoConteudos = new HashSet<Sdctb049ItemDocumentoConteudo>();
        }

        public int NuItemDocumento { get; set; }
        public string DeItemDocumento { get; set; }
        public string DeItemDocumentoTitulo { get; set; }
        public bool IcNumerico { get; set; }
        public short NuTamanho { get; set; }
        public short NuOrdem { get; set; }

        public virtual ICollection<Sdctb048ItemDocumentoExigido> Sdctb048ItemDocumentoExigidos { get; set; }
        public virtual ICollection<Sdctb049ItemDocumentoConteudo> Sdctb049ItemDocumentoConteudos { get; set; }
    }
}
