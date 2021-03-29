using System.Collections.Generic;

namespace SeguroCaixa.DTO.Response
{

    public class ListaDocumentoResponse
    {
        public int IdGrupoDocumento { get; set; }
        public string NomeGrupoDocumento { get; set; }
        public string AbreviaturaGrupoDocumento { get; set; }
        public bool Obrigatorio { get; set; }
        public bool? DocumentoUnico { get; set; }
        public List<Documento> documentos { get; set; }
        public short NuOrdem { get; set; }
    }

    

    public class Documento
    {
        public int IdGrupoDocumento { get; set; }
        public short IdDocumentoExigido { get; set; }
        public int IdTipoDocumento { get; set; }
        public string NomeTipoDocumento { get; set; }
        public string AbreviaturaTipoDocumento { get; set; }
        public short? quantidadePaginas { get; set; }
    }
}
