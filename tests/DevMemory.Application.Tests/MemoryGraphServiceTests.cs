using DevMemory.Application.Abstractions;
using DevMemory.Application.Models.Graph;
using DevMemory.Core;

namespace DevMemory.Application.Tests;

public sealed class MemoryGraphServiceTests
{
    [Fact]
    public void ExportGraph_WhenMemoriesExist_BuildsGraphWithExpectedNodesAndEdges()
    {
        // Arrange
        var repository = new InMemoryRepository
        {
            Memories =
            [
                new TaskMemory
                {
                    Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                    Title = "Fix revision bug",
                    Project = "LogicalCommon",
                    Area = "Estimate",
                    Tags = ["dotnet", "bug-fix"],
                    FilesTouched = ["src/EstimateRevisionService.cs"]
                }
            ]
        };

        var exporter = new InMemoryGraphExporter();

        var service = new MemoryGraphService(repository, exporter);

        // Act
        var result = service.ExportGraph();

        // Assert
        Assert.Equal("/fake/path/devmemory-graph.json", result.FilePath);
        Assert.NotNull(exporter.ExportedGraph);

        Assert.Contains(exporter.ExportedGraph.Nodes, node => node.Id == "memory:aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
        Assert.Contains(exporter.ExportedGraph.Nodes, node => node.Id == "project:logicalcommon");
        Assert.Contains(exporter.ExportedGraph.Nodes, node => node.Id == "area:estimate");
        Assert.Contains(exporter.ExportedGraph.Nodes, node => node.Id == "tag:dotnet");
        Assert.Contains(exporter.ExportedGraph.Nodes, node => node.Id == "file:src/estimaterevisionservice.cs");

        Assert.Contains(exporter.ExportedGraph.Edges, edge =>
            edge.SourceId == "memory:aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa" &&
            edge.TargetId == "project:logicalcommon" &&
            edge.Type == "belongs_to_project");

        Assert.Contains(exporter.ExportedGraph.Edges, edge =>
            edge.SourceId == "memory:aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa" &&
            edge.TargetId == "tag:dotnet" &&
            edge.Type == "has_tag");
    }

    private sealed class InMemoryRepository : IMemoryRepository
    {
        public List<TaskMemory> Memories { get; init; } = [];

        public List<TaskMemory> Load()
        {
            return Memories.ToList();
        }

        public void Save(List<TaskMemory> memories)
        {
        }

        public string GetStorageFilePath()
        {
            return "/fake/path/devmemory.json";
        }

        public string GetMarkdownDirectoryPath()
        {
            return "/fake/path/markdown";
        }
    }

    private sealed class InMemoryGraphExporter : IMemoryGraphExporter
    {
        public MemoryGraph? ExportedGraph { get; private set; }

        public string Export(MemoryGraph graph, string? outputPath = null)
        {
            ExportedGraph = graph;
            return outputPath ?? "/fake/path/devmemory-graph.json";
        }
    }
}