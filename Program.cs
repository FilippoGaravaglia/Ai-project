using AiAgent.Application;
using AiAgent.Core;

var service = new MemoryService();

Console.WriteLine("DevMemory - Local Developer Memory");
Console.WriteLine("----------------------------------");
Console.WriteLine("Commands:");
Console.WriteLine("1. add");
Console.WriteLine("2. list");
Console.WriteLine("3. search");
Console.WriteLine("4. show");
Console.WriteLine();

Console.Write("Command: ");
var command = Console.ReadLine()?.Trim().ToLowerInvariant();

switch (command)
{
    case "add":
    case "1":
        AddMemory(service);
        break;

    case "list":
    case "2":
        ListMemories(service);
        break;

    case "search":
    case "3":
        SearchMemories(service);
        break;

    case "show":
    case "4":
        ShowMemory(service);
        break;

    default:
        Console.WriteLine("Unknown command.");
        break;
}

static void AddMemory(MemoryService service)
{
    var memory = new TaskMemory
    {
        Title = AskRequired("Title"),
        Project = AskRequired("Project"),
        Area = AskRequired("Area"),
        Branch = AskOptional("Branch"),
        Tags = AskList("Tags comma separated"),
        Problem = AskRequired("Problem"),
        Solution = AskRequired("Solution"),
        Decisions = AskMultilineList("Decisions"),
        FilesTouched = AskMultilineList("Files touched"),
        Tests = AskMultilineList("Tests added/updated"),
        LessonsLearned = AskOptional("Lessons learned")
    };

    service.Add(memory);

    Console.WriteLine();
    Console.WriteLine("Memory saved.");
    Console.WriteLine($"Id: {memory.Id}");
}

static void ListMemories(MemoryService service)
{
    var memories = service.List();

    if (!memories.Any())
    {
        Console.WriteLine("No memories found.");
        return;
    }

    foreach (var memory in memories)
    {
        Console.WriteLine($"{memory.Id} | {memory.CreatedAt:u} | {memory.Project} | {memory.Area} | {memory.Title}");
    }
}

static void SearchMemories(MemoryService service)
{
    var query = AskRequired("Search query");
    var results = service.Search(query);

    if (!results.Any())
    {
        Console.WriteLine("No matching memories found.");
        return;
    }

    foreach (var memory in results)
    {
        Console.WriteLine($"{memory.Id} | {memory.Project} | {memory.Area} | {memory.Title}");
    }
}

static void ShowMemory(MemoryService service)
{
    var idText = AskRequired("Memory id");

    if (!Guid.TryParse(idText, out var id))
    {
        Console.WriteLine("Invalid id.");
        return;
    }

    var memory = service.GetById(id);

    if (memory is null)
    {
        Console.WriteLine("Memory not found.");
        return;
    }

    Console.WriteLine();
    Console.WriteLine($"# {memory.Title}");
    Console.WriteLine();
    Console.WriteLine($"Id: {memory.Id}");
    Console.WriteLine($"Project: {memory.Project}");
    Console.WriteLine($"Area: {memory.Area}");
    Console.WriteLine($"Branch: {memory.Branch}");
    Console.WriteLine($"Created at: {memory.CreatedAt:u}");
    Console.WriteLine($"Tags: {string.Join(", ", memory.Tags)}");
    Console.WriteLine();

    Console.WriteLine("## Problem");
    Console.WriteLine(memory.Problem);
    Console.WriteLine();

    Console.WriteLine("## Solution");
    Console.WriteLine(memory.Solution);
    Console.WriteLine();

    PrintList("## Decisions", memory.Decisions);
    PrintList("## Files touched", memory.FilesTouched);
    PrintList("## Tests", memory.Tests);

    Console.WriteLine("## Lessons learned");
    Console.WriteLine(memory.LessonsLearned);
}

static string AskRequired(string label)
{
    while (true)
    {
        Console.Write($"{label}: ");
        var value = Console.ReadLine();

        if (!string.IsNullOrWhiteSpace(value))
        {
            return value.Trim();
        }

        Console.WriteLine($"{label} is required.");
    }
}

static string AskOptional(string label)
{
    Console.Write($"{label}: ");
    return Console.ReadLine()?.Trim() ?? string.Empty;
}

static List<string> AskList(string label)
{
    Console.Write($"{label}: ");
    var value = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(value))
    {
        return [];
    }

    return value
        .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
        .ToList();
}

static List<string> AskMultilineList(string label)
{
    Console.WriteLine($"{label} - write one item per line. Leave empty line to finish.");

    var values = new List<string>();

    while (true)
    {
        Console.Write("- ");
        var value = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(value))
        {
            break;
        }

        values.Add(value.Trim());
    }

    return values;
}

static void PrintList(string title, IReadOnlyCollection<string> values)
{
    Console.WriteLine(title);

    if (!values.Any())
    {
        Console.WriteLine("-");
        Console.WriteLine();
        return;
    }

    foreach (var value in values)
    {
        Console.WriteLine($"- {value}");
    }

    Console.WriteLine();
}