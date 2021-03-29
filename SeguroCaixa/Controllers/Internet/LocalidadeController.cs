using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SeguroCaixa.DTO;
using SeguroCaixa.DTO.Response;
using SeguroCaixa.Helpers;
using SeguroCaixa.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SeguroCaixa.Controllers
{
    [Authorize(Policy = "Nivel12")]
    [ApiController]
    [Route("api/[controller]")]
    public class LocalidadeController : BaseController
    {
        private readonly ILogger<LocalidadeController> _logger;
        LocalidadeService _localidadeService;

        public LocalidadeController(LocalidadeService localidadeService, ILogger<LocalidadeController> logger, IMemoryCache cache, IConfiguration configuration) : base(cache, configuration)
        {
            _localidadeService = localidadeService;
            _logger = logger;
        }

        [HttpGet]
        [Route("v1/uf")]
        public async Task<ActionResult<List<UfResponse>>> GetUFs()
        {
            try
            {
                List<UfResponse> listaUFs = GetCache<List<UfResponse>>(CacheTypes.UF);

                if (listaUFs == null)
                {
                    listaUFs = await _localidadeService.GetUFs();
                    SetCache(CacheTypes.UF, listaUFs);
                }

                return StatusCode(200, listaUFs);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Erro -  GetUFs: {GetCache<List<UfResponse>>(CacheTypes.UF)}");
                return StatusCode(500, "Erro de serviço");
            }
        }

        [HttpGet]
        [Route("v1/municipio/{coUf}")]
        public async Task<ActionResult<MunicipioResponse>> ListaMunicipio(string coUf)
        {
            try
            {
                if(coUf?.Length < 2)
                {
                    return new NoContentResult();
                }               

                string cacheKey = string.Format(CacheTypes.Municipio, coUf);

                List<MunicipioResponse> municipios = GetCache<List<MunicipioResponse>>(cacheKey);

                if (municipios == null)
                {
                    municipios = await _localidadeService.GetMunicipios(coUf);
                    SetCache(cacheKey, municipios);
                }

                return StatusCode(200, municipios);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Erro -  ListaMunicipio: {coUf}");
                return StatusCode(500, "Erro de serviço");
            }
        }

        [HttpGet]
        [Route("v1/nacionalidade")]
        public async Task<ActionResult<PaisResponse>> ListaNacionalidade()
        {
            try
            {
                List<PaisResponse> paises = GetCache<List<PaisResponse>>(CacheTypes.Nacionalidade);

                if (paises == null)
                {
                    paises = await _localidadeService.GetPaises();
                    SetCache(CacheTypes.Nacionalidade, paises);
                }

                return StatusCode(200, paises);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Erro -  ListaNacionalidade: {GetCache<List<PaisResponse>>(CacheTypes.Nacionalidade)}");
                return StatusCode(500, "Erro de serviço");
            }
        }
    }
}
