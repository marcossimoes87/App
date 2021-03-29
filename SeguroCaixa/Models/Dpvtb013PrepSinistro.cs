using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SeguroCaixa.Models
{
    public class Dpvtb013PrepSinistro
    {
        public Int32? IdSinistro { get; set; }
        public string CdAvisoSinistro { get; set; }
        public DateTime? DtCriacaoAvisoSinistro { get; set; }
        public DateTime? DtCriacaoNumeroSinistro { get; set; }
        public string NuSinistro { get; set; }
        public Int32? NuSequenciaSinistro { get; set; }
        public string DsNaturezaSinistro { get; set; }
        public string DsTipoInvalidez { get; set; }
        public string SgUfOcorrenciaSinistro { get; set; }
        public string NmMuniOcorrenciaSinistro { get; set; }
        public string DsDelegaciaOcorrencia { get; set; }
        public DateTime? DtOcorrenciaSinistro { get; set; }
        public DateTime? DtReclamacaoSinistro { get; set; }

        public string SgUfDelegaciaOcorrencia { get; set; }
        public string NuBoletimOcorrencia { get; set; }
        public DateTime? DtBoletimOcorrencia { get; set; }

        public string NuLaudoIml { get; set; }
        public string SgUfObitoVitima { get; set; }
        public DateTime? DtObitoVitima { get; set; }
        public float? VlPleiteado { get; set; }
        public string NmMedicoPrimeiroAtendimento { get; set; }
        public string NuCrmMedicoPrimAtend { get; set; }
        public string SgUfCrmMedicoPrimAtend { get; set; }
        public string DsRazaoNegativa { get; set; }
        public string DsMotivoNegativa { get; set; }
        public string DsMotivoBloqueio { get; set; }
        public string NmVitima { get; set; }
        public DateTime? DtNascimentoVitima { get; set; }
        public Int64? NuCpfVitima { get; set; }
        public string NmTitularCpf { get; set; }
        public string NmMaeTitularCpf { get; set; }
        public string NmUltimoPortador { get; set; }
        public Int64? NuCpfUltimoPortador { get; set; }
        public string DsTipoCpfVitima { get; set; }
        public string DsEmailVitima { get; set; }
        public string SgUfVitima { get; set; }
        public string DsSexoVitima { get; set; }
        public string DsTipoSinistrado { get; set; }
        public string DsTipoDocVeiculo { get; set; }
        public string SgUfPlacaVeiculo { get; set; }
        public string NuPlacaVeiculo { get; set; }
        public string CdCategoriaVeiculo { get; set; }
        public string DsCategoriaVeiculo { get; set; }
        public string NmEmpresaDigitalizadora { get; set; }
        public string NmSituacaoAtualSinistro { get; set; }
        public DateTime? DtUltimoMovimentoSinistro { get; set; }
        public DateTime? DtUltimoDocumento { get; set; }
        public string NmAcaoSinistro { get; set; }
        public string NmSituacaoPosterior { get; set; }
        public DateTime? DtInclusaoDado { get; set; }

    }
}
