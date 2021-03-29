using System;

namespace SeguroCaixa.DTO.Response
{
    public class DadosAcidenteResponse
    {
        public string CabecalhoBloco { get; set; }
        public DateTime DhSinistro { get; set; }
        public short NuTipoVeiculo { get; set; }
        public string DeAbreviaturaTipoVeiculo { get; set; }
        public string TipoLogradouro { get; set; }
        public string NoLogradouro { get; set; }
        public string CoPosicaoImovel { get; set; }
        public string DeMunicipio { get; set; }
        public string Complemento { get; set; }
        public string NoBairro { get; set; }
        public string CoEstado { get; set; }
        public string DtSinistro { get; set; }
        public string HrSinistro { get; set; }

    }
}
