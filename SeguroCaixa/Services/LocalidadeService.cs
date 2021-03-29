using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SeguroCaixa.DTO.Response;
using SeguroCaixa.Helpers;
using SeguroCaixa.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SeguroCaixa.Services
{
    public class LocalidadeService
    {
        private readonly DbLeitura _contextLeitura;
        private readonly ILogger<LocalidadeService> _logger;
        private IConfiguration _configuration;

        public LocalidadeService(ILogger<LocalidadeService> logger, DbLeitura contextLeitura, IConfiguration configuration)
        {
            _logger = logger;
            _contextLeitura = contextLeitura;
            _configuration = configuration;
        }

        public async Task<List<UfResponse>> GetUFs()
        {
            _logger.LogInformation($"GetUFs - Start -");
            try
            {
                List<UfResponse> ufs = await _contextLeitura.Sdctb013Uf
                .OrderBy(c => c.NoUf)
                .Select(s => new UfResponse() { CodigoUf = s.CoUf, NomeUf = s.NoUf })
                .ToListAsync();

                _logger.LogInformation($"GetUFs - retorno -{ufs.ToJson()}");
                return ufs;
            }
            catch (System.Exception e)
            {
                _logger.LogError($"GetUFs - erro", e.Message);
                throw;
            }
            
        }

        public async Task<List<MunicipioResponse>> GetMunicipios(string coUf)
        {
            _logger.LogInformation($"GetMunicipios - Start -");
            try
            {
                List<MunicipioResponse> municipios = await _contextLeitura
                                .Sdctb001Municipio
                                .Where(x => x.CoUf == coUf)
                                .OrderBy(o => o.DeMunicipio)
                                .Select(s => new MunicipioResponse() { CodigoMunicipio = s.NuMunicipio, NomeMunicipio = s.DeMunicipio })
                                .ToListAsync();

                _logger.LogInformation($"GetUFs - retorno -{municipios.ToJson()}");
                return municipios;
            }
            catch (System.Exception e)
            {
                _logger.LogError($"GetMunicipios - erro", e.Message);
                throw;
            }
            
        }


        public async Task<List<PaisResponse>> GetPaises()
        {
            _logger.LogInformation($"GetPaises - Start -");
            try
            {
                List<PaisResponse> paises = await _contextLeitura
                               .Sdctb020Nacionalidade
                               .OrderBy(o => o.NoNacionalidade)
                               .Select(s => new PaisResponse() { Codigo = s.NuNacionalidade, Descricao = s.NoNacionalidade })
                               .ToListAsync();

                _logger.LogInformation($"GetPaises - retorno -{paises.ToJson()}");
                return paises;
            }
            catch (System.Exception e)
            {
                _logger.LogError($"GetPaises - erro", e.Message);
                throw;
            }
           
        }
    }
}
