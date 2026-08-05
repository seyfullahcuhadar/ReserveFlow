namespace ReserveFlow.Infrastructure.Options;

public sealed class ObservabilityOptions
{
    public const string SectionName = "Observability";

    public bool Enabled { get; init; } = true;

    public string ServiceName { get; init; } = "ReserveFlow.Api";

    public string ServiceNamespace { get; init; } = "ReserveFlow";

    /// <summary>
    /// OTLP/gRPC endpoint that receives traces (Alloy/LGTM on the host).
    /// Local: http://localhost:4317.
    /// </summary>
    public string TracesEndpoint { get; init; } = "http://localhost:4317";

    /// <summary>
    /// OTLP/HTTP endpoint that receives metrics (Alloy/LGTM on the host).
    /// Programmatic configuration must include the signal path (/v1/metrics).
    /// Local: http://localhost:4318/v1/metrics.
    /// </summary>
    public string MetricsEndpoint { get; init; } = "http://localhost:4318/v1/metrics";

    /// <summary>
    /// OTLP/HTTP endpoint that receives logs (Alloy/LGTM on the host).
    /// Programmatic configuration must include the signal path (/v1/logs).
    /// Local: http://localhost:4318/v1/logs.
    /// </summary>
    public string LogsEndpoint { get; init; } = "http://localhost:4318/v1/logs";
}
