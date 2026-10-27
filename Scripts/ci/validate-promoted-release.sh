#!/usr/bin/env bash
set -euo pipefail

semver_re='^v(0|[1-9][0-9]*)\.(0|[1-9][0-9]*)\.(0|[1-9][0-9]*)$'
fail() { echo "::error::$*" >&2; exit 1; }

[[ "${GITHUB_REF:-}" == refs/heads/main ]] || fail 'Run this workflow from main.'
git fetch --force --tags origin
git fetch --force --no-tags origin \
  '+refs/heads/main:refs/remotes/origin/main' \
  '+refs/heads/pre-release:refs/remotes/origin/pre-release'

: "${PROMOTED_SHA:?PROMOTED_SHA must be the pre-release PR head SHA}"
: "${PROMOTION_SHA:?PROMOTION_SHA must be the main merge commit SHA}"
[[ "${BASE_REF:-}" == main ]] || fail 'Only promotions into main can publish a release.'
[[ "$PROMOTED_SHA" =~ ^[0-9a-fA-F]{40,64}$ && "$PROMOTION_SHA" =~ ^[0-9a-fA-F]{40,64}$ ]] || fail 'Invalid promotion commit SHA.'
git cat-file -e "${PROMOTED_SHA}^{commit}" 2>/dev/null || fail 'Pre-release commit is unavailable.'
git cat-file -e "${PROMOTION_SHA}^{commit}" 2>/dev/null || fail 'Main merge commit is unavailable.'
git merge-base --is-ancestor "$PROMOTED_SHA" refs/remotes/origin/pre-release || fail 'The PR source commit is not in pre-release history.'
git merge-base --is-ancestor "$PROMOTION_SHA" refs/remotes/origin/main || fail 'The promotion merge commit is not in main history.'
git merge-base --is-ancestor "$PROMOTED_SHA" "$PROMOTION_SHA" || fail 'The tagged commit must be preserved in main history. Use a regular merge commit, not squash/rebase.'

tagged_tree="$(git rev-parse "${PROMOTED_SHA}^{tree}")"
promoted_tree="$(git rev-parse "${PROMOTION_SHA}^{tree}")"
[[ "$tagged_tree" == "$promoted_tree" ]] || fail 'Main differs from the tested pre-release tree. Re-sync and retest before publishing.'

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
