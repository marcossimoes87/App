using System;

namespace SeguroCaixa.DTO.Request
{
    public class DadosPessoaisVitimaRequest
    {
        public long IdPessoa { get; set; }
        public string TipoLogradouro { get; set; }
        public string Logradouro { get; set; }
        public string Numero { get; set; }
        public short? Municipio { get; set; }
        public string Complemento { get; set; }
        public string Bairro { get; set; }
        public short? IdNacionalidade { get; set; }        
        public short? IdOcupacao { get; set; }
        public bool? IcFormal { get; set; }
        public decimal? Renda { get; set; }
        public DateTime? DataRenda { get; set; }
        public decimal? Patrimonio { get; set; }
        public short? IdVeiculoPatrimonio { get; set; }
        public short IdTipoParticipacao { get; set; }
        public short? AnoFabricacaoVeiculoPatrimonio { get; set; }
        public short? AnoModeloVeiculoPatrimonio { get; set; }
        public decimal? Nif { get; set; }
        public short? IdParentescoVitima { get; set; }
        public decimal? CpfVitima { get; set; }
        public short? IdEstadoCivilVitima { get; set; }
        public decimal? CpfCompanheiroVitima { get; set; }
        public bool? PossuiNascituro { get; set; }
        public short? QtdFilhosFalescidos { get; set; }
        public short? QtdIrmaosFalescidos { get; set; }
    }
}
