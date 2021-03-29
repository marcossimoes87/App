using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace SeguroCaixa.Models
{
    public partial class Sdctb015Participacao
    {
        public long NuParticipacao { get; set; }
        [Required]
        public long NuPessoaParticipante { get; set; }
        [Required]
        public long NuPedidoIndenizacao { get; set; }
        [Required]
        public short NuTipoParticipacao { get; set; }
        public decimal? NuNif { get; set; }
        public short? NuNacionalidade { get; set; }
        public short? NuParentesco { get; set; }
        public short? NuEstadoCivil { get; set; }
        public short? NuTipoVeiculoProprietario { get; set; }
        public short? AaFabricacao { get; set; }
        public short? AaModelo { get; set; }
        public short? NuMunicipio { get; set; }
        [MaxLength(3)]
        public string SgTipoLogradouro { get; set; }
        public int? NuCep { get; set; }
        [MaxLength(50)]
        public string NoLogradouro { get; set; }
        [MaxLength(7)]
        public string CoPosicaoImovel { get; set; }
        [MaxLength(15)]
        public string DeComplemento { get; set; }
        [MaxLength(40)]
        public string NoBairro { get; set; }
        public short? NuDdd { get; set; }
        public decimal? NuTelefone { get; set; }
        [MaxLength(100)]
        [EmailAddress]
        public string DeEmail { get; set; }
        public short? NuOcupacao { get; set; }
        public bool? IcFormal { get; set; }
        public decimal? VrRenda { get; set; }
        public DateTime? DtRenda { get; set; }
        public decimal? VrPatrimonio { get; set; }
        public DateTime? DhFim { get; set; }
        public short? NuTipoDocumento { get; set; }
        [MaxLength(15)]
        public string CoDocumento { get; set; }
        [MaxLength(10)]
        public string CoOrgaoEmissor { get; set; }
        [MaxLength(2)]
        public string CoUfOrgaoEmissor { get; set; }
        public DateTime? DtExpedicao { get; set; }

        public virtual Sdctb013Uf CoUfOrgaoEmissorNavigation { get; set; }
        public virtual Sdctb025EstadoCivil NuEstadoCivilNavigation { get; set; }
        public virtual Sdctb001Municipio NuMunicipioNavigation { get; set; }
        public virtual Sdctb020Nacionalidade NuNacionalidadeNavigation { get; set; }
        public virtual Sdctb021Ocupacao NuOcupacaoNavigation { get; set; }
        public virtual Sdctb022Parentesco NuParentescoNavigation { get; set; }
        public virtual Sdctb008PedidoIndenizacao NuPedidoIndenizacaoNavigation { get; set; }
        public virtual Sdctb009Pessoa NuPessoaParticipanteNavigation { get; set; }
        public virtual Sdctb006TipoDocumento NuTipoDocumentoNavigation { get; set; }
        public virtual Sdctb014TipoParticipacao NuTipoParticipacaoNavigation { get; set; }
        public virtual Sdctb012TipoVeiculo NuTipoVeiculoProprietarioNavigation { get; set; }
        public virtual Sdctb004TipoLogradouro SgTipoLogradouroNavigation { get; set; }
    }
}
