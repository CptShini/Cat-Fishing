#!/usr/bin/env bash
set -euo pipefail

: "${VERSION_TAG:?VERSION_TAG must be set to the Git tag, such as v0.1.5}"
if [[ ! "$VERSION_TAG" =~ ^v(0|[1-9][0-9]*)\.(0|[1-9][0-9]*)\.(0|[1-9][0-9]*)$ ]]; then
  echo "::error::Invalid VERSION_TAG: $VERSION_TAG"
  exit 1
fi

APP_NAME="CatFishing-Windows-${VERSION_TAG}"
DIST_DIR="dist"
RELEASE_DIR="release-assets"
PACKAGE_DIR="${RELEASE_DIR}/${APP_NAME}"
ZIP_NAME="CatFishing-${VERSION_TAG}-Windows.zip"

[[ -f "${DIST_DIR}/${APP_NAME}.exe" ]] || { echo "::error::Missing ${DIST_DIR}/${APP_NAME}.exe"; exit 1; }
[[ -d "${DIST_DIR}/${APP_NAME}_Data" ]] || { echo "::error::Missing ${DIST_DIR}/${APP_NAME}_Data"; exit 1; }

rm -rf "$RELEASE_DIR"
mkdir -p "$PACKAGE_DIR"
cp -a "$DIST_DIR/." "$PACKAGE_DIR/"

(
  cd "$RELEASE_DIR"
  zip -qr "$ZIP_NAME" "$APP_NAME"
)

test -s "${RELEASE_DIR}/${ZIP_NAME}"
echo "Created ${RELEASE_DIR}/${ZIP_NAME}"
