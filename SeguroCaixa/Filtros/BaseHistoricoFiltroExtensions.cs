using Microsoft.EntityFrameworkCore.Internal;
using SeguroCaixa.DTO.Response;
using SeguroCaixa.Models;
using System.Collections.Generic;
using System.Linq;

namespace BackOfficeSeguroCaixa.Filtros
{
    public static class BaseHistoricoFiltroExtensions
    {
        public static IQueryable<BaseHistoricoResponse> AplicaFiltro(this IQueryable<BaseHistoricoResponse> query, BaseHistoricoFiltro filtro)
        {

            if (filtro?.NuCpf != null)
            {
                query = query.Where(x => x.CpfVitima == filtro.NuCpf);
            }

            return query;
        }

    }
    public class BaseHistoricoFiltro
    {
        public long? NuCpf { get; set; }
    }
}
