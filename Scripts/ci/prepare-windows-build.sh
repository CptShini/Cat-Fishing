#!/usr/bin/env bash
set -euo pipefail

BUILD_DIR="build/StandaloneWindows64"
DIST_DIR="dist"

test -f "$BUILD_DIR/StandaloneWindows64.exe"
test -d "$BUILD_DIR/StandaloneWindows64_Data"

if [[ "$GITHUB_REF" == refs/tags/v* ]]; then
  APP_NAME="CatFishing-Windows-${GITHUB_REF_NAME}"
else
  APP_NAME="CatFishing-Windows"
fi

rm -rf "$DIST_DIR"
mkdir -p "$DIST_DIR"
cp -a "$BUILD_DIR/." "$DIST_DIR/"
mv "$DIST_DIR/StandaloneWindows64.exe" "$DIST_DIR/${APP_NAME}.exe"
mv "$DIST_DIR/StandaloneWindows64_Data" "$DIST_DIR/${APP_NAME}_Data"

echo "Prepared ${APP_NAME}.exe"
