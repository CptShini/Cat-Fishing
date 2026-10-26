#!/usr/bin/env bash
set -euo pipefail

if [[ "$GITHUB_REF" != "refs/heads/main" ]]; then
  echo "::error::Run this workflow from main. Selected ref: $GITHUB_REF"
  exit 1
fi

VERSION_FILE="version.txt"
if [[ ! -f "$VERSION_FILE" ]]; then
  echo "::error::Missing version.txt at the repository root."
  exit 1
fi

version="$(cat "$VERSION_FILE")"
if [[ ! "$version" =~ ^(0|[1-9][0-9]*)\.(0|[1-9][0-9]*)\.(0|[1-9][0-9]*)$ ]]; then
  echo "::error::version.txt must contain a stable semantic version like 0.1.5 (without the v prefix)."
  exit 1
fi

tag="v$version"
git fetch --force --tags origin

if ! git rev-parse --verify --quiet "refs/tags/$tag^{commit}" >/dev/null; then
  echo "::error::Tag $tag does not exist. This version must first be tagged by the pre-release workflow."
  exit 1
fi

tag_sha="$(git rev-parse "refs/tags/$tag^{commit}")"
if ! git merge-base --is-ancestor "$tag_sha" "$GITHUB_SHA"; then
  echo "::error::Tag $tag is not reachable from main. Promote the tagged pre-release commit using a merge that preserves its commit identity; rebase/squash promotion rewrites it."
  exit 1
fi

tagged_version="$(git show "${tag_sha}:${VERSION_FILE}")"
if [[ "$tagged_version" != "$version" ]]; then
  echo "::error::Tag $tag points to a commit whose version.txt is '$tagged_version', not '$version'."
  exit 1
fi

if GH_TOKEN="${GH_TOKEN:-${GITHUB_TOKEN:-}}" gh release view "$tag" --repo "$GITHUB_REPOSITORY" >/dev/null 2>&1; then
  echo "GitHub Release $tag already exists; nothing to publish."
  publish=false
else
  echo "Validated $tag at $tag_sha; ready to publish."
  publish=true
fi

echo "tag=$tag" >> "$GITHUB_OUTPUT"
echo "sha=$tag_sha" >> "$GITHUB_OUTPUT"
echo "publish=$publish" >> "$GITHUB_OUTPUT"
