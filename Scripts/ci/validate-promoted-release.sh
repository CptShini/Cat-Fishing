#!/usr/bin/env bash
set -euo pipefail

semver_re='^v(0|[1-9][0-9]*)\.(0|[1-9][0-9]*)\.(0|[1-9][0-9]*)$'
fail() { echo "::error::$*" >&2; exit 1; }

event_name="${GITHUB_EVENT_NAME:-push}"
git fetch --force --tags origin
git fetch --force --no-tags origin \
  '+refs/heads/main:refs/remotes/origin/main' \
  '+refs/heads/pre-release:refs/remotes/origin/pre-release'

find_tag_for_commit() {
  local target_sha="$1" candidate found=""
  while IFS= read -r candidate; do
    [[ "$candidate" =~ $semver_re ]] || continue
    [[ "$(git rev-parse "${candidate}^{commit}")" == "$target_sha" ]] || continue
    [[ -z "$found" ]] || fail "Multiple semantic-version tags point to $target_sha."
    found="$candidate"
  done < <(git tag --list 'v*')

  [[ -n "$found" ]] || fail "No stable semantic-version tag points to $target_sha. If the latest pre-release merge used version:skip, create a normal tagged pre-release before promoting to main."
  printf '%s\n' "$found"
}

if [[ "$event_name" == pull_request_target ]]; then
  [[ "${GITHUB_REF:-}" == refs/heads/main && "${BASE_REF:-}" == main ]] \
    || fail 'Release validation only applies to PRs targeting main.'
  [[ "${PR_HEAD_REF:-}" == pre-release ]] \
    || fail 'PRs into main must come from pre-release.'
  [[ "${PR_HEAD_REPOSITORY:-}" == "${TARGET_REPOSITORY:-}" ]] \
    || fail 'Release PRs must originate from a branch in this repository.'
  pre_release_tip="$(git rev-parse 'refs/remotes/origin/pre-release^{commit}')"
  [[ "$PR_HEAD_SHA" == "$pre_release_tip" ]] \
    || fail 'The promotion PR must target the current pre-release tip. Update the PR and rerun validation.'

  tag="$(find_tag_for_commit "$PR_HEAD_SHA")"
  echo "Validated promotion PR: $tag points to the current pre-release tip."
  echo "tag=$tag" >> "$GITHUB_OUTPUT"
  echo 'publish=false' >> "$GITHUB_OUTPUT"
  exit 0
fi

[[ "$event_name" == push ]] || fail "Unsupported event: $event_name"
[[ "${GITHUB_REF:-}" == refs/heads/main ]] || fail 'Run this workflow on pushes to main.'
: "${GITHUB_SHA:?GITHUB_SHA must be set to the pushed main commit}"

PROMOTION_SHA="$GITHUB_SHA"
merge_line="$(git rev-list --parents -n 1 "$PROMOTION_SHA")"
read -r _ main_parent promoted_sha extra <<< "$merge_line"
[[ -n "${main_parent:-}" && -n "${promoted_sha:-}" && -z "${extra:-}" ]] \
  || fail 'The pushed main commit is not a regular two-parent merge commit. Promote pre-release to main using Create a merge commit.'
git merge-base --is-ancestor "$promoted_sha" refs/remotes/origin/pre-release \
  || fail 'The merge commit’s second parent is not in pre-release history.'
git merge-base --is-ancestor "$PROMOTION_SHA" refs/remotes/origin/main \
  || fail 'The main merge commit is not in main history.'

tagged_tree="$(git rev-parse "${promoted_sha}^{tree}")"
promoted_tree="$(git rev-parse "${PROMOTION_SHA}^{tree}")"
[[ "$tagged_tree" == "$promoted_tree" ]] \
  || fail 'Main differs from the tested pre-release tree. Re-sync and retest before publishing.'

tag="$(find_tag_for_commit "$promoted_sha")"
if GH_TOKEN="${GH_TOKEN:-${GITHUB_TOKEN:-}}" gh release view "$tag" --repo "$GITHUB_REPOSITORY" >/dev/null 2>&1; then
  echo "GitHub Release $tag already exists; nothing to publish."
  publish=false
else
  echo "Validated $tag; ready to publish."
  publish=true
fi

echo "tag=$tag" >> "$GITHUB_OUTPUT"
echo "publish=$publish" >> "$GITHUB_OUTPUT"
