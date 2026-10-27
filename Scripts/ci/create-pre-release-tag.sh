#!/usr/bin/env bash
set -euo pipefail

semver_re='^v(0|[1-9][0-9]*)\.(0|[1-9][0-9]*)\.(0|[1-9][0-9]*)$'
fail() { echo "::error::$*" >&2; exit 1; }

: "${TARGET_SHA:?TARGET_SHA must be the merged PR commit SHA}"
: "${BASE_REF:?BASE_REF is required}"
: "${HEAD_REF:?HEAD_REF is required}"
: "${HEAD_REPOSITORY:?HEAD_REPOSITORY is required}"
: "${TARGET_REPOSITORY:?TARGET_REPOSITORY is required}"

labels_json="${PR_LABELS_JSON:-[]}"
jq -e 'type == "array"' <<< "$labels_json" >/dev/null || fail 'PR_LABELS_JSON must be a JSON array.'
mapfile -t version_labels < <(jq -r '.[] | .name | select(startswith("version:"))' <<< "$labels_json")
(( ${#version_labels[@]} <= 1 )) || fail 'Use at most one version:* label.'

major_bump=false
minor_bump=false
case "${version_labels[0]:-}" in
  '') ;;
  version:major) major_bump=true ;;
  version:minor) minor_bump=true ;;
  version:skip) fail 'version:skip must not run tag creation.' ;;
  *) fail "Unsupported version label '${version_labels[0]}'. Use version:major, version:minor, or version:skip; no label means patch." ;;
esac

[[ "$BASE_REF" == pre-release ]] || fail 'Tags can only be created for PRs merged into pre-release.'
[[ "$HEAD_REPOSITORY" == "$TARGET_REPOSITORY" ]] || fail 'PRs must originate from this repository.'
[[ "$HEAD_REF" =~ ^develop/.+ ]] || fail 'PRs into pre-release must come from develop/<name>.'
[[ "$TARGET_SHA" =~ ^[0-9a-fA-F]{40,64}$ ]] || fail "Invalid merged commit SHA: $TARGET_SHA"

# Tag allocation is serialized by the workflow. Only use version tags on this commit's history.
git fetch --force --no-tags origin '+refs/heads/pre-release:refs/remotes/origin/pre-release'
git fetch --force --tags origin

git cat-file -e "${TARGET_SHA}^{commit}" 2>/dev/null || fail "Merged commit $TARGET_SHA is unavailable."
git merge-base --is-ancestor "$TARGET_SHA" refs/remotes/origin/pre-release || fail "Merged commit $TARGET_SHA is not on pre-release."

# Reuse the tag on this exact commit when a workflow is rerun.
matching_tag=""
while IFS= read -r candidate; do
  [[ "$candidate" =~ $semver_re ]] || continue
  [[ "$(git rev-parse "${candidate}^{commit}")" == "$TARGET_SHA" ]] || continue
  [[ -z "$matching_tag" ]] || fail "Multiple version tags point to $TARGET_SHA."
  matching_tag="$candidate"
done < <(git tag --list 'v*')

if [[ -n "$matching_tag" ]]; then
  tag="$matching_tag"
  echo "Reusing existing tag $tag for this commit."
else
  latest_tag="$(git tag --merged "$TARGET_SHA" --list 'v*' | grep -E "$semver_re" | LC_ALL=C sort -V | tail -n 1 || true)"
  latest_version="${latest_tag#v}"
  [[ -n "$latest_tag" ]] || latest_version='0.0.0'
  IFS=. read -r major minor patch <<< "$latest_version"

  if [[ "$major_bump" == true ]]; then
    major=$((10#$major + 1))
    minor=0
    patch=0
    bump=major
  elif [[ "$minor_bump" == true ]]; then
    minor=$((10#$minor + 1))
    patch=0
    bump=minor
  else
    patch=$((10#$patch + 1))
    bump=patch
  fi

  tag="v${major}.${minor}.${patch}"
  if git show-ref --verify --quiet "refs/tags/$tag"; then
    fail "Tag $tag already exists outside this commit's history. Review tag/workflow ordering before retrying."
  fi

  git config user.name 'github-actions[bot]'
  git config user.email '41898282+github-actions[bot]@users.noreply.github.com'
  git tag -a "$tag" "$TARGET_SHA" -m "CatFishing pre-release $tag"

  if ! git push origin "refs/tags/$tag"; then
    # Accept a competing creation only when the remote tag points at the same commit.
    if git fetch --force --no-tags origin "refs/tags/$tag:refs/tags/$tag" 2>/dev/null &&
      [[ "$(git rev-parse "${tag}^{commit}")" == "$TARGET_SHA" ]]; then
      echo "Verified existing tag $tag points to this commit."
    else
      fail "Could not create $tag. Check the tag ruleset permits creation by GITHUB_TOKEN."
    fi
  fi

  echo "Created $tag ($bump bump) at $TARGET_SHA."
fi

echo "tag=$tag" >> "$GITHUB_OUTPUT"
echo "sha=$TARGET_SHA" >> "$GITHUB_OUTPUT"
