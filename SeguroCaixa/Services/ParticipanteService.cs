using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SeguroCaixa.DTO.Request;
using SeguroCaixa.DTO.Response;
using SeguroCaixa.Helpers;
using SeguroCaixa.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace SeguroCaixa.Services
{
    public class ParticipanteService
    {
        private readonly DbLeitura _contextLeitura;
        private readonly ILogger<ParticipanteService> _logger;
        private IConfiguration _configuration;

        public ParticipanteService(ILogger<ParticipanteService> logger, DbLeitura contextLeitura, IConfiguration configuration)
        {
            _logger = logger;
            _contextLeitura = contextLeitura;
            _configuration = configuration;
        }
        
        public async Task<List<EstadoCivilResponse>> GetListaEstadoCivil()
        {
            _logger.LogInformation($"Start GetListaEstadoCivil");
            try
            {
                List<EstadoCivilResponse> estadoCivilResponses = await _contextLeitura
                .Sdctb025EstadoCivil
                .Select(s => new EstadoCivilResponse() { Codigo = s.NuEstadoCivil, Descricao = s.NoEstadoCivil })
                .Distinct()
                .ToListAsync();

                _logger.LogInformation($"GetListaEstadoCivil: retorno: {estadoCivilResponses.ToJson()}");

                return estadoCivilResponses;
            }
            catch (Exception e)
            {
                _logger.LogError($"GetListaEstadoCivil - erro", e.Message);
                throw;
            }          
        }

        public async Task<DadosPessoaisResponse> ValidaDadosPessoais(DadosPessoaisRequest dadosPessoaisRequest)
        {
            try
            {
                _logger.LogInformation($"Start ValidaDadosPessoais - DadosPessoaisRequest {dadosPessoaisRequest.ToJson()}");

                DadosPessoaisResponse dadosPessoaisResponse = await _contextLeitura
                    .Sdctb009Pessoa
                    .Where(c => c.NuCpf == dadosPessoaisRequest.Cpf)
                    .Select(s => new DadosPessoaisResponse()
                    {
                        Cpf = s.NuCpf,
                        DataNascimento = s.DtNascimento,
                        Genero = s.CoGenero,
                        IdPessoa = s.NuPessoa,
                        Nome = s.NoPessoa,
                        NomeMae = s.NoMae,
                        status = 200
                    })
                    .FirstOrDefaultAsync();

                _logger.LogDebug($"ValidaDadosPessoais - DadosPessoaisResponse {dadosPessoaisResponse.ToJson()}");

                if (dadosPessoaisResponse == null
                    || dadosPessoaisResponse.DataNascimento != dadosPessoaisRequest.DataNascimento
                    || !(String.Compare(dadosPessoaisResponse.NomeMae?.ToUpper(), dadosPessoaisRequest.NomeMae?.ToUpper(), CultureInfo.CurrentCulture, CompareOptions.IgnoreNonSpace) == 0)
                    || !(String.Compare(dadosPessoaisResponse.Nome.ToUpper(), dadosPessoaisRequest.Nome.ToUpper(), CultureInfo.CurrentCulture, CompareOptions.IgnoreNonSpace) == 0)
                    || dadosPessoaisResponse.Genero != dadosPessoaisRequest.Genero)
                {
                    dadosPessoaisResponse = new DadosPessoaisResponse() { IdPessoa = 0, status = 400, Mensagem = "Dados divergentes da receita." };
                }

                _logger.LogInformation($"ValidaDadosPessoais - retorno: DadosPessoaisResponse {dadosPessoaisResponse.ToJson()}");
                return dadosPessoaisResponse;
            }
            catch (Exception e)
            {
                _logger.LogError($"Erro - ValidaDadosPessoais: {e.Message}");
                return null;
            }
        }

        public async Task<List<ParentescoResponse>> GetParentesco()
        {
            _logger.LogInformation($"Start GetParentesco");
            try
            {
                List<ParentescoResponse> parentescoResponses = await _contextLeitura
                    .Sdctb022Parentesco
                    .Select(s => new ParentescoResponse() { Codigo = s.NuParentesco, Descricao = s.NoParentesco })
                    .Distinct()
                    .ToListAsync();

                _logger.LogInformation($"GetParentesco - retorno: parentescoResponses {parentescoResponses.ToJson()}");
                return parentescoResponses;
            }
            catch (Exception e)
            {
                _logger.LogError($"Erro - GetParentesco: {e.Message}");
                return null;
            }
        }

        public async Task<DadosPessoaisResponse> RetornaDadosPessoais(decimal nuCpf)
        {
            _logger.LogInformation($"Start RetornaDadosPessoais - CPF {nuCpf}");
            try
            {
                DadosPessoaisResponse dadosPessoaisResponse = await _contextLeitura
                    .Sdctb009Pessoa
                    .Where(c => c.NuCpf == nuCpf)
                    .Select(s => new DadosPessoaisResponse()
                    {
                        Cpf = s.NuCpf,
                        DataNascimento = s.DtNascimento,
                        Genero = s.CoGenero,
                        IdPessoa = s.NuPessoa,
                        Nome = s.NoPessoa
                    })
                    .FirstOrDefaultAsync();

                _logger.LogInformation($"RetornaDadosPessoais - retorno: dadosPessoaisResponse {dadosPessoaisResponse.ToJson()}");
                return dadosPessoaisResponse;
            }
            catch (Exception e)
            {
                _logger.LogError($"Erro - RetornaDadosPessoais: {e.Message}");
                return null;
            }
        }

        public async Task<List<OcupacaoResponse>> GetOcupacoes(string tipoOcupacao)
        {
            _logger.LogInformation($"Start GetOcupacoes - tipoOcupacao {tipoOcupacao}");
            try
            {
                List<OcupacaoResponse> ocupacoes = await _contextLeitura
                    .Sdctb021Ocupacao
                    .Where(x => x.TpOcupacao == tipoOcupacao)
                    .Select(s => new OcupacaoResponse() { Codigo = s.NuOcupacao, Descricao = s.NoOcupacao })
                    .OrderBy(x => x.Descricao)
                    .Distinct()
                    .ToListAsync();

                _logger.LogInformation($"GetOcupacoes - retorno: ocupacoes {ocupacoes.ToJson()}");
                return ocupacoes;
            }
            catch (Exception e)
            {
                _logger.LogError($"Erro - GetOcupacoes: {e.Message}");
                return null;
            }
        }

        public async Task<ProcessoResponse> RetornaPedidoNaoIndeferidoFluxoMortePorCpfVitima(decimal CPF)
        {
            try
            {
                var pedido = await (from pi in _contextLeitura.Sdctb015Participacao
                                    join p in _contextLeitura.Sdctb009Pessoa on pi.NuPessoaParticipante equals p.NuPessoa
                                    join ind in _contextLeitura.Sdctb008PedidoIndenizacao on pi.NuPedidoIndenizacao equals ind.NuPedidoIndenizacao
                                    join sitPedido in _contextLeitura.Sdctb010SituacaoPedido on ind.NuSituacaoPedido equals sitPedido.NuSituacaoPedido
                                    where p.NuCpf == CPF
                                    where ind.NuTipoIndenizacao == 4 //igual a indenização por morte
                                    where sitPedido.NuTipoSituacaoPedido != 6 //diferente de indeferido
                                    select new ProcessoResponse
                                    {
                                        NuPedidoIndenizacao = pi.NuPedidoIndenizacao
                                    }).FirstOrDefaultAsync();

                return pedido;
            }
            catch (Exception e)
            {
                _logger.LogError($"Erro - VerificaSeExisteSolicitacaoParaVititma: {e.Message}");
                return null;
            }
        }
    }
}
