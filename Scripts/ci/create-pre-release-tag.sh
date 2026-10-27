#!/usr/bin/env bash
set -euo pipefail

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
sha="$(git rev-parse "${GITHUB_SHA}^{commit}")"

set +e
remote_ref="$(git ls-remote --exit-code --refs origin "refs/tags/$tag" 2>&1)"
remote_status=$?
set -e
case "$remote_status" in
  0)
    git fetch --force --no-tags origin "refs/tags/$tag:refs/tags/$tag"
    existing_sha="$(git rev-parse "refs/tags/$tag^{commit}")"
    if [[ "$existing_sha" != "$sha" ]]; then
      echo "::error::Tag $tag already points to $existing_sha, not this commit ($sha). Update version.txt before merging another pre-release."
      exit 1
    fi
    echo "Tag $tag already points to this commit; continuing to support safe workflow reruns."
    ;;
  2)
    git config user.name "github-actions[bot]"
    git config user.email "41898282+github-actions[bot]@users.noreply.github.com"
    git tag -a "$tag" "$sha" -m "CatFishing pre-release $tag"
    if ! git push origin "refs/tags/$tag"; then
      git fetch --force --no-tags origin "refs/tags/$tag:refs/tags/$tag" || {
        echo "::error::Failed to push tag $tag, and it could not be fetched for verification."
        exit 1
      }
      existing_sha="$(git rev-parse "refs/tags/$tag^{commit}")"
      if [[ "$existing_sha" != "$sha" ]]; then
        echo "::error::Tag $tag was created concurrently for a different commit ($existing_sha)."
        exit 1
      fi
    fi
    ;;
  *)
    echo "::error::Could not check whether tag $tag exists: $remote_ref"
    exit "$remote_status"
    ;;
esac

echo "Created/verified pre-release tag $tag at $sha."
echo "version=$version" >> "$GITHUB_OUTPUT"
echo "tag=$tag" >> "$GITHUB_OUTPUT"
echo "sha=$sha" >> "$GITHUB_OUTPUT"
