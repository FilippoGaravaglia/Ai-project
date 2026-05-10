#!/usr/bin/env bash
set -euo pipefail

ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
SMOKE_HOME="$(mktemp -d)"

cleanup() {
  rm -rf "$SMOKE_HOME"
}

trap cleanup EXIT

cd "$ROOT_DIR"

echo "Running DevMemory smoke test..."
echo "Using isolated DEVMEMORY_HOME: $SMOKE_HOME"
echo

export DEVMEMORY_HOME="$SMOKE_HOME"

if ! command -v devmemory >/dev/null 2>&1; then
  echo "The 'devmemory' command was not found."
  echo "Install it first with:"
  echo "  ./scripts/install-local-tool.sh"
  exit 1
fi

echo "Checking version..."
devmemory version

echo
echo "Checking help..."
devmemory help >/dev/null
devmemory --help >/dev/null
devmemory -h >/dev/null

echo "Checking version aliases..."
devmemory --version >/dev/null
devmemory -v >/dev/null

echo "Checking storage paths..."
devmemory storage
devmemory markdown

echo
echo "Checking empty list..."
devmemory list

echo
echo "Checking empty search..."
devmemory search revision

echo
echo "Creating memory through interactive command..."
printf '%s\n' \
  'Smoke test memory' \
  'DevMemory' \
  'CLI' \
  'smoke-test-branch' \
  'dotnet, cli, smoke-test' \
  'Verify installed CLI behavior end-to-end.' \
  'Run smoke tests against an isolated local storage directory.' \
  'Keep smoke tests isolated from real user data.' \
  '' \
  'scripts/smoke-test.sh' \
  '' \
  'Added smoke test script.' \
  '' \
  'Smoke testing improves confidence before release.' \
  | devmemory add >/dev/null

echo "Checking populated list..."
devmemory list

echo
echo "Checking search..."
devmemory search smoke-test

MEMORY_ID="$(python3 - "$SMOKE_HOME/devmemory.json" <<'PY'
import json
import sys

path = sys.argv[1]

with open(path, "r", encoding="utf-8") as file:
    data = json.load(file)

if isinstance(data, list):
    memories = data
elif isinstance(data, dict) and "memories" in data:
    memories = data["memories"]
else:
    raise SystemExit("Unsupported storage format.")

if not memories:
    raise SystemExit("No memories found in smoke storage.")

memory = memories[0]

for key in ("id", "Id", "ID"):
    if key in memory:
        print(memory[key])
        raise SystemExit(0)

raise SystemExit("Memory id not found.")
PY
)"

echo
echo "Checking show..."
devmemory show "$MEMORY_ID"

echo
echo "Checking graph export..."
devmemory graph-export

echo
echo "Checking graph view..."
devmemory graph-view

echo
echo "Validating generated files..."

if [ ! -f "$SMOKE_HOME/devmemory.json" ]; then
  echo "Missing storage file."
  exit 1
fi

if [ ! -d "$SMOKE_HOME/markdown" ]; then
  echo "Missing markdown directory."
  exit 1
fi

if [ ! -d "$SMOKE_HOME/graph" ]; then
  echo "Missing graph directory."
  exit 1
fi

if [ ! -f "$SMOKE_HOME/graph/devmemory-graph.json" ]; then
  echo "Missing graph JSON file."
  exit 1
fi

if [ ! -f "$SMOKE_HOME/graph/devmemory-graph.html" ]; then
  echo "Missing graph HTML file."
  exit 1
fi

echo
echo "Smoke test completed successfully."