using System;

namespace SeguroCaixa.Helpers
{
    public static class CacheTypes
    {
        public static string Beneficiario { get { return "sc_{0}_Beneficiario"; } }
        public static string Veiculo { get { return "sc_Veiculo"; } }
        public static string TipoIndenizacao { get { return "sc_TipoIndenizacao"; } }
        public static string UF { get { return "sc_UF"; } }
        public static string Municipio { get { return "sc_{0}_Municipio"; } }
        public static object CancellationToken { get { return "sc_cancellationToken"; } }
        public static string EstadoCivil { get { return "sc_EstadoCivil"; } }
        public static string Parentesco { get { return "sc_Parentesco"; } }
        public static string Nacionalidade { get { return "sc_Nacionalidade"; } }
        public static string Ocupacao { get { return "sc_{0}_Ocupacao"; } }
        public static string DocumentoExigido { get { return "sc_{0}_{1}_DocsExigidos"; } }
    }
}
