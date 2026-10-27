#!/usr/bin/env bash
set -euo pipefail

: "${BASE_REF:?BASE_REF is required}"
: "${HEAD_REF:?HEAD_REF is required}"
: "${HEAD_REPOSITORY:?HEAD_REPOSITORY is required}"
: "${TARGET_REPOSITORY:?TARGET_REPOSITORY is required}"

if [[ "$HEAD_REPOSITORY" != "$TARGET_REPOSITORY" ]]; then
  echo "::error::PRs must originate from a branch in this repository."
  exit 1
fi

case "$BASE_REF" in
  main)
    if [[ "$HEAD_REF" != "pre-release" ]]; then
      echo "::error::PRs into main must come from pre-release."
      exit 1
    fi
    ;;
  pre-release)
    if [[ ! "$HEAD_REF" =~ ^develop/.+ ]]; then
      echo "::error::PRs into pre-release must come from develop/<name>."
      exit 1
    fi
    ;;
  *)
    echo "::error::Unexpected target branch: $BASE_REF"
    exit 1
    ;;
esac

echo "PR source branch is allowed."
