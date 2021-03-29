using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;

namespace SeguroCaixa.Controllers
{
    public class BaseController : ControllerBase
    {
        protected IMemoryCache _cache;
        protected IConfiguration _configuration;
        private System.Threading.CancellationTokenSource cts;

        public BaseController(IMemoryCache memoryCache, IConfiguration configuration)
        {
            _cache = memoryCache;
            _configuration = configuration;
            if ((cts == null && !_cache.TryGetValue(Helpers.CacheTypes.CancellationToken, out cts)) || (cts != null && cts.IsCancellationRequested))
            {
                cts = new System.Threading.CancellationTokenSource(TimeSpan.FromMinutes(int.Parse(_configuration["CacheTimeInMinutes"])));
                _cache.Set(Helpers.CacheTypes.CancellationToken, cts);
            }
        }

        public decimal CPF
        {
            get
            {
                if (HttpContext != null && HttpContext.User != null && HttpContext.User.Claims.Any(x => x.Type.Equals("preferred_username")))
                {
                    return decimal.Parse(HttpContext.User.Claims.FirstOrDefault(x => x.Type.Equals("preferred_username")).Value);
                }

                return 0m;
            }
        }
        public string MatriculaFuncCaixa
        {
            get
            {
                if (HttpContext != null && HttpContext.User != null && HttpContext.User.Claims.Any(x => x.Type.Equals("preferred_username")))
                {
                    return HttpContext.User.Claims.FirstOrDefault(x => x.Type.Equals("preferred_username"))?.Value.ToString();
                }
                return "";
            }
        }

        public bool IsValidUser(decimal CPFParam)
        {
            int nivel = 0;
            decimal CPFToken = CPF;

            return (CPFToken.Equals(CPFParam) && nivel.Equals(12)) ? true : false;
            
        }

        public string IP
        {
            get
            {
                if (HttpContext.Request.Headers.ContainsKey("X-Forwarded-For"))
                {
                    return HttpContext.Request.Headers["X-Forwarded-For"];
                }
                return HttpContext.Connection.RemoteIpAddress.ToString();
            }
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public T GetCache<T>(string cacheType) where T : class
        {
            T responseCache;

            if (!_cache.TryGetValue(cacheType, out responseCache))
            {
                return null;
            }

            return responseCache;
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        public void SetCache(string cacheType, object obj)
        {
            var cacheEntryOptions = new MemoryCacheEntryOptions().AddExpirationToken(new Microsoft.Extensions.Primitives.CancellationChangeToken(cts.Token));
            _cache.Set(cacheType, obj, cacheEntryOptions);
        }

        [HttpGet]
        [Route("api/segurocaixa/v1/LimparCache")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public ActionResult<string> LimparCache()
        {
            try
            {
                _cache.Get<System.Threading.CancellationTokenSource>(Helpers.CacheTypes.CancellationToken).Cancel();

                return StatusCode(200);
            }
            catch (Exception e)
            {
                return StatusCode(500, string.Format("Erro de serviço ao limpar o cache: {0}", e));
            }
        }
    }
}
