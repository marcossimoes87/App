using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.ApplicationInsights.WindowsServer.Channel.Implementation;
using Microsoft.ApplicationInsights.WindowsServer.TelemetryChannel;

namespace SeguroCaixa.Configuration
{
    internal class SamplingProcessor : AdaptiveSamplingTelemetryProcessor
    {
        private readonly ITelemetryProcessor _next;

        public SamplingProcessor(ITelemetryProcessor next) : base(next)
        {
            _next = next;
        }

        public SamplingProcessor(SamplingPercentageEstimatorSettings settings, AdaptiveSamplingPercentageEvaluatedCallback callback, ITelemetryProcessor next)
            : base(settings, callback, next)
        {
            _next = next;
        }

        public new void Process(ITelemetry item)
        {
            if (item is TraceTelemetry trace)
            {
                _next.Process(trace);
                return;
            }

            base.Process(item);
        }
    }
}