using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SeguroCaixa.DTO.Response
{
    public class DadosDespesasMedicasResponse
    {
        public bool? Aprovado { get; set; }
        public long NuDocumentoCapturado { get; set; }
        public long NuPedidoIndenizacao { get; set; }
        public short NuDocumentoExigido { get; set; }
        public long? NuDocumentoPendente { get; set; }
        public short NuPagina { get; set; }
        public string DeUrlImagem { get; set; }
        public string DeCaminhoBlob { get; set; }
        public int NuGedamPedido { get; set; }
        public string DeNomeArquivo { get; set; }
        public DateTime DhInclusao { get; set; }
        public DateTime? DhExclusao { get; set; }
        public int TipoParticipacao { get; set; }
        public string DeTipoParticipacao { get; set; }
        public List<Documento> DocumentosGrupo { get; set; }
        public List<DadosEmitentesResponse> Emitentes { get; set; }

    }
}
