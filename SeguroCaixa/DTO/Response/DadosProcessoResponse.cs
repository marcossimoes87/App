using SeguroCaixa.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace SeguroCaixa.DTO.Response
{
    public class DadosProcessoResponse
    {
        public DadosAnaliseMeritoResponse DadosAnalise { get; set; }
        public DadosAcidenteResponse DadosAcidente { get; set; }
        public DadosPessoaisBlocoResponse DadosPessoais { get; set; }
        public DadosPessoaisBlocoResponse DadosPessoaisVitima { get; set; }
        public DadosComprovanteResidenciaResponse DadosResidencia { get; set; }
        public List<DocumentosInfoResponse> DocumentosAnexos { get; set; }
        public List<DadosDespesasMedicasResponse> DespesasMedicas { get; set; }
        public List<DadosConclusaoAnaliseResponse> ConclusaoAnalise { get; set; }
        public List<TipoItemDespesaResponse> TipoItemDespesa { get; set; }
        public List<TipoIndeferimentoResponse> TipoIndeferimento { get; set; }
        public List<MotivoIndeferimentoResponse> TipoMotivoIndeferimento { get; set; }
        public DadosDependentesResponse DadosDependentes { get; set; }
        public ObservacaoIndenizacaoResponse Observacao { get; set; }
        public short NuCanal { get; set; }
       
    }
}
