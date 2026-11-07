#!/usr/bin/env bash
set -euo pipefail

semver_re='^v(0|[1-9][0-9]*)\.(0|[1-9][0-9]*)\.(0|[1-9][0-9]*)$'
fail() { echo "::error::$*" >&2; exit 1; }
: "${TARGET_SHA:?TARGET_SHA is required}"

case "${VERSION_LABEL:-}" in
  version:major) bump=major ;;
  version:minor) bump=minor ;;
  *) bump=patch ;;
esac

git merge-base --is-ancestor "$TARGET_SHA" refs/remotes/origin/pre-release \
  || fail "Merged commit $TARGET_SHA is not on pre-release."

matching_tag=""
while IFS= read -r candidate; do
  [[ "$candidate" =~ $semver_re ]] || continue
  [[ "$(git rev-parse "${candidate}^{commit}")" == "$TARGET_SHA" ]] || continue
  [[ -z "$matching_tag" ]] || fail "Multiple version tags point to $TARGET_SHA."
  matching_tag="$candidate"
done < <(git tag --list 'v*')

if [[ -n "$matching_tag" ]]; then
  tag="$matching_tag"
else
  latest_tag="$(git tag --merged "$TARGET_SHA" --list 'v*' | grep -E "$semver_re" | LC_ALL=C sort -V | tail -n 1 || true)"
  latest_version="${latest_tag#v}"
  [[ -n "$latest_tag" ]] || latest_version='0.0.0'
  IFS=. read -r major minor patch <<< "$latest_version"

  case "$bump" in
    major) major=$((10#$major + 1)); minor=0; patch=0 ;;
    minor) minor=$((10#$minor + 1)); patch=0 ;;
    patch) patch=$((10#$patch + 1)) ;;
  esac
  tag="v${major}.${minor}.${patch}"
  git show-ref --verify --quiet "refs/tags/$tag" && fail "Tag $tag already exists on another commit."

  git config user.name 'github-actions[bot]'
  git config user.email '41898282+github-actions[bot]@users.noreply.github.com'
  git tag -a "$tag" "$TARGET_SHA" -m "CatFishing pre-release $tag"
  git push origin "refs/tags/$tag" || fail "Could not create $tag. Check the tag ruleset permits creation by GITHUB_TOKEN."
  echo "Created $tag ($bump bump)."
fi

echo "tag=$tag" >> "$GITHUB_OUTPUT"
echo "sha=$TARGET_SHA" >> "$GITHUB_OUTPUT"
