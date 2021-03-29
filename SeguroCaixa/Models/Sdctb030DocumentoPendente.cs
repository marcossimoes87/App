using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SeguroCaixa.Models
{
    public partial class Sdctb030DocumentoPendente
    {
        public long NuDocumentoPendente { get; set; }
        [Required]
        public long NuSituacaoPedido { get; set; }
        [Required]
        public short NuDocumentoExigido { get; set; }
        public DateTime? DhExclusao { get; set; }
        public virtual ICollection<Sdctb002DocumentoCapturado> NuDocumentoCapturadoNavigation { get; set; }
        public virtual Sdctb003DocumentoExigido NuDocumentoExigidoNavigation { get; set; }
        public virtual Sdctb010SituacaoPedido NuSituacaoPedidoNavigation { get; set; }
    }
}
