using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SeguroCaixa.Models
{
    public partial class Sdctb029Acordo
    {
        public long NuAcordo { get; set; }
        [Required]
        public short NuTipoAcordo { get; set; }
        [Required]
        public long NuPedidoIndenizacao { get; set; }
        [Required]
        public DateTime DhInicio { get; set; }
        public DateTime? DhFim { get; set; }
        
        public virtual Sdctb008PedidoIndenizacao NuPedidoIndenizacaoNavigation { get; set; }
        public virtual Sdctb028TipoAcordo NuTipoAcordoNavigation { get; set; }
    }
}
