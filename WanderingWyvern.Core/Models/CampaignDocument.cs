namespace DhrMaes.WanderingWyvern.Core.Models;

/// <summary>
/// A single Markdown file discovered under the campaign content root.
/// </summary>
public sealed class CampaignDocument
{
    /// <summary>Path relative to the campaign root, using forward slashes.</summary>
    public required string RelativePath { get; init; }

    /// <summary>Display title, taken from the first Markdown H1 heading, falling back to the file name.</summary>
    public required string Title { get; init; }

    /// <summary>Whether the display title came from an explicit Markdown H1 heading.</summary>
    public bool HasExplicitTitle { get; init; }

    /// <summary>File name without extension (e.g. "robert", "outline", "script").</summary>
    public required string FileName { get; init; }
}
