#!/usr/bin/env bash
set -euo pipefail

semver_re='^v(0|[1-9][0-9]*)\.(0|[1-9][0-9]*)\.(0|[1-9][0-9]*)$'
fail() { echo "::error::$*" >&2; exit 1; }

[[ "${GITHUB_REF:-}" == refs/heads/main ]] || fail 'Run this workflow from main.'
: "${GITHUB_SHA:?GITHUB_SHA must be set to the pushed main commit}"

git fetch --force --tags origin
git fetch --force --no-tags origin \
  '+refs/heads/main:refs/remotes/origin/main' \
  '+refs/heads/pre-release:refs/remotes/origin/pre-release'

PROMOTION_SHA="$GITHUB_SHA"
merge_line="$(git rev-list --parents -n 1 "$PROMOTION_SHA")"
read -r merge_sha main_parent promoted_sha extra <<< "$merge_line"
[[ "$merge_sha" == "$PROMOTION_SHA" && -n "${main_parent:-}" && -n "${promoted_sha:-}" && -z "${extra:-}" ]] \
  || fail 'The pushed main commit is not a regular two-parent merge commit. Promote pre-release to main using Create a merge commit.'
PROMOTED_SHA="$promoted_sha"

git cat-file -e "${PROMOTED_SHA}^{commit}" 2>/dev/null || fail 'The pre-release merge parent is unavailable.'
git merge-base --is-ancestor "$PROMOTED_SHA" refs/remotes/origin/pre-release \
  || fail 'The merge commit’s second parent is not in pre-release history.'
git merge-base --is-ancestor "$PROMOTION_SHA" refs/remotes/origin/main \
  || fail 'The main merge commit is not in main history.'

tagged_tree="$(git rev-parse "${PROMOTED_SHA}^{tree}")"
promoted_tree="$(git rev-parse "${PROMOTION_SHA}^{tree}")"
[[ "$tagged_tree" == "$promoted_tree" ]] \
  || fail 'Main differs from the tested pre-release tree. Re-sync and retest before publishing.'

tag=""
while IFS= read -r candidate; do
  [[ "$candidate" =~ $semver_re ]] || continue
  [[ "$(git rev-parse "${candidate}^{commit}")" == "$PROMOTED_SHA" ]] || continue
  [[ -z "$tag" ]] || fail 'Multiple version tags point to the promoted commit.'
  tag="$candidate"
done < <(git tag --list 'v*')
[[ -n "$tag" ]] || fail 'No semantic-version tag points to the tested pre-release commit.'

if GH_TOKEN="${GH_TOKEN:-${GITHUB_TOKEN:-}}" gh release view "$tag" --repo "$GITHUB_REPOSITORY" >/dev/null 2>&1; then
  echo "GitHub Release $tag already exists; nothing to publish."
  publish=false
else
  echo "Validated $tag; ready to publish."
  publish=true
fi

echo "tag=$tag" >> "$GITHUB_OUTPUT"
echo "publish=$publish" >> "$GITHUB_OUTPUT"
