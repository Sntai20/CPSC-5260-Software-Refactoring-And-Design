namespace CodeSmellDetection.Options;

/// <summary>
/// Options for configuring the long parameter list detector.
/// </summary>
internal class LongParameterListDetectorOptions
{
    /// <summary>
    /// Gets or sets the threshold for the number of parameters that triggers a detection.
    /// </summary>
    /// <value>
    /// The threshold for the number of parameters. Default is 3.
    /// </value>
    public int ParameterThreshold { get; internal set; } = 3;
}