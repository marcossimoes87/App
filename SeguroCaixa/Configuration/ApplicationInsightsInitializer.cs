using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Http;

namespace SeguroCaixa.Configuration
{
    public class ApplicationInsightsInitializer : ITelemetryInitializer
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private ITelemetry _telemetry;

        public ApplicationInsightsInitializer(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public void Initialize(ITelemetry telemetry)
        {
            _telemetry = telemetry;

            AdicionarDadosDeRequest();
        }

        private void AdicionarDadosDeRequest()
        {
            var context = _httpContextAccessor.HttpContext;
            if (context?.Request != null)
            {
                var headers = context.Request.Headers;
                if (headers.ContainsKey("origin") && !string.IsNullOrWhiteSpace(headers["origin"].ToString()))
                    SetProperty("origin", headers["origin"].ToString());

                if (headers.ContainsKey("referer") && !string.IsNullOrWhiteSpace(headers["referer"].ToString()))
                    SetProperty("referer", headers["referer"].ToString());
            }
        }

        private void SetProperty(string key, string value)
        {
            if (!_telemetry.Context.GlobalProperties.ContainsKey(key))
                _telemetry.Context.GlobalProperties.Add(key, value);
            else
                _telemetry.Context.GlobalProperties[key] = value;
        }
    }
}
