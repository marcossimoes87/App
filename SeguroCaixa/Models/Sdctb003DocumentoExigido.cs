using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace SeguroCaixa.Models
{
    public partial class Sdctb003DocumentoExigido
    {
        public Sdctb003DocumentoExigido()
        {
            Sdctb002DocumentoCapturados = new HashSet<Sdctb002DocumentoCapturado>();
            Sdctb030DocumentoPendentes = new HashSet<Sdctb030DocumentoPendente>();
        }

        public short NuDocumentoExigido { get; set; }
        [Required]
        public short NuGrupoDocumento { get; set; }
        [Required]
        public short NuTipoDocumento { get; set; }
        [Required]
        public DateTime DhInicio { get; set; }
        public DateTime? DhFim { get; set; }

        public virtual Sdctb005GrupoDocumento NuGrupoDocumentoNavigation { get; set; }
        public virtual Sdctb006TipoDocumento NuTipoDocumentoNavigation { get; set; }
        public virtual ICollection<Sdctb002DocumentoCapturado> Sdctb002DocumentoCapturados { get; set; }
        public virtual ICollection<Sdctb030DocumentoPendente> Sdctb030DocumentoPendentes { get; set; }
    }
}
