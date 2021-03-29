using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SeguroCaixa.Models
{
    public class Dpvtb014PrepPessoa
    {
        public Int32? IdSinistro { get; set; }
        public string NuSinistro { get; set; }
        public Int32? IdPessoa { get; set; }
        public string DsNaturezaPessoa { get; set; }
        public string InVitima { get; set; }
        public string InBeneficiario { get; set; }
        public string InProcurador { get; set; }
        public string InRepresentanteLegal { get; set; }
        public string InCessionario { get; set; }
        public string InIntermediario { get; set; }
        public string InPessoaExcluida { get; set; }
        public DateTime? DtExclusaoPessoa { get; set; }
        public Int64? NuCnpjPessoa { get; set; }
        public string DsTipoCpfVitima { get; set; }
        public Int64? NuCpfPessoa { get; set; }
        public string NmPessoa { get; set; }
        public DateTime? DtNascimentoPessoa { get; set; }
        public string NmLogradouroPessoa { get; set; }
        public string NuLogradouroEndPessoa { get; set; }
        public string DsComplEnderecoPessoa { get; set; }
        public string NmBairroEndPessoa { get; set; }
        public string NmMunicipioEndPessoa { get; set; }
        public string SgUfEndPessoa { get; set; }
        public string NmUfEndPessoa { get; set; }
        public string CdCepEndPessoa { get; set; }
        public string NuDddTelefonePessoa { get; set; }
        public string NuTelefonePessoa { get; set; }
        public string NuDddCelularPessoa { get; set; }
        public string NuCelularPessoa { get; set; }
        public string DsEmailPessoa { get; set; }
        public string DsProfissaoInformadaPessoa { get; set; }
        public string DsProfissaoComprovadaPessoa { get; set; }
        public string DsRendaInformadaPessoa { get; set; }
        public string DsRendaComprovadaPessoa { get; set; }
        public string NmPessoaTitularCpf { get; set; }
        public string NmMaeTitularCpf { get; set; }
        public DateTime? DtNascimentoTitularCpf { get; set; }
        public DateTime? DtObitoTitularCpf { get; set; }
        public string DsSexoTitularCpf { get; set; }
        public string NmLogradouroEndTitularCpf { get; set; }
        public string NuLogradouroEndTitularCpf { get; set; }
        public string DsComplEndTitularCpf { get; set; }
        public string NmBairroEndTitularCpf { get; set; }
        public string NmMunicipioEndTitularCpf { get; set; }
        public string SgUfEndTitularCpf { get; set; }
        public string NmUfEndTitularCpf { get; set; }
        public string CdCepEndTitularCpf { get; set; }
        public string NuDddTelefoneTitularCpf { get; set; }
        public string NuTelefoneTitularCpf { get; set; }
        public string DsSituacaoTitularCpf { get; set; }
        public string InCandidatoBeneficiario { get; set; }
        public string DsTipoBeneficiario { get; set; }
        public string IdPessoaRepresentanteLegal { get; set; }
        public string NmPessoaRepresentanteLegal { get; set; }
        public DateTime? DtNascimentoRepLegal { get; set; }
        public Int64? NuCpfPessoaRepLegal { get; set; }
        public DateTime? DtProcuracao { get; set; }
        public string SgUfProcuracao { get; set; }
        public string NmUfProcuracao { get; set; }
        public float? VlEmitidoPessoa { get; set; }
        public float? VlPagoPessoa { get; set; }
        public float? VlAPagarPessoa { get; set; }
        public float? VlJurosMultaPago { get; set; }
        public string DtUltimoPagamento { get; set; }
        public string CdBancoPessoa { get; set; }
        public string NmBancoPessoa { get; set; }
        public string NuAgenciaBancariaPessoa { get; set; }
        public string NuDvAgenciaBancariaPessoa { get; set; }
        public string DsTipoContaBancariaPessoa { get; set; }
        public string NuContaBancariaPessoa { get; set; }
        public string NuDvContaBancariaPessoa { get; set; }
        public string NuDvaContaBancariaPessoa { get; set; }
        public string IdPessoaUnificada { get; set; }
        public string DtInclusaoDados { get; set; }

    }
}
