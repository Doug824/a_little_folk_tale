#!/bin/bash

# Quick Review - Fast code review for current changes

set -e

# Color codes
YELLOW='\033[1;33m'
GREEN='\033[0;32m'
RED='\033[0;31m'
NC='\033[0m'

echo -e "${YELLOW}Quick Code Review${NC}"

# Get current changes
if [[ -n $(git status --porcelain) ]]; then
    DIFF=$(git diff HEAD)
    if [[ -z "$DIFF" ]]; then
        DIFF=$(git diff)
    fi
else
    DIFF=$(git diff HEAD~1 HEAD)
fi

if [[ -z "$DIFF" ]]; then
    echo -e "${RED}No changes to review!${NC}"
    exit 1
fi

# Create focused review request
REVIEW="Please quickly review these changes and report only:
1. Critical security issues
2. Obvious bugs
3. Major performance problems

\`\`\`diff
$DIFF
\`\`\`

Be concise - only report actual problems that need immediate attention."

echo "$REVIEW" | claude --no-project --no-memory --print