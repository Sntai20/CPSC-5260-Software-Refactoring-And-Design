namespace CodeSmellDetection.Options;

/// <summary>
/// Options for configuring the duplicated code detector.
/// </summary>
internal class DuplicatedCodeDetectorOptions
{
    /// <summary>
    /// Gets or sets the Jaccard similarity threshold for detecting duplicated code.
    /// A value between 0 and 1, where 1 means exact match and 0 means no similarity.
    /// Default value is 0.75.
    /// </summary>
    public double JaccardThreshold { get; internal set; } = 0.75;
}