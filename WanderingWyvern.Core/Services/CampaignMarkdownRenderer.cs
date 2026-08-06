using DhrMaes.WanderingWyvern.Core.Models;
using Markdig;
using Markdig.Renderers;
using Markdig.Renderers.Html;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;

namespace DhrMaes.WanderingWyvern.Core.Services;

/// <summary>
/// Renders Markdown using a campaign content store and rewrites resolvable campaign links
/// into campaign document navigation links.
/// </summary>
public sealed class CampaignMarkdownRenderer
{
    private readonly MarkdownPipeline _pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
    private readonly ICampaignContentStore _contentStore;

    public CampaignMarkdownRenderer(ICampaignContentStore contentStore)
    {
        _contentStore = contentStore;
    }

    public async Task<string> RenderToHtmlAsync(
        CampaignDocument document,
        CampaignIndex index,
        CancellationToken cancellationToken = default)
    {
        var markdown = await _contentStore.ReadTextAsync(document.RelativePath, cancellationToken);
        var parsedDocument = Markdown.Parse(markdown, _pipeline);
        var currentDirectory = GetRelativeDirectory(document.RelativePath);

        foreach (var link in parsedDocument.Descendants<LinkInline>())
            RewriteLinkIfResolvable(link, currentDirectory, index);

        using var writer = new StringWriter();
        var renderer = new HtmlRenderer(writer);
        _pipeline.Setup(renderer);
        renderer.Render(parsedDocument);
        return writer.ToString();
    }

    private static string GetRelativeDirectory(string relativePath) =>
        relativePath.Contains('/')
            ? relativePath[..relativePath.LastIndexOf('/')]
            : string.Empty;

    private void RewriteLinkIfResolvable(
        LinkInline link,
        string currentDirectory,
        CampaignIndex index)
    {
        if (string.IsNullOrEmpty(link.Url) || Uri.IsWellFormedUriString(link.Url, UriKind.Absolute))
            return;

        var target = ResolveRelativeLink(currentDirectory, link.Url, index);
        if (target is null)
            return;

        link.Url = BuildDocumentUrl(target.RelativePath);
    }

    private static CampaignDocument? ResolveRelativeLink(
        string currentDirectory,
        string url,
        CampaignIndex index)
    {
        var withoutFragment = url.Split('#')[0];
        if (string.IsNullOrWhiteSpace(withoutFragment))
            return null;

        var path = string.IsNullOrEmpty(currentDirectory)
            ? withoutFragment
            : $"{currentDirectory}/{withoutFragment}";

        return index.FindDocument(NormalizeRelativePath(path));
    }

    private static string NormalizeRelativePath(string path)
    {
        var segments = new List<string>();
        foreach (var segment in path.Replace('\\', '/').Split('/', StringSplitOptions.RemoveEmptyEntries))
        {
            if (segment == ".")
                continue;

            if (segment == "..")
            {
                if (segments.Count == 0)
                    return string.Empty;

                segments.RemoveAt(segments.Count - 1);
                continue;
            }

            segments.Add(segment);
        }

        return string.Join('/', segments);
    }

    private static string BuildDocumentUrl(string relativePath) =>
        "/document/" + string.Join(
            '/',
            relativePath.Split('/', StringSplitOptions.RemoveEmptyEntries).Select(Uri.EscapeDataString));
}
