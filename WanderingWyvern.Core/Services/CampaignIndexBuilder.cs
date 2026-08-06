using DhrMaes.WanderingWyvern.Core.Models;

namespace DhrMaes.WanderingWyvern.Core.Services;

/// <summary>
/// Builds a campaign index from a storage provider. The same builder can run on the server or
/// in WebAssembly because it only depends on campaign-relative paths and the content-store API.
/// </summary>
public sealed class CampaignIndexBuilder
{
    private static readonly string[] ImageExtensions = [".png", ".jpg", ".jpeg", ".gif", ".webp"];

    public async Task<CampaignIndex> BuildAsync(
        ICampaignContentStore contentStore,
        CancellationToken cancellationToken = default)
    {
        var files = (await contentStore.ListFilesAsync(cancellationToken))
            .Select(NormalizeRelativePath)
            .Where(path => !IsHidden(path))
            .Where(path => !IsPromptDocument(path))
            .ToArray();
        var documentsByPath = new Dictionary<string, CampaignDocument>(StringComparer.OrdinalIgnoreCase);

        foreach (var file in files.Where(IsMarkdown))
        {
            var document = await CreateDocumentAsync(file, contentStore, cancellationToken);
            documentsByPath[document.RelativePath] = document;
        }

        return new CampaignIndex
        {
            Sessions = await ScanEntityFolderAsync("Sessions", CampaignEntityType.Session, files, documentsByPath, contentStore, cancellationToken),
            Npcs = await ScanEntityFolderAsync("NPCs", CampaignEntityType.Npc, files, documentsByPath, contentStore, cancellationToken),
            Locations = await ScanEntityFolderAsync("Locations", CampaignEntityType.Location, files, documentsByPath, contentStore, cancellationToken),
            Lore = await ScanEntityFolderAsync("Lore", CampaignEntityType.Lore, files, documentsByPath, contentStore, cancellationToken),
            Pcs = await ScanEntityFolderAsync("PCs", CampaignEntityType.Pc, files, documentsByPath, contentStore, cancellationToken),
            Items = await ScanEntityFolderAsync("Items", CampaignEntityType.Item, files, documentsByPath, contentStore, cancellationToken),
            Handouts = await ScanEntityFolderAsync("Handouts", CampaignEntityType.Handout, files, documentsByPath, contentStore, cancellationToken),
            DocumentsByPath = documentsByPath
        };
    }

