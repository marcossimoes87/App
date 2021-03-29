using SeguroCaixa.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace SeguroCaixa.DTO.Response
{
    public class BaseHistoricoResponse
    {
        public BaseHistoricoResponse()
        {
            DataPedido = DateTime.MinValue;
            DataAcidente = DateTime.MinValue;
            DataSituacao = DateTime.MinValue;
            Valor = 0;
        }
        public short NuPedido { get; set; }
        public DateTime? DataPedido { get; set; }
        public DateTime? DataAcidente { get; set; }
        public Int64? CpfVitima { get; set; }
        public Int64? CpfBeneficiario { get; set; }
        public string Descricao { get; set; }
        public string Status { get; set; }
        public DateTime? DataSituacao { get; set; }
        public string Motivo { get; set; }
        public float? Valor { get; set; }

        public string ValorFormatado
        {
            get
            {
                return Valor.HasValue ? Valor.Value.ToString("C", CultureInfo.CreateSpecificCulture("pt-BR")) : string.Empty;
            }
        }
    }

    public class ProcessoPaginacaoHistorico
    {
        private readonly int TAMANHO_PADRAO = 5;
        private int _pagina = 0;
        private int _tamanho = 0;

        public int Pagina
        {
            get
            {
                return (_pagina <= 0) ? 1 : _pagina;
            }
            set
            {
                _pagina = value;
            }
        }
        public int Tamanho
        {
            get
            {
                return (_tamanho <= 0) ? TAMANHO_PADRAO : _tamanho;
            }
            set
            {
                _tamanho = value;
            }
        }

        public int QtdeParaDescartar => Pagina > 0 ? Tamanho * (Pagina - 1) : Tamanho;
    }

    public class ProcessoPaginadoHistorico
    {
        public int Total { get; set; }
        public int TotalPaginas { get; set; }
        public int TamanhoPagina { get; set; }
        public int NumeroPagina { get; set; }
        public IList<BaseHistoricoResponse> Resultado { get; set; }
        public string Anterior { get; set; }
        public string Proximo { get; set; }

        public static ProcessoPaginadoHistorico From(ProcessoPaginacaoHistorico parametros, IQueryable<BaseHistoricoResponse> origem)
        {
            if (parametros == null)
            {
                parametros = new ProcessoPaginacaoHistorico();
            }
            int totalItens = origem.Count();
            //260 itens / 25 itens por página >> 10,4 e seu teto é 11.
            int totalPaginas = (int)Math.Ceiling(totalItens / (double)parametros.Tamanho);
            bool temPaginaAnterior = (parametros.Pagina > 1);
            bool temProximaPagina = (parametros.Pagina < totalPaginas);

            var listaTransf = origem
                .Skip(parametros.QtdeParaDescartar)
                .Take(parametros.Tamanho)
                .OrderByDescending( x => x.DataPedido)
                .ToList();

            List<BaseHistoricoResponse> listaResponse = new List<BaseHistoricoResponse>();
            foreach (var item in listaTransf)
            {
                listaResponse.Add(new BaseHistoricoResponse()
                {
                    NuPedido = item.NuPedido,
                    DataPedido = item.DataPedido,
                    DataAcidente = item.DataAcidente,
                    CpfVitima = item.CpfVitima,
                    CpfBeneficiario = item.CpfBeneficiario,
                    Descricao = item.Descricao,
                    Status = item.Status,
                    DataSituacao = item.DataSituacao,
                    Motivo = item.Motivo,
                    Valor = item.Valor
                });
            }
            return new ProcessoPaginadoHistorico
            {
                Total = totalItens,
                TotalPaginas = totalPaginas,
                TamanhoPagina = parametros.Tamanho,
                NumeroPagina = parametros.Pagina,
                Resultado = listaResponse,
                Anterior = temPaginaAnterior
                    ? $"processo?pagina={parametros.Pagina - 1}&tamanho={parametros.Tamanho}"
                    : "",
                Proximo = temProximaPagina
                    ? $"processo?pagina={parametros.Pagina + 1}&tamanho={parametros.Tamanho}"
                    : ""
            };
        }
    }
}
