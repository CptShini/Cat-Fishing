#!/usr/bin/env bash
set -euo pipefail

VERSION_FILE="version.txt"

if [[ ! -f "$VERSION_FILE" ]]; then
  echo "::error::Missing version.txt at the repository root. Add it before merging into pre-release."
  exit 1
fi

version="$(cat "$VERSION_FILE")"
semver_re='^(0|[1-9][0-9]*)\.(0|[1-9][0-9]*)\.(0|[1-9][0-9]*)$'
if [[ ! "$version" =~ $semver_re ]]; then
  echo "::error::version.txt must contain a stable semantic version like 0.1.5 (without the v prefix)."
  exit 1
fi

tag="v$version"

if [[ -z "${PR_BASE_SHA:-}" ]]; then
  echo "::error::PR_BASE_SHA was not provided."
  exit 1
fi

if git cat-file -e "${PR_BASE_SHA}:$VERSION_FILE" 2>/dev/null; then
  base_version="$(git show "${PR_BASE_SHA}:$VERSION_FILE")"
  if [[ ! "$base_version" =~ $semver_re ]]; then
    echo "::error::The base branch's version.txt is invalid: '$base_version'. Fix it before merging."
    exit 1
  fi

  version_is_greater() {
    local major minor patch base_major base_minor base_patch
    IFS=. read -r major minor patch <<< "$1"
    IFS=. read -r base_major base_minor base_patch <<< "$2"

    if (( 10#$major != 10#$base_major )); then
      (( 10#$major > 10#$base_major ))
      return
    fi
    if (( 10#$minor != 10#$base_minor )); then
      (( 10#$minor > 10#$base_minor ))
      return
    fi
    (( 10#$patch > 10#$base_patch ))
  }

  if ! version_is_greater "$version" "$base_version"; then
    echo "::error::Bump version.txt to a version greater than the base branch version ($base_version). Candidate: $version."
    exit 1
  fi
else
  echo "::notice::The base branch has no version.txt yet; skipping the comparison for initial setup."
fi

set +e
git ls-remote --exit-code --refs origin "refs/tags/$tag" >/dev/null 2>&1
remote_status=$?
set -e
case "$remote_status" in
  0)
    echo "::error::Tag $tag already exists. Choose a new version in version.txt."
    exit 1
    ;;
  2)
    ;;
  *)
    echo "::error::Could not check whether tag $tag already exists on origin."
    exit "$remote_status"
    ;;
esac

echo "Version $version is valid and tag $tag is not in use."
