using Microsoft.EntityFrameworkCore.Internal;
using SeguroCaixa.DTO.Response;
using SeguroCaixa.Models;
using System.Collections.Generic;
using System.Linq;

namespace BackOfficeSeguroCaixa.Filtros
{
    public static class ProcessoFiltroExtensions
    {
        public static IQueryable<Sdctb008PedidoIndenizacao> AplicaFiltro(this IQueryable<Sdctb008PedidoIndenizacao> query, ProcessoFiltro filtro)
        {

            if (filtro?.NuStatusPedido != null)
            {
                switch (filtro?.NuStatusPedido)
                {
                    case EnumStatusPedido.Disponivel:
                        query = query.Where(p => p.NuSituacaoPedidoNavigation.NuTipoSituacaoPedido == 2
                            && p.NuSituacaoPedidoNavigation.Sdctb018SituacaoMotivos.Any(m => m.NuTipoMotivoSituacao == 1));
                        break;
                    case EnumStatusPedido.EmAtendimento:
                        query = query.Where(p => new List<long> { 2, 3 }.Contains(p.NuSituacaoPedidoNavigation.NuTipoSituacaoPedido)
                          && p.NuSituacaoPedidoNavigation.Sdctb018SituacaoMotivos.Any(m => m.NuTipoMotivoSituacao == 7));
                        break;
                    case EnumStatusPedido.Pendente:
                        query = query.Where(p => new List<long> { 1, 4 }.Contains(p.NuSituacaoPedidoNavigation.NuTipoSituacaoPedido)
                          && p.NuSituacaoPedidoNavigation.Sdctb018SituacaoMotivos.Any(m => new List<long> { 4, 9 }.Contains(m.NuTipoMotivoSituacao)));
                        break;
                    case EnumStatusPedido.PericiaMedica:
                        query = query.Where(p => p.NuSituacaoPedidoNavigation.NuTipoSituacaoPedido == 3
                            && p.NuSituacaoPedidoNavigation.Sdctb018SituacaoMotivos.Any(m =>  m.NuTipoMotivoSituacao == 8));
                        break;
                    case EnumStatusPedido.Deferido:
                        query = query.Where(p => p.NuSituacaoPedidoNavigation.NuTipoSituacaoPedido == 5
                            && p.NuSituacaoPedidoNavigation.Sdctb018SituacaoMotivos.Any(m => m.NuTipoMotivoSituacao == 3));
                        break;
                    case EnumStatusPedido.CreditoEfetuado:
                        query = query.Where(p => p.NuSituacaoPedidoNavigation.NuTipoSituacaoPedido == 8
                          && p.NuSituacaoPedidoNavigation.Sdctb018SituacaoMotivos.Any(m => new List<long> { 5, 6 }.Contains(m.NuTipoMotivoSituacao)));
                        break;
                    case EnumStatusPedido.Indeferido:
                        query = query.Where(p => p.NuSituacaoPedidoNavigation.NuTipoSituacaoPedido == 6);
                        break;
                    default:
                        break;
                }
            }

            if (filtro?.NuCpfVitimaOuNuPedido != null)
            {
                query = query.Where(x => x.Sdctb015Participacaos
                    .Any(p => p.NuPessoaParticipanteNavigation.NuCpf == filtro.NuCpfVitimaOuNuPedido || p.NuPedidoIndenizacao == filtro.NuCpfVitimaOuNuPedido));
            }

            return query;
        }

    }
    public class ProcessoFiltro
    {
        public EnumStatusPedido? NuStatusPedido { get; set; }
        public decimal? NuCpfVitimaOuNuPedido { get; set; }
    }
}