    private async Task<List<CampaignEntity>> ScanEntityFolderAsync(
        string folderName,
        CampaignEntityType type,
        IReadOnlyList<string> files,
        IReadOnlyDictionary<string, CampaignDocument> documentsByPath,
        ICampaignContentStore contentStore,
        CancellationToken cancellationToken)
    {
        var prefix = folderName + "/";
        var entries = files
            .Where(path => path.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            .Select(path => path[prefix.Length..].Split('/')[0])
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(path => path, StringComparer.OrdinalIgnoreCase);
        var entities = new List<CampaignEntity>();

        foreach (var entry in entries)
        {
            var entryPath = prefix + entry;
            if (files.Any(path => string.Equals(path, entryPath, StringComparison.OrdinalIgnoreCase)
                && IsMarkdown(path)))
            {
                entities.Add(BuildSingleFileEntity(entryPath, type, documentsByPath, files));
            }
            else if (files.Any(path => path.StartsWith(entryPath + "/", StringComparison.OrdinalIgnoreCase)))
            {
                var entity = BuildFolderEntity(entryPath, type, documentsByPath, files);
                if (entity is not null)
                    entities.Add(entity);
            }
        }

        await Task.CompletedTask;
        return entities;
    }

    private static CampaignEntity? BuildFolderEntity(
        string folderPath,
        CampaignEntityType type,
        IReadOnlyDictionary<string, CampaignDocument> documentsByPath,
        IReadOnlyList<string> files)
    {
        var documents = new Dictionary<string, CampaignDocument>(StringComparer.OrdinalIgnoreCase);
        var images = new List<string>();
        string? iconImage = null;

        foreach (var file in files.Where(path => GetDirectory(path).Equals(folderPath, StringComparison.OrdinalIgnoreCase)))
        {
            if (IsMarkdown(file))
            {
                if (documentsByPath.TryGetValue(file, out var document))
                    documents[document.FileName] = document;
            }
            else if (IsImage(file))
            {
                if (IsIconImage(file) && iconImage is null)
                    iconImage = file;
                else
                    images.Add(file);
            }
        }

        if (documents.Count == 0)
            return null;

        var slug = folderPath[(folderPath.LastIndexOf('/') + 1)..];
        return new CampaignEntity
        {
            Type = type,
            Slug = slug,
            Title = ResolveFolderTitle(slug, documents),
            RelativeFolderPath = folderPath,
            Documents = documents,
            Images = images,
            IconImage = iconImage
        };
    }

    private static CampaignEntity BuildSingleFileEntity(
        string filePath,
        CampaignEntityType type,
        IReadOnlyDictionary<string, CampaignDocument> documentsByPath,
        IReadOnlyList<string> files)
    {
        var document = documentsByPath[filePath];
        var fileName = GetFileNameWithoutExtension(filePath);
        var directory = GetDirectory(filePath);
        var images = files
            .Where(path => GetDirectory(path).Equals(directory, StringComparison.OrdinalIgnoreCase))
            .Where(path => IsImage(path) && GetImageBaseName(path).Equals(fileName, StringComparison.OrdinalIgnoreCase))
            .Where(path => !IsIconImage(path))
            .ToList();
        var icon = files
            .FirstOrDefault(path => GetDirectory(path).Equals(directory, StringComparison.OrdinalIgnoreCase)
                && IsImage(path)
                && IsIconImage(path)
                && GetImageBaseName(path).Equals(fileName, StringComparison.OrdinalIgnoreCase));

        return new CampaignEntity
        {
            Type = type,
            Slug = fileName,
            Title = document.Title,
            RelativeFolderPath = directory,
            Documents = new Dictionary<string, CampaignDocument>(StringComparer.OrdinalIgnoreCase)
            {
                [document.FileName] = document
            },
            Images = images,
            IconImage = icon
        };
    }

    private static async Task<CampaignDocument> CreateDocumentAsync(
        string relativePath,
        ICampaignContentStore contentStore,
        CancellationToken cancellationToken)
    {
        var fileName = GetFileNameWithoutExtension(relativePath);
        var extractedTitle = await ExtractTitleAsync(relativePath, contentStore, cancellationToken);
        return new CampaignDocument
        {
            RelativePath = relativePath,
            Title = extractedTitle ?? Humanize(fileName),
            FileName = fileName,
            HasExplicitTitle = extractedTitle is not null
        };
    }

    private static async Task<string?> ExtractTitleAsync(
        string relativePath,
        ICampaignContentStore contentStore,
        CancellationToken cancellationToken)
    {
        var content = await contentStore.ReadTextAsync(relativePath, cancellationToken);
        foreach (var line in content.Split(["\r\n", "\n"], StringSplitOptions.None).Take(20))
        {
            if (line.StartsWith("# ", StringComparison.Ordinal))
                return line[2..].Trim();
        }

        return null;
    }

    private static string ResolveFolderTitle(
        string slug,
        IReadOnlyDictionary<string, CampaignDocument> documents)
    {
        if (documents.TryGetValue(slug, out var matching))
            return matching.Title;

        if (documents.TryGetValue("outline", out var outline))
            return outline.Title;

        return documents.Values.First().Title;
    }

    private static bool IsMarkdown(string path) =>
        Path.GetExtension(path).Equals(".md", StringComparison.OrdinalIgnoreCase);

    private static bool IsPromptDocument(string path) =>
        Path.GetFileNameWithoutExtension(path)
            .EndsWith(".prompt", StringComparison.OrdinalIgnoreCase);

    private static bool IsImage(string path) =>
        ImageExtensions.Contains(Path.GetExtension(path), StringComparer.OrdinalIgnoreCase);

    private static bool IsIconImage(string path) =>
        Path.GetFileNameWithoutExtension(path).EndsWith(".icon", StringComparison.OrdinalIgnoreCase);

    private static string GetImageBaseName(string path)
    {
        var name = Path.GetFileNameWithoutExtension(path);
        return name.EndsWith(".icon", StringComparison.OrdinalIgnoreCase)
            ? name[..^5]
            : name;
    }

    private static string GetFileNameWithoutExtension(string path) =>
        Path.GetFileNameWithoutExtension(path);

    private static string GetDirectory(string path)
    {
        var separator = path.LastIndexOf('/');
        return separator < 0 ? string.Empty : path[..separator];
    }

    private static string NormalizeRelativePath(string path) => path.Replace('\\', '/');

    private static bool IsHidden(string path) =>
        path.Split('/').Any(segment =>
            segment.StartsWith('.') ||
            segment.Equals("bin", StringComparison.OrdinalIgnoreCase) ||
            segment.Equals("obj", StringComparison.OrdinalIgnoreCase));

    private static string Humanize(string fileOrFolderName)
    {
        var withSpaces = fileOrFolderName.Replace('-', ' ').Replace('_', ' ');
        return System.Globalization.CultureInfo.InvariantCulture.TextInfo.ToTitleCase(withSpaces);
    }
}
