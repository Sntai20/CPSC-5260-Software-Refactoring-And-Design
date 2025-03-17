namespace CodeSmellDetection.Options;

/// <summary>
/// Options for configuring the long method detector.
/// </summary>
internal class LongMethodOptions
{
    /// <summary>
    /// Gets or sets the threshold for the number of lines in a method before it is considered a long method.
    /// </summary>
    /// <value>
    /// The number of lines in a method. Default is 15.
    /// </value>
    public int MethodLineCountThreshold { get; set; } = 15;
}