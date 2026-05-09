namespace AiAgent.Infrastructure;

public sealed class DevMemoryStorageOptions
{
    public string StorageDirectory { get; init; } = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
        ".devmemory");

    public string StorageFileName { get; init; } = "devmemory.json";

    public string MarkdownDirectoryName { get; init; } = "markdown";

    public string StorageFilePath => Path.Combine(StorageDirectory, StorageFileName);

    public string MarkdownDirectoryPath => Path.Combine(StorageDirectory, MarkdownDirectoryName);
}