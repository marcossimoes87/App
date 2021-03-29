using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace SeguroCaixa.Models
{
    public partial class Sdctb005GrupoDocumento
    {
        public Sdctb005GrupoDocumento()
        {
            Sdctb003DocumentoExigidos = new HashSet<Sdctb003DocumentoExigido>();
        }

        public short NuGrupoDocumento { get; set; }
        public short NuTipoIndenizacao { get; set; }
        public short? NuTipoParticipacao { get; set; }
        [Required]
        [MaxLength(500)]
        public string DeGrupoDocumento { get; set; }
        [Required]
        [MaxLength(100)]
        public string DeAbreviaturaGrupoDocumento { get; set; }
        [Required]
        public bool IcObrigatorio { get; set; }
        [Required]
        public bool IcDocumentoUnico { get; set; }
        [Required]
        public short NuOrdem { get; set; }
        [Required]
        public DateTime DhInicio { get; set; }
        public DateTime? DhFim { get; set; }

        public virtual Sdctb007TipoIndenizacao NuTipoIndenizacaoNavigation { get; set; }
        public virtual Sdctb014TipoParticipacao NuTipoParticipacaoNavigation { get; set; }
        public virtual ICollection<Sdctb003DocumentoExigido> Sdctb003DocumentoExigidos { get; set; }
    }
}
