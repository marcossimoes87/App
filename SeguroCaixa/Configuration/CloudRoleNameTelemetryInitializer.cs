using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.Configuration;

namespace SeguroCaixa.Configuration
{
    public class CloudRoleNameTelemetryInitializer : ITelemetryInitializer
    {
        private IConfiguration _configuration;
        public CloudRoleNameTelemetryInitializer(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public void Initialize(ITelemetry telemetry)
        {
            telemetry.Context.Cloud.RoleName = _configuration["ApplicationInsights:RoleName"];
        }
    }
}
