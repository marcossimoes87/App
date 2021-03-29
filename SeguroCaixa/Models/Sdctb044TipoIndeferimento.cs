using System;
using System.Collections.Generic;

#nullable disable

namespace SeguroCaixa.Models
{
    public partial class Sdctb044TipoIndeferimento
    {
        public Sdctb044TipoIndeferimento()
        {
            Sdctb045MotivoIndeferimentos = new HashSet<Sdctb045MotivoIndeferimento>();
        }

        public short NuTipoIndeferimento { get; set; }
        public string DeTipoIndeferimento { get; set; }

        public virtual ICollection<Sdctb045MotivoIndeferimento> Sdctb045MotivoIndeferimentos { get; set; }
    }
}
