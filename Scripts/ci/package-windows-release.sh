#!/usr/bin/env bash
set -euo pipefail

: "${GITHUB_REF_NAME:?GITHUB_REF_NAME must be set}"

ARTIFACT_DIR="release-assets"
APP_NAME="CatFishing-Windows-${GITHUB_REF_NAME}"
PACKAGE_DIR="${ARTIFACT_DIR}/${APP_NAME}"
ZIP_NAME="CatFishing-${GITHUB_REF_NAME}-Windows.zip"

mkdir -p "$PACKAGE_DIR"
find "$ARTIFACT_DIR" -mindepth 1 -maxdepth 1 \
  ! -name "$APP_NAME" \
  -exec mv -t "$PACKAGE_DIR" -- {} +

test -f "$PACKAGE_DIR/${APP_NAME}.exe"
test -d "$PACKAGE_DIR/${APP_NAME}_Data"

(
  cd "$ARTIFACT_DIR"
  zip -qr "$ZIP_NAME" "$APP_NAME"
)

test -s "${ARTIFACT_DIR}/${ZIP_NAME}"
echo "Created ${ARTIFACT_DIR}/${ZIP_NAME}"
