using System;
using System.Collections.Generic;

namespace SeguroCaixa.DTO.Response
{
    public class DadosPessoaisBlocoResponse
    {
        public string CabecalhoBloco { get; set; }
        public long IdPessoa { get; set; }
        public short? NuNacionalidade { get; set; }
        public string NoNacionalidade { get; set; }
        public decimal Cpf { get; set; }
        public string Nome { get; set; }
        public DateTime DataNascimento { get; set; }
        public string Genero { get; set; }
        public string NomeMae { get; set; }
        public int status { get; set; }
        public string Mensagem { get; set; }
        public string CoEstado { get; set; }
        public short? NuTipoDocumento { get; set; }
        public string CoDocumento { get; set; }
        public string CoOrgaoEmissor { get; set; }
        public string CoUfOrgaoEmissor { get; set; }
        public DateTime? DtExpedicao { get; set; }
        public int TipoParticipacao { get; set; }
        public string DeTipoParticipacao { get; set; }
        public List<CamposDocumentoResponse> CamposDocumento { get; set; } 
        public List<DocumentosInfoResponse> Documentos { get; set; }
    }
}
