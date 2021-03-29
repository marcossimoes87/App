using System;
using System.Collections.Generic;

namespace SeguroCaixa.DTO.Response
{
    public class DadosComprovanteResidenciaResponse
    {
        public string CabecalhoBloco { get; set; }
        public short? IdMunicipio { get; set; }
        public string DeMunicipio { get; set; }
        public string DeBairro { get; set; }
        public string DeLogradouro { get; set; }
        public string DeNumero { get; set; }
        public int? DeCep { get; set; }
        public string CoEstado { get; set; }
        public int TipoParticipacao { get; set; }
        public string DeTipoParticipacao { get; set; }
        public List<DocumentosInfoResponse> Documentos { get; set; }

    }
}
