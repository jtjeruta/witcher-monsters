#!/usr/bin/env bash
# Builds Assemblies/WitcherMonsters.dll (and copies 0Harmony.dll) via dotnet SDK.
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

HARMONY_SRC="$HOME/.nuget/packages/lib.harmony/2.3.3/lib/net472/0Harmony.dll"
if [[ -f "$HARMONY_SRC" && ! -f "$OUT_DIR/0Harmony.dll" ]]; then
  cp "$HARMONY_SRC" "$OUT_DIR/0Harmony.dll"
fi

echo "Built: $OUT_DIR/WitcherMonsters.dll"
ls -lh "$OUT_DIR/WitcherMonsters.dll"
if [[ -f "$OUT_DIR/0Harmony.dll" ]]; then
  ls -lh "$OUT_DIR/0Harmony.dll"
fi
