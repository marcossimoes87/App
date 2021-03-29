using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace SeguroCaixa.Models
{
    public partial class Sdctb026DeclaraHerdeiroComplemento
    {
        public long NuHerdeiroComplemento { get; set; }
        [Required]
        public long NuPedidoIndenizacao { get; set; }
        public bool? IcPossuiNascituro { get; set; }
        public short? QtFilhosFalecidos { get; set; }
        public short? QtIrmaosFalecidos { get; set; }
        public DateTime? DhExclusao { get; set; }

        public virtual Sdctb008PedidoIndenizacao NuPedidoIndenizacaoNavigation { get; set; }
    }
}
