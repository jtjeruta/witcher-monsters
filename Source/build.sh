#!/usr/bin/env bash
# Builds Assemblies/WitcherMonsters.dll via dotnet SDK.
# Harmony is provided at runtime by HugsLib / the game — do not ship 0Harmony.dll.
# Set RIMWORLD_MANAGED to override the default game Managed folder path.

set -euo pipefail

REPO_ROOT="$(cd "$(dirname "$0")/.." && pwd)"
MANAGED="${RIMWORLD_MANAGED:-/Users/tristan/Library/Application Support/Steam/steamapps/common/RimWorld/RimWorldMac.app/Contents/Resources/Data/Managed}"
DOTNET="${DOTNET:-/usr/local/share/dotnet/dotnet}"

if [[ ! -d "$MANAGED" ]]; then
  echo "ERROR: RimWorld Managed folder not found at: $MANAGED" >&2
  exit 1
fi

OUT_DIR="$REPO_ROOT/Assemblies"
mkdir -p "$OUT_DIR"

echo "Building WitcherMonsters.dll..."
"$DOTNET" build "$REPO_ROOT/Source/WitcherMonsters.csproj" \
  -c Release \
  -p:GamePath="$MANAGED" \
  --nologo \
  -v:q

rm -f "$OUT_DIR/0Harmony.dll"

echo "Built: $OUT_DIR/WitcherMonsters.dll"
ls -lh "$OUT_DIR/WitcherMonsters.dll"
