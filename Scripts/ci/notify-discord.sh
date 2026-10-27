#!/usr/bin/env bash
set -euo pipefail

mode="${1:-}"
[[ "$mode" == prerelease || "$mode" == release ]] || { echo "Usage: $0 prerelease|release" >&2; exit 2; }

if [[ -z "${DISCORD_WEBHOOK_URL:-}" ]]; then
  echo "::warning::Discord webhook secret is empty; skipping notification."
  exit 0
fi

: "${VERSION_TAG:?VERSION_TAG is required}"
: "${RUN_URL:?RUN_URL is required}"

if [[ "$mode" == prerelease ]]; then
  : "${ARTIFACT_URL:?ARTIFACT_URL is required}"
  commit="${VERSION_SHA:-${GITHUB_SHA:-}}"
  commit="${commit:0:7}"
  content="$(jq -n --arg tag "$VERSION_TAG" --arg url "$ARTIFACT_URL" --arg run "$RUN_URL" --arg commit "$commit" \
    '{content: ("**CatFishing pre-release " + $tag + " is ready!**\n\n" + "[Download build](" + $url + ")\n" + "[View workflow logs](" + $run + ")\n" + "Commit: `" + $commit + "`")}')"
else
  : "${RELEASE_URL:?RELEASE_URL is required}"
  content="$(jq -n --arg tag "$VERSION_TAG" --arg url "$RELEASE_URL" --arg run "$RUN_URL" \
    '{content: ("**CatFishing " + $tag + " has been released!**\n\n" + "[Download release](" + $url + ")\n" + "[View workflow logs](" + $run + ")")}')"
fi

curl --fail-with-body --silent --show-error \
  -H 'Content-Type: application/json' \
  -d "$content" \
  "$DISCORD_WEBHOOK_URL"
