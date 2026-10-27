#!/usr/bin/env bash
set -euo pipefail

fail() { echo "::error::$*" >&2; exit 1; }
semver='^v(0|[1-9][0-9]*)\.(0|[1-9][0-9]*)\.(0|[1-9][0-9]*)$'

git fetch --force --tags origin
git fetch --no-tags origin '+refs/heads/pre-release:refs/remotes/origin/pre-release'

tag_for() {
  local tags
  mapfile -t tags < <(git tag --points-at "$1" --list 'v*' | grep -E "$semver" || true)
  ((${#tags[@]} == 1)) || fail "Expected one semantic-version tag on $1; found ${#tags[@]}"
  printf '%s\n' "${tags[0]}"
}

if [[ "${GITHUB_EVENT_NAME:-push}" == pull_request_target ]]; then
  [[ "${BASE_REF:-}" == main && "${PR_HEAD_REF:-}" == pre-release ]] || fail 'Only pre-release can be promoted to main.'
  [[ "$PR_HEAD_SHA" == "$(git rev-parse refs/remotes/origin/pre-release)" ]] || fail 'Update the PR to the latest pre-release commit.'
  tag="$(tag_for "$PR_HEAD_SHA")"
  echo "tag=$tag" >> "$GITHUB_OUTPUT"
  echo 'publish=false' >> "$GITHUB_OUTPUT"
  exit 0
fi

[[ "${GITHUB_EVENT_NAME:-push}" == push && "${GITHUB_REF:-}" == refs/heads/main ]] || fail 'Expected a push to main.'
read -r merge_commit first_parent promoted_sha extra <<< "$(git rev-list --parents -n 1 "$GITHUB_SHA")"
[[ -n "${promoted_sha:-}" && -z "${extra:-}" ]] || fail 'Promote with Create a merge commit.'
git merge-base --is-ancestor "$promoted_sha" refs/remotes/origin/pre-release || fail 'The merge must include pre-release history.'
[[ "$(git rev-parse "${promoted_sha}^{tree}")" == "$(git rev-parse "${GITHUB_SHA}^{tree}")" ]] || fail 'The main merge result differs from the tested pre-release tree.'

tag="$(tag_for "$promoted_sha")"
if gh release view "$tag" --repo "$GITHUB_REPOSITORY" >/dev/null 2>&1; then publish=false; else publish=true; fi
echo "tag=$tag" >> "$GITHUB_OUTPUT"
echo "publish=$publish" >> "$GITHUB_OUTPUT"
