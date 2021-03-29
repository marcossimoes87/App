﻿using System;
using System.Collections.Generic;
using System.Globalization;

namespace SeguroCaixa.DTO.Response
{
    public class PedidoComHistoricoResponse
    {
        public PedidoComHistoricoResponse()
        {
            Data = DateTime.MinValue;
            Valor = 0;
        }

        public long IdPedido { get; set; }
        public long IdTipoSituacao { get; set; }
        public string TituloPedido { get; set; }
        public string Descricao { get; set; } 
        public string Status { get; set; }
        public decimal? Valor { get; set; }
        public DateTime Data { get; set; }
        public DateTime DHSituacao { get; set; }
        public string Mensagem { get; set; }
        public string TxtImg { get; set; }
        public long NuTipoParticipacao { get; set; }
        public string DeAbreviaTipoParticipacao { get; set; }
        public string DeMotivoIndeferimento { get; set; }

        public List<PedidoComHistoricoResponse> Historico;
        public DadosPessoaisResponse DadosPessoais { get; set; }
        public List<EnvolvidosProcessoResponse> EnvolvidosProcesso { get; set; }
        public string DataFormatada
        {
            get
            {
                return $"{Data.Month.ToString().PadLeft(2,'0')}/{Data.Year}";
            }
        }

        public int DiferencaData
        {
            get
            {
                var diferenca = DateTime.Now.AddHours(-3).Date - Data.Date;
                return diferenca.Days / 365;
            }
        }

        public string ValorFormatado
        {
            get
            {
                return Valor.HasValue ? Valor.Value.ToString("C", CultureInfo.CreateSpecificCulture("pt-BR")) : string.Empty;
            }
        }
    }
}