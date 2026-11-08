#!/usr/bin/env bash
set -euo pipefail

mode="${1:-}"
if [[ "$mode" != prerelease && "$mode" != release ]]; then
  echo "Usage: $0 prerelease|release" >&2
  exit 2
fi

: "${VERSION_TAG:?VERSION_TAG must be a tag such as v0.1.7}"
if [[ ! "$VERSION_TAG" =~ ^v(0|[1-9][0-9]*)\.(0|[1-9][0-9]*)\.(0|[1-9][0-9]*)$ ]]; then
  echo "::error::Invalid VERSION_TAG: $VERSION_TAG"
  exit 1
fi

build_dir='build/StandaloneWindows64'
dist_dir='dist'
app_name="CatFishing-Windows-${VERSION_TAG}"

if [[ ! -f "$build_dir/StandaloneWindows64.exe" ]]; then
  echo "::error::Unity executable missing: $build_dir/StandaloneWindows64.exe"
  exit 1
fi
if [[ ! -d "$build_dir/StandaloneWindows64_Data" ]]; then
  echo "::error::Unity data directory missing: $build_dir/StandaloneWindows64_Data"
  exit 1
fi

rm -rf "$dist_dir"
mkdir -p "$dist_dir"
cp -a "$build_dir/." "$dist_dir/"
mv "$dist_dir/StandaloneWindows64.exe" "$dist_dir/${app_name}.exe"
mv "$dist_dir/StandaloneWindows64_Data" "$dist_dir/${app_name}_Data"
echo "Prepared ${app_name}.exe"

if [[ "$mode" == release ]]; then
  release_dir='release-assets'
  zip_name="CatFishing-${VERSION_TAG}-Windows.zip"
  rm -rf "$release_dir"
  mkdir -p "$release_dir/$app_name"
  cp -a "$dist_dir/." "$release_dir/$app_name/"
  (cd "$release_dir" && zip -qr "$zip_name" "$app_name")
  if [[ ! -s "$release_dir/$zip_name" ]]; then
    echo '::error::Release ZIP was not created.'
    exit 1
  fi
  echo "Created $release_dir/$zip_name"
fi
