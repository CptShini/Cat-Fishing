#!/usr/bin/env bash
set -euo pipefail

MODE="${1:-}"
: "${DISCORD_WEBHOOK_URL:?DISCORD_WEBHOOK_URL must be set}"
: "${RUN_URL:?RUN_URL must be set}"

case "$MODE" in
  prerelease)
    : "${ARTIFACT_URL:?ARTIFACT_URL must be set for pre-release notifications}"
    COMMIT="${GITHUB_SHA:0:7}"
    PAYLOAD="$(jq -n \
      --arg url "$ARTIFACT_URL" \
      --arg run "$RUN_URL" \
      --arg commit "$COMMIT" \
      '{content: ("**CatFishing pre-release build is ready!**\n\n" +
        "[Download build](" + $url + ")\n" +
        "[View workflow logs](" + $run + ")\n" +
        "Commit: `" + $commit + "`")}')"
    ;;
  release)
    : "${RELEASE_URL:?RELEASE_URL must be set for release notifications}"
    : "${GITHUB_REF_NAME:?GITHUB_REF_NAME must be set}"
    PAYLOAD="$(jq -n \
      --arg tag "$GITHUB_REF_NAME" \
      --arg url "$RELEASE_URL" \
      --arg run "$RUN_URL" \
      '{content: ("**CatFishing " + $tag + " has been released!**\n\n" +
        "[Download release](" + $url + ")\n" +
        "[View workflow logs](" + $run + ")")}')"
    ;;
  *)
    echo "Usage: $0 {prerelease|release}" >&2
    exit 2
    ;;
esac

curl --fail-with-body --silent --show-error \
  -H 'Content-Type: application/json' \
  -d "$PAYLOAD" \
  "$DISCORD_WEBHOOK_URL"
