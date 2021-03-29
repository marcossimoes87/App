using BackOfficeSeguroCaixa.Filtros;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SeguroCaixa.DTO.Response;
using SeguroCaixa.Helpers;
using SeguroCaixa.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SeguroCaixa.Services
{
    public class HistoricoService
    {
        #region Inicializadores e Construtor
        private readonly DbEscrita _context;
        private readonly DbLeitura _contextLeitura;
        private readonly ILogger<HistoricoService> _logger;
        private IConfiguration _configuration;


        public HistoricoService(DbEscrita context, ILogger<HistoricoService> logger, DbLeitura contextLeitura, IConfiguration configuration)
        {
            _context = context;
            _logger = logger;
            _contextLeitura = contextLeitura;
            _configuration = configuration;
        }
        #endregion

        public async Task<HistoricoResponse> UltimoHistorico(decimal nuCpf)
        {
            _logger.LogInformation($"UltimoHistorico - inicio - CPF: {nuCpf}");

            HistoricoResponse historico = new HistoricoResponse();

            try
            {
                historico = await (from pi in _contextLeitura.Sdctb008PedidoIndenizacao
                                   join ti in _contextLeitura.Sdctb007TipoIndenizacao on pi.NuTipoIndenizacao equals ti.NuTipoIndenizacao
                                   join p in _contextLeitura.Sdctb009Pessoa on pi.NuPessoaSolicitante equals p.NuPessoa
                                   join sp in _contextLeitura.Sdctb010SituacaoPedido on pi.NuSituacaoPedido equals sp.NuSituacaoPedido
                                   join tsp in _contextLeitura.Sdctb011TipoSituacaoPedido on sp.NuTipoSituacaoPedido equals tsp.NuTipoSituacaoPedido
                                   join Sdctb019ValorIndenizacao in _contextLeitura.Sdctb019ValorIndenizacao on pi.NuPedidoIndenizacao equals Sdctb019ValorIndenizacao.NuPedidoIndenizacao into tempVI
                                   from vi in tempVI.DefaultIfEmpty()
                                   where pi.NuPessoaSolicitanteNavigation.NuCpf == nuCpf && tsp.NuTipoSituacaoPedido > 1
                                   orderby pi.DhPedido descending
                                   select new HistoricoResponse
                                   {
                                       IdPedido = pi.NuPedidoIndenizacao,
                                       Data = pi.DhPedido,
                                       Descricao = ti.DeTipoIndenizacao,
                                       TipoIndenizacao = ti.DeAbreviaturaTipoIndenizaca,
                                       Valor = vi.VrIndenizacao,
                                       Status = tsp.DeAbreviaturaSituacaoPedido
                                   }
                             ).FirstOrDefaultAsync();

            }
            catch (Exception e)
            {
                _logger.LogError($"UltimoHistorico - erro", e.Message);
                throw;
            }

            _logger.LogInformation($"UltimoHistorico - resultado: {historico.ToJson()}");
            return historico;
        }

        public async Task<List<HistoricoResponse>> ListaHistoricos(decimal nuCpf)
        {

            _logger.LogInformation($"ListaHistoricos - incio - CPF {nuCpf}");
            List<HistoricoResponse> historicos = new List<HistoricoResponse>();
            try
            {
                historicos = await (from pi in _contextLeitura.Sdctb008PedidoIndenizacao
                                    join ti in _contextLeitura.Sdctb007TipoIndenizacao on pi.NuTipoIndenizacao equals ti.NuTipoIndenizacao
                                    join p in _contextLeitura.Sdctb009Pessoa on pi.NuPessoaSolicitante equals p.NuPessoa
                                    join sp in _contextLeitura.Sdctb010SituacaoPedido on pi.NuSituacaoPedido equals sp.NuSituacaoPedido
                                    join tsp in _contextLeitura.Sdctb011TipoSituacaoPedido on sp.NuTipoSituacaoPedido equals tsp.NuTipoSituacaoPedido
                                    join Sdctb019ValorIndenizacao in _contextLeitura.Sdctb019ValorIndenizacao on pi.NuPedidoIndenizacao equals Sdctb019ValorIndenizacao.NuPedidoIndenizacao into tempVI
                                    from vi in tempVI.DefaultIfEmpty()
                                    where pi.NuPessoaSolicitanteNavigation.NuCpf == nuCpf
                                    orderby pi.DhPedido descending
                                    select new HistoricoResponse
                                    {
                                        IdPedido = pi.NuPedidoIndenizacao,
                                        Data = pi.DhPedido,
                                        Descricao = ti.DeTipoIndenizacao,
                                        TipoIndenizacao = ti.DeAbreviaturaTipoIndenizaca,
                                        Valor = vi.VrIndenizacao,
                                        Status = tsp.DeAbreviaturaSituacaoPedido,
                                        DadosPessoais = new DadosPessoaisResponse()
                                        {
                                            Cpf = p.NuCpf,
                                            DataNascimento = p.DtNascimento,
                                            Genero = p.CoGenero,
                                            IdPessoa = p.NuPessoa,
                                            Nome = p.NoPessoa,
                                            NomeMae = p.NoMae
                                        }
                                    }
                             ).ToListAsync();
            }
            catch (Exception e)
            {
                _logger.LogError("$ListaHistoricos - erro", e.Message);
                throw;
            }

            _logger.LogInformation($"ListaHistoricos - resultado: {historicos.ToJson()}");
            return historicos;
        }

        public async Task<List<HistoricoResponse>> ListaHistoricosPedido(decimal nuCpf, long nuPedido)
        {
            _logger.LogInformation($"ListaHistoricosPedido - incio - CPF {nuCpf} numero do pedido: {nuPedido}");
            List<HistoricoResponse> historicos = new List<HistoricoResponse>();

            try
            {
                historicos = await (from pi in _contextLeitura.Sdctb008PedidoIndenizacao
                                    join p in _contextLeitura.Sdctb009Pessoa on pi.NuPessoaSolicitante equals p.NuPessoa
                                    join ti in _contextLeitura.Sdctb007TipoIndenizacao on pi.NuTipoIndenizacao equals ti.NuTipoIndenizacao
                                    join sp in _contextLeitura.Sdctb010SituacaoPedido on pi.NuPedidoIndenizacao equals sp.NuPedidoIndenizacao
                                    join tsp in _contextLeitura.Sdctb011TipoSituacaoPedido on sp.NuTipoSituacaoPedido equals tsp.NuTipoSituacaoPedido
                                    join sm in _contextLeitura.Sdctb018SituacaoMotivo on sp.NuSituacaoPedido equals sm.NuSituacaoPedido
                                    join tsm in _contextLeitura.Sdctb017TipoMotivoSituacao on sm.NuTipoMotivoSituacao equals tsm.NuTipoMotivoSituacao
                                    join Sdctb019ValorIndenizacao in _contextLeitura.Sdctb019ValorIndenizacao on pi.NuPedidoIndenizacao equals Sdctb019ValorIndenizacao.NuPedidoIndenizacao into tempVI
                                    from vi in tempVI.DefaultIfEmpty()
                                    where p.NuCpf == nuCpf && pi.NuPedidoIndenizacao == nuPedido
                                    orderby sp.DhSituacao descending
                                    select new HistoricoResponse
                                    {
                                        IdPedido = pi.NuPedidoIndenizacao,
                                        Data = pi.DhPedido,
                                        Descricao = tsm.DeTipoMotivoSituacao,
                                        TipoIndenizacao = ti.DeAbreviaturaTipoIndenizaca,
                                        NuTipoIndenizacao = ti.NuTipoIndenizacao,
                                        Valor = vi.VrIndenizacao,
                                        Status = tsp.DeAbreviaturaSituacaoPedido,
                                    }
                             ).ToListAsync();
            }
            catch (Exception e)
            {
                _logger.LogError("$ListaHistoricosPedido - erro", e.Message);
                throw;
            }

            _logger.LogInformation($"ListaHistoricosPedido - resultado: {historicos.ToJson()}");
            return historicos;
        }

        public async Task<List<PedidoComHistoricoResponse>> ListaPedidoComHistorico(decimal nuCpf)
        {
            _logger.LogInformation($"ListaPedidoComHistorico - incio - CPF {nuCpf}");
            List<HistoricoResponse> historicos = new List<HistoricoResponse>();
            PedidoComHistoricoResponse PedidoComhistorico = new PedidoComHistoricoResponse();
            List<PedidoComHistoricoResponse> PedidoComhistoricoResponse = new List<PedidoComHistoricoResponse>();
            List<EnvolvidosProcessoResponse> EnvolvidosProcesso = new List<EnvolvidosProcessoResponse>();

            try
            {
                historicos = await (from pi in _contextLeitura.Sdctb008PedidoIndenizacao
                                    join ti in _contextLeitura.Sdctb007TipoIndenizacao on pi.NuTipoIndenizacao equals ti.NuTipoIndenizacao
                                    join pa in _contextLeitura.Sdctb015Participacao on pi.NuPedidoIndenizacao equals pa.NuPedidoIndenizacao
                                    join tpa in _contextLeitura.Sdctb014TipoParticipacao on pa.NuTipoParticipacao equals tpa.NuTipoParticipacao
                                    join p in _contextLeitura.Sdctb009Pessoa on pa.NuPessoaParticipante equals p.NuPessoa
                                    join sp in _contextLeitura.Sdctb010SituacaoPedido on pi.NuPedidoIndenizacao equals sp.NuPedidoIndenizacao
                                    join tsp in _contextLeitura.Sdctb011TipoSituacaoPedido on sp.NuTipoSituacaoPedido equals tsp.NuTipoSituacaoPedido
                                    join ssm in _contextLeitura.Sdctb018SituacaoMotivo on sp.NuSituacaoPedido equals ssm.NuSituacaoPedido
                                    join stm in _contextLeitura.Sdctb017TipoMotivoSituacao on ssm.NuTipoMotivoSituacao equals stm.NuTipoMotivoSituacao
                                    join Sdctb019ValorIndenizacao in _contextLeitura.Sdctb019ValorIndenizacao on pi.NuPedidoIndenizacao equals Sdctb019ValorIndenizacao.NuPedidoIndenizacao into tempVI
                                    from vi in tempVI.DefaultIfEmpty()
                                    where pa.NuPessoaParticipanteNavigation.NuCpf == nuCpf
                                    orderby sp.DhSituacao descending
                                    select new HistoricoResponse
                                    {
                                        IdPedido = pi.NuPedidoIndenizacao,
                                        IdTipoSituacao = sp.NuTipoSituacaoPedido,
                                        Data = pi.DhPedido,
                                        DHSituacao = sp.DhSituacao,
                                        TituloPedido = ti.DeAbreviaturaTipoIndenizaca,
                                        Descricao = ti.DeTipoIndenizacao,
                                        Valor = vi.VrIndenizacao,
                                        Status = tsp.DeAbreviaturaSituacaoPedido,
                                        Mensagem = stm.DeAbreviaturaMotivoSituacao,
                                        TxtImg = tsp.DeIcone,
                                        NuTipoParticipacao = tpa.NuTipoParticipacao,
                                        DeAbreviaTipoParticipacao = tpa.DeAbreviaTipoParticipacao,
                                        DadosPessoais = new DadosPessoaisResponse()
                                        {
                                            Cpf = p.NuCpf,
                                            DataNascimento = p.DtNascimento,
                                            Genero = p.CoGenero,
                                            IdPessoa = p.NuPessoa,
                                            Nome = p.NoPessoa,
                                            NomeMae = p.NoMae
                                        }

                                    }
                             ).ToListAsync();

                foreach (var h in historicos.Where(x => x.NuTipoParticipacao == 4 || x.NuTipoParticipacao ==3))
                {
                    if (!EnvolvidosProcesso.Any(x => x.IdPedido == h.IdPedido))
                    {
                        EnvolvidosProcesso.AddRange ( await (from tb15 in _contextLeitura.Sdctb015Participacao
                                                    join tb9 in _contextLeitura.Sdctb009Pessoa on tb15.NuPessoaParticipante equals tb9.NuPessoa
                                                    where tb15.NuPedidoIndenizacao == h.IdPedido
                                                    select new EnvolvidosProcessoResponse
                                                    {
                                                        Genero = tb9.CoGenero,
                                                        DataNascimento = tb9.DtNascimento,
                                                        NomeMae = tb9.NoMae,
                                                        Nome = tb9.NoPessoa,
                                                        Cpf = tb9.NuCpf,
                                                        IdPessoa = tb9.NuPessoa,
                                                        IdPedido = tb15.NuPedidoIndenizacao,
                                                        NuTipoParticipacao = tb15.NuTipoParticipacao
                                                    }).ToListAsync());
                    }
                 
                }

                foreach (var h in historicos.Where(x => x.IdTipoSituacao == 6))
                {
                    h.DeMotivoIndeferimento = await (from sp in _contextLeitura.Sdctb010SituacaoPedido
                                                     join mi in _contextLeitura.Sdctb045MotivoIndeferimentos on sp.NuMotivoIndeferimento equals mi.NuMotivoIndeferimento
                                                     where sp.NuPedidoIndenizacao == h.IdPedido
                                                     select  
                                                      mi.DeMotivoIndeferimento 
                                                     ).FirstOrDefaultAsync();
                }
                foreach (var hist in historicos.OrderByDescending(h => h.Data).GroupBy(h => h.IdPedido).ToList())
                {

                    PedidoComhistorico = (from h in hist
                                          select new PedidoComHistoricoResponse
                                          {
                                              IdPedido = h.IdPedido,
                                              IdTipoSituacao = h.IdTipoSituacao,
                                              Data = h.Data,
                                              DHSituacao = h.DHSituacao,
                                              TituloPedido = h.TituloPedido,
                                              Descricao = h.Descricao,
                                              Valor = h.Valor,
                                              Status = h.Status,
                                              Mensagem = h.Mensagem,
                                              TxtImg = h.TxtImg,
                                              NuTipoParticipacao = h.NuTipoParticipacao,
                                              DeAbreviaTipoParticipacao = h.DeAbreviaTipoParticipacao,
                                              DeMotivoIndeferimento = h.DeMotivoIndeferimento,
                                              DadosPessoais = new DadosPessoaisResponse()
                                              {
                                                  Cpf = h.DadosPessoais.Cpf,
                                                  DataNascimento = h.DadosPessoais.DataNascimento,
                                                  Genero = h.DadosPessoais.Genero,
                                                  IdPessoa = h.DadosPessoais.IdPessoa,
                                                  Nome = h.DadosPessoais.Nome,
                                                  NomeMae = h.DadosPessoais.NomeMae
                                              }
                                          }).FirstOrDefault();


                    PedidoComhistorico.EnvolvidosProcesso = (from L in EnvolvidosProcesso
                                                             where L.IdPedido == hist.FirstOrDefault().IdPedido
                                                             select new EnvolvidosProcessoResponse
                                                             {
                                                                 Cpf = L.Cpf,
                                                                 DataNascimento = L.DataNascimento,
                                                                 Genero = L.Genero,
                                                                 IdPessoa = L.IdPessoa,
                                                                 Nome = L.Nome,
                                                                 NomeMae = L.NomeMae,
                                                                 IdPedido = L.IdPedido,
                                                                 NuTipoParticipacao = L.NuTipoParticipacao
                                                             }).ToList();


                    PedidoComhistorico.Historico = (from h in hist
                                                    select new PedidoComHistoricoResponse
                                                    {
                                                        IdPedido = h.IdPedido,
                                                        IdTipoSituacao = h.IdTipoSituacao,
                                                        Data = h.Data,
                                                        DHSituacao = h.DHSituacao,
                                                        TituloPedido = h.TituloPedido,
                                                        Descricao = h.Descricao,
                                                        Valor = h.Valor,
                                                        Status = h.Status,
                                                        Mensagem = h.Mensagem,
                                                        TxtImg = h.TxtImg,
                                                        NuTipoParticipacao = h.NuTipoParticipacao,
                                                        DeAbreviaTipoParticipacao = h.DeAbreviaTipoParticipacao,
                                                        DeMotivoIndeferimento = h.DeMotivoIndeferimento,
                                                        DadosPessoais = new DadosPessoaisResponse()
                                                        {
                                                            Cpf = h.DadosPessoais.Cpf,
                                                            DataNascimento = h.DadosPessoais.DataNascimento,
                                                            Genero = h.DadosPessoais.Genero,
                                                            IdPessoa = h.DadosPessoais.IdPessoa,
                                                            Nome = h.DadosPessoais.Nome,
                                                            NomeMae = h.DadosPessoais.NomeMae
                                                        }
                                                    }).ToList();

                    PedidoComhistoricoResponse.Add(PedidoComhistorico);
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"ListaPedidoComHistorico - erro", e.Message);
                throw;
            }

            _logger.LogInformation($"ListaPedidoComHistorico - resultado: {PedidoComhistoricoResponse.ToJson()}");
            return PedidoComhistoricoResponse;
        }
    }
}