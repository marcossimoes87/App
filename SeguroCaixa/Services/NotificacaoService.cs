using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SeguroCaixa.DTO;
using SeguroCaixa.DTO.Response;
using SeguroCaixa.Helpers;
using SeguroCaixa.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SeguroCaixa.Services
{
    public class NotificacaoService
    {
        #region Inicializadores e Construtor
        private readonly DbEscrita _context;
        private readonly DbLeitura _contextLeitura;
        private readonly ILogger<NotificacaoService> _logger;

        public NotificacaoService(DbEscrita context, ILogger<NotificacaoService> logger, DbLeitura contextLeitura)
        {
            _context = context;
            _logger = logger;
            _contextLeitura = contextLeitura;
        }
        #endregion

        public async Task<List<NotificacaoResponse>> ListaNotificacao(decimal cpf)
        {
            _logger.LogInformation($"Start ListaNotificacao -  CPF: {cpf}");
            List<NotificacaoResponse> notificacaoResponses = new List<NotificacaoResponse>();
            try
            {

            notificacaoResponses = await _contextLeitura.Sdctb027Notificacao
                .Where(c => c.NuPessoaNotificadaNavigation.NuCpf == cpf && !c.DhCienciaNotificacao.HasValue)
                .Select(s => new NotificacaoResponse() { Descricao = s.DeMensagem, Data = s.DhEnvioNotificacao, Visualizado = s.DhCienciaNotificacao.HasValue ? true : false })
                .ToListAsync();
            }
            catch (Exception e)
            {
                _logger.LogError($"ListaNotificacao - erro {e.Message}");
                throw e;
            }

            _logger.LogInformation($"ListaNotificacao -  retorno: notificacaoResponses: {notificacaoResponses}");

            return notificacaoResponses;
        }

        public async Task<ResponseMessage> VisualizaNotificacoes(decimal cpf)
        {
            ResponseMessage response = new ResponseMessage();
            try
            {
                _logger.LogInformation($"Start VisualizaNotificacoes -  CPF: {cpf}");

                var lsNotificacoes = await _context
                .Sdctb027Notificacao
                .Include(s => s.NuPessoaNotificadaNavigation)
                .Where(c => c.NuPessoaNotificadaNavigation.NuPessoa == cpf && !c.DhCienciaNotificacao.HasValue)
                .ToListAsync();

                lsNotificacoes.ForEach(c =>
                {
                    c.DhCienciaNotificacao = DateTime.Now.AddHours(-3);
                });

                _logger.LogDebug($"VisualizaNotificacoes nofiticacoes: {lsNotificacoes.ToJson()}");
                _context.SaveChanges();

                response.StatusCode = 200;
                response.Mensagem = "Notificações atualizadas";
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Erro -  VisualizaNotificacoes: {cpf}");

                response.StatusCode = 500;
                response.Mensagem = "Erro de serviço";
                return response;
            }
            return response;
        }
    }
}
