using DevMemory.Core;
using DevMemory.Infrastructure;

namespace DevMemory.Application;

public sealed class MemoryService
{
    private readonly MemoryRepository _repository;
    private readonly MarkdownMemoryExporter _markdownExporter;

    public MemoryService()
    {
        _repository = new MemoryRepository();
        _markdownExporter = new MarkdownMemoryExporter();
    }

    public string Add(TaskMemory memory)
    {
        var memories = _repository.Load();

        memories.Add(memory);

        _repository.Save(memories);

        return _markdownExporter.Export(memory);
    }

    public IReadOnlyCollection<TaskMemory> List()
    {
        return _repository
            .Load()
            .OrderByDescending(memory => memory.CreatedAt)
            .ToList();
    }

    public TaskMemory? GetById(Guid id)
    {
        return _repository
            .Load()
            .FirstOrDefault(memory => memory.Id == id);
    }

    public IReadOnlyCollection<TaskMemory> Search(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return [];
        }

        var normalizedQuery = query.Trim();

        return _repository
            .Load()
            .Where(memory =>
                Contains(memory.Title, normalizedQuery) ||
                Contains(memory.Project, normalizedQuery) ||
                Contains(memory.Area, normalizedQuery) ||
                Contains(memory.Branch, normalizedQuery) ||
                Contains(memory.Problem, normalizedQuery) ||
                Contains(memory.Solution, normalizedQuery) ||
                Contains(memory.LessonsLearned, normalizedQuery) ||
                memory.Tags.Any(tag => Contains(tag, normalizedQuery)) ||
                memory.Decisions.Any(decision => Contains(decision, normalizedQuery)) ||
                memory.FilesTouched.Any(file => Contains(file, normalizedQuery)) ||
                memory.Tests.Any(test => Contains(test, normalizedQuery)))
            .OrderByDescending(memory => memory.CreatedAt)
            .ToList();
    }

    public string GetStorageFilePath()
    {
        return _repository.GetStorageFilePath();
    }

    public string GetMarkdownDirectoryPath()
    {
        return _repository.GetMarkdownDirectoryPath();
    }

    private static bool Contains(string value, string query)
    {
        return value.Contains(query, StringComparison.OrdinalIgnoreCase);
    }
}