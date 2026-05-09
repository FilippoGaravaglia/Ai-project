using System.Text.Json;
using AiAgent.Core;

namespace AiAgent.Infrastructure;

public sealed class MemoryRepository
{
    private const string FilePath = "devmemory.json";

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true
    };

    public List<TaskMemory> Load()
    {
        if (!File.Exists(FilePath))
        {
            return [];
        }

        var json = File.ReadAllText(FilePath);

        if (string.IsNullOrWhiteSpace(json))
        {
            return [];
        }

        return JsonSerializer.Deserialize<List<TaskMemory>>(json, JsonOptions) ?? [];
    }

    public void Save(List<TaskMemory> memories)
    {
        var json = JsonSerializer.Serialize(memories, JsonOptions);
        File.WriteAllText(FilePath, json);
    }
}