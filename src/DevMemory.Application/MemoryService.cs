using DevMemory.Application.Abstractions;
using DevMemory.Application.Models;
using DevMemory.Application.Normalization;
using DevMemory.Application.Validation;
using DevMemory.Core;

namespace DevMemory.Application;

public sealed class MemoryService
{
    private readonly IMemoryRepository _repository;
    private readonly IMemoryExporter _memoryExporter;

    public MemoryService(
        IMemoryRepository repository,
        IMemoryExporter memoryExporter)
    {
        _repository = repository;
        _memoryExporter = memoryExporter;
    }

    public AddMemoryResult Add(TaskMemory memory)
    {
        TaskMemoryNormalizer.Normalize(memory);

        var errors = TaskMemoryValidator.Validate(memory);

        if (errors.Count > 0)
        {
            return AddMemoryResult.Fail(errors);
        }

        var memories = _repository.Load();

        memories.Add(memory);

        _repository.Save(memories);

        var markdownFilePath = _memoryExporter.Export(memory);

        return AddMemoryResult.Ok(markdownFilePath);
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