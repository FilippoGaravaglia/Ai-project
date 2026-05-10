<div align="center">

# DevMemory

### Local-first developer memory for .NET engineers

DevMemory is a local CLI tool that helps developers capture, search, export and visualize technical knowledge produced during day-to-day software engineering work.

It is designed to avoid losing context across tasks, branches, commits, code reviews and AI/chat sessions.

</div>

---

## Overview

DevMemory is a **local-first developer memory CLI** built with .NET.

It allows developers to save structured engineering memories containing:

- project
- area
- branch
- tags
- problem
- solution
- decisions
- files touched
- tests
- lessons learned

Those memories can then be searched, exported to Markdown, enriched from Git context, and visualized as a knowledge graph.

The project is currently under active development and is intended both as a practical daily developer tool and as a portfolio project focused on clean .NET architecture, CLI tooling, Git integration and local knowledge management.

---

## Why DevMemory?

When working on multiple tasks, branches, repositories and AI-assisted development sessions, useful technical context is often lost.

DevMemory helps answer questions such as:

- What did I change last time in this area?
- Which files are usually involved in this feature?
- What decisions did I make during that refactor?
- Which previous tasks touched this component?
- What lessons did I learn from a bug fix?
- Which memories are related to this project, tag or file?

---

## Features

### Structured task memories

Create a new memory from the terminal:

```bash
devmemory add
```

Each memory stores technical context in a structured format:

```text
Title
Project
Area
Branch
Tags
Problem
Solution
Decisions
Files touched
Tests
Lessons learned
```

---

### Local-first storage

By default, DevMemory stores all data locally:

```text
~/.devmemory/devmemory.json
```

No cloud sync, no external services and no LLM API are required.

You can customize the storage directory with:

```bash
DEVMEMORY_HOME=~/devmemory-work devmemory storage
```

---

### Resilient JSON persistence

DevMemory currently uses JSON storage with defensive writes:

- writes to a temporary file
- creates a backup file
- replaces the main storage file only after a successful write

This reduces the risk of corrupting the local memory store.

---

### Markdown export

Every saved memory is automatically exported to Markdown:

```text
~/.devmemory/markdown/
```

The generated Markdown includes:

- metadata
- problem
- solution
- decisions
- files touched
- tests
- lessons learned
- continuation prompt

This makes memories easy to reuse in documentation, GitHub Copilot, ChatGPT or future AI-assisted workflows.

---

### Ranked search with filters

Search memories by free text:

```bash
devmemory search revision
```

Filter by project:

```bash
devmemory search revision --project LogicalCommon
```

Filter by area:

```bash
devmemory search revision --area Estimate
```

Filter by tag:

```bash
devmemory search revision --tag dotnet
```

Search results include a relevance score.

---

### Git inspection

Inspect the current Git repository:

```bash
devmemory git-status
```

Or inspect a specific repository:

```bash
devmemory git-status --path ~/work/LogicalCommon
```

DevMemory reads:

- repository path
- current branch
- last commit hash
- last commit message
- changed files

Generated files are automatically excluded.

---

### Learn from Git

Create a new memory starting from the current Git context:

```bash
devmemory learn-from-git
```

This pre-fills:

- project
- branch
- files touched

You then complete the memory manually with:

- area
- tags
- problem
- solution
- decisions
- tests
- lessons learned

---

### Knowledge graph export

Export all stored memories as a graph JSON:

```bash
devmemory graph-export
```

Default output:

```text
~/.devmemory/graph/devmemory-graph.json
```

The graph contains relationships such as:

```text
Memory -> Project
Memory -> Area
Memory -> Tag
Memory -> File
```

---

### HTML knowledge graph view

Generate a local HTML visualization of the knowledge graph:

```bash
devmemory graph-view
```

Default output:

```text
~/.devmemory/graph/devmemory-graph.html
```

Open it in the browser:

```bash
open ~/.devmemory/graph/devmemory-graph.html
```

The graph currently visualizes:

- memory nodes
- project nodes
- area nodes
- tag nodes
- file nodes

---

### Generated file filtering

DevMemory automatically excludes noisy/generated files from Git inspection and graph generation:

```text
bin/
obj/
artifacts/
.git/
.vs/
.idea/
node_modules/
dist/
build/
coverage/
*.dll
*.exe
*.pdb
*.nupkg
*.snupkg
*.deps.json
*.runtimeconfig.json
```

This keeps the generated knowledge graph focused on meaningful source files.

---

## Architecture

DevMemory is structured as a layered .NET solution.

```text
DevMemory.slnx
├── src
│   ├── DevMemory.Core
│   ├── DevMemory.Application
│   ├── DevMemory.Infrastructure
│   └── DevMemory.Cli
├── tests
│   ├── DevMemory.Application.Tests
│   ├── DevMemory.Infrastructure.Tests
│   └── DevMemory.Cli.Tests
└── scripts
```

### DevMemory.Core

Contains the core domain models.

Example:

```text
TaskMemory
```

This project does not depend on infrastructure, CLI, file system or Git implementation details.

---

### DevMemory.Application

Contains application services, abstractions, validation, normalization, filtering, search logic, graph generation and Git memory draft creation.

Examples:

```text
MemoryService
GitMemoryDraftService
MemoryGraphService
MemoryFileFilter
IMemoryRepository
IMemoryExporter
IGitRepositoryInspector
IMemoryGraphExporter
IMemoryGraphHtmlExporter
```

The application layer defines contracts and application behavior.

---

### DevMemory.Infrastructure

Contains technical implementations.

Examples:

```text
MemoryRepository
MarkdownMemoryExporter
GitRepositoryInspector
JsonMemoryGraphExporter
HtmlMemoryGraphExporter
DevMemoryStorageOptions
```

This layer handles:

- JSON storage
- Markdown export
- Git command execution
- graph JSON export
- graph HTML export
- environment-based storage configuration

---

### DevMemory.Cli

Contains the terminal interface and command dispatching.

Current commands:

```text
add
list
search
show
storage
markdown
git-status
learn-from-git
graph-export
graph-view
help
```

The CLI entry point is intentionally kept small and works as a composition root.

Command execution is delegated to dedicated command handlers:

```text
AddCommandHandler
ListCommandHandler
SearchCommandHandler
ShowCommandHandler
StorageCommandHandler
MarkdownCommandHandler
GitStatusCommandHandler
LearnFromGitCommandHandler
GraphExportCommandHandler
GraphViewCommandHandler
HelpCommandHandler
```

Supporting CLI components include:

```text
CommandDispatcher
CommandOptions
CliPrompt
CliExitCodes
MemoryConsolePrinter
```

---

## Installation

### Package the CLI

```bash
dotnet pack src/DevMemory.Cli/DevMemory.Cli.csproj -c Release -o artifacts/packages
```

### Install as a .NET global tool

```bash
dotnet tool install --global DevMemory.Cli --add-source ./artifacts/packages
```

If already installed:

```bash
dotnet tool update --global DevMemory.Cli --add-source ./artifacts/packages
```

Make sure .NET global tools are available in your `PATH`:

```bash
export PATH="$PATH:$HOME/.dotnet/tools"
```

Then run:

```bash
devmemory help
```

---

## Usage

### Add a memory

```bash
devmemory add
```

### List memories

```bash
devmemory list
```

### Search memories

```bash
devmemory search revision
```

With filters:

```bash
devmemory search revision --project LogicalCommon
devmemory search revision --area Estimate
devmemory search revision --tag dotnet
```

### Show a memory

```bash
devmemory show <memory-id>
```

### Show storage path

```bash
devmemory storage
```

### Show Markdown directory

```bash
devmemory markdown
```

### Inspect Git status

```bash
devmemory git-status
```

### Create memory from Git context

```bash
devmemory learn-from-git
```

### Export graph JSON

```bash
devmemory graph-export
```

### Generate HTML graph view

```bash
devmemory graph-view
```

---

## Configuration

### Default storage

```text
~/.devmemory
```

Main storage file:

```text
~/.devmemory/devmemory.json
```

Markdown exports:

```text
~/.devmemory/markdown/
```

Graph exports:

```text
~/.devmemory/graph/
```

### Custom storage directory

```bash
DEVMEMORY_HOME=~/devmemory-work devmemory storage
```

---

## Local development scripts

DevMemory includes local development scripts to simplify the build, test, packaging and cleanup workflow.

### Build and test

Run the full build and test pipeline:

```bash
./scripts/build-test.sh
```

This script:

- builds the solution;
- runs the full test suite;
- fails fast if build or tests fail.

---

### Install as a local global tool

Package and install DevMemory locally as a .NET global tool:

```bash
./scripts/install-local-tool.sh
```

This script:

- builds the solution;
- runs all tests;
- cleans the local package output directory;
- creates the NuGet package under `artifacts/packages`;
- uninstalls any previous local global-tool installation;
- installs the new package globally;
- verifies the installation by running:

```bash
devmemory help
```

After this step, the CLI is available from anywhere with:

```bash
devmemory
```

---

### Clean generated files

Remove local generated files and build artifacts:

```bash
./scripts/clean-generated.sh
```

This script removes:

- `bin/`
- `obj/`
- `artifacts/`
- local `devmemory.json` files generated during development

It does not remove the real user storage under:

```text
~/.devmemory/
```

---

### Recommended local workflow

Before opening a pull request or creating a release package, run:

```bash
./scripts/build-test.sh
```

When testing the CLI as an installed tool, run:

```bash
./scripts/install-local-tool.sh
```

When you want to clean the workspace from generated files, run:

```bash
./scripts/clean-generated.sh
```

---

## Testing

Run all tests:

```bash
dotnet test DevMemory.slnx
```

The current test suite covers:

- memory service behavior
- validation and normalization
- ranked search
- storage repository
- Markdown export
- Git memory draft creation
- generated file filtering
- graph generation
- JSON graph export
- HTML graph export
- CLI command option parsing
- CLI command dispatching

---

## Development commands

Build the solution:

```bash
dotnet build DevMemory.slnx
```

Run tests:

```bash
dotnet test DevMemory.slnx
```

Run the CLI from source:

```bash
dotnet run --project src/DevMemory.Cli -- help
```

Package the CLI:

```bash
dotnet pack src/DevMemory.Cli/DevMemory.Cli.csproj -c Release -o artifacts/packages
```

Update the locally installed global tool:

```bash
dotnet tool update --global DevMemory.Cli --add-source ./artifacts/packages
```

For the recommended local workflow, prefer using the scripts under `scripts/`.

---

## Current limitations

DevMemory is still under active development.

Current limitations:

- CLI parsing is currently implemented manually.
- Storage is JSON-based.
- No SQLite storage yet.
- No LLM integration yet.
- No automatic semantic summary of Git diffs yet.
- HTML graph layout is simple and static.
- No published public NuGet package yet.
- No CI/CD pipeline yet.
- No automated release workflow yet.

---

## Roadmap

Planned improvements:

- Improve CLI output with tables and cleaner formatting.
- Add better command parsing with `System.CommandLine` or `Spectre.Console`.
- Add Git diff summaries for `learn-from-git`.
- Add optional SQLite storage.
- Add optional AI provider integration.
- Improve HTML graph layout and filtering.
- Add screenshots to the README.
- Add release scripts and version bump workflow.
- Add `CHANGELOG.md`.
- Publish as a public or private NuGet tool package.
- Add CI pipeline for build and tests.

---

## Privacy

DevMemory is local-first.

By default:

- no cloud sync
- no external API calls
- no LLM provider required
- no data leaves the machine

All memories are stored locally under:

```text
~/.devmemory
```

or under the directory configured with:

```bash
DEVMEMORY_HOME
```

---

## Project goal

DevMemory aims to become a personal engineering knowledge base for developers who want to preserve context across tasks, branches, code reviews and AI-assisted development sessions.

The long-term goal is to provide a local, searchable and extensible memory layer for everyday software engineering work.

---

## License

License not defined yet.
