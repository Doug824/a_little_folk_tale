#!/bin/bash

# Review PR - Review changes in current branch vs main

set -e

# Color codes
BLUE='\033[0;34m'
YELLOW='\033[1;33m'
GREEN='\033[0;32m'
RED='\033[0;31m'
NC='\033[0m'

echo -e "${BLUE}=== PR REVIEW TOOL ===${NC}"

# Get current branch
CURRENT_BRANCH=$(git branch --show-current)
BASE_BRANCH="${1:-main}"

echo "Reviewing changes: $CURRENT_BRANCH vs $BASE_BRANCH"
echo ""

# Get diff between branches
DIFF=$(git diff "$BASE_BRANCH"..."$CURRENT_BRANCH")
DIFF_STAT=$(git diff "$BASE_BRANCH"..."$CURRENT_BRANCH" --stat)

if [[ -z "$DIFF" ]]; then
    echo -e "${RED}No differences between $CURRENT_BRANCH and $BASE_BRANCH${NC}"
    exit 1
fi

echo -e "${GREEN}Changes summary:${NC}"
echo "$DIFF_STAT"
echo ""

# Get commit messages
echo -e "${YELLOW}Commits in this branch:${NC}"
git log --oneline "$BASE_BRANCH".."$CURRENT_BRANCH"
echo ""

# Create PR review request
REVIEW_REQUEST="Please review this pull request.

BRANCH: $CURRENT_BRANCH -> $BASE_BRANCH

CHANGES SUMMARY:
$DIFF_STAT

COMMITS:
$(git log --oneline "$BASE_BRANCH".."$CURRENT_BRANCH")

DETAILED CHANGES:
\`\`\`diff
$DIFF
\`\`\`

Please evaluate:
1. **Code Quality** - Are changes well-structured and maintainable?
2. **Functionality** - Do changes achieve their intended purpose?
3. **Testing** - Are changes adequately tested?
4. **Documentation** - Are changes properly documented?
5. **Breaking Changes** - Any backward compatibility issues?

Provide a summary recommendation: APPROVE | REQUEST_CHANGES | COMMENT"

echo "$REVIEW_REQUEST" | claude --no-project --no-memory --print