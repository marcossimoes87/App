using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace SeguroCaixa.Models
{
    public partial class Sdctb002DocumentoCapturado
    {
        public Sdctb002DocumentoCapturado()
        {
        }
        public long NuDocumentoCapturado { get; set; }
        [Required]
        public long NuPedidoIndenizacao { get; set; }
        [Required]
        public short NuDocumentoExigido { get; set; }
        public long? NuDocumentoPendente { get; set; }
        [Required]
        public short NuPagina { get; set; }
        [Required]
        [MaxLength(500)]
        public string DeUrlImagem { get; set; }
        [Required]
        [MaxLength(255)]
        public string DeCaminhoBlob { get; set; }
        [MaxLength(255)]
        public string DeNomeArquivo { get; set; }
        [Required]
        public DateTime DhInclusao { get; set; }
        public DateTime? DhExclusao { get; set; }
        public bool? IcRejeitado { get; set; }
        public virtual Sdctb003DocumentoExigido NuDocumentoExigidoNavigation { get; set; }
        public virtual Sdctb008PedidoIndenizacao NuPedidoIndenizacaoNavigation { get; set; }
        public virtual Sdctb030DocumentoPendente NuDocumentoPendenteNavigation { get; set; }

        public virtual ICollection<Sdctb049ItemDocumentoConteudo> Sdctb049ItemDocumentoConteudos { get; set; }


        public virtual ICollection<Sdctb034Emitente> Sdctb034Emitentes { get; set; }
    }
}
