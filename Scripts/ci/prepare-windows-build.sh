#!/usr/bin/env bash
set -euo pipefail

: "${VERSION_TAG:?VERSION_TAG must be set to the Git tag, such as v0.1.5}"
if [[ ! "$VERSION_TAG" =~ ^v(0|[1-9][0-9]*)\.(0|[1-9][0-9]*)\.(0|[1-9][0-9]*)$ ]]; then
  echo "::error::Invalid VERSION_TAG: $VERSION_TAG"
  exit 1
fi

BUILD_DIR="build/StandaloneWindows64"
DIST_DIR="dist"
APP_NAME="CatFishing-Windows-${VERSION_TAG}"

if [[ ! -f "$BUILD_DIR/StandaloneWindows64.exe" ]]; then
  echo "::error::Unity executable not found at $BUILD_DIR/StandaloneWindows64.exe"
  exit 1
fi
if [[ ! -d "$BUILD_DIR/StandaloneWindows64_Data" ]]; then
  echo "::error::Unity data directory not found at $BUILD_DIR/StandaloneWindows64_Data"
  exit 1
fi

rm -rf "$DIST_DIR"
mkdir -p "$DIST_DIR"
cp -a "$BUILD_DIR/." "$DIST_DIR/"
mv "$DIST_DIR/StandaloneWindows64.exe" "$DIST_DIR/${APP_NAME}.exe"
mv "$DIST_DIR/StandaloneWindows64_Data" "$DIST_DIR/${APP_NAME}_Data"

echo "Prepared ${APP_NAME}.exe and ${APP_NAME}_Data/"
