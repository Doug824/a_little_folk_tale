#!/bin/bash

# Get Reviews - Comprehensive code review using dual Claude Code perspectives

set -e

# Color codes for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

echo -e "${BLUE}=== CODE REVIEW TOOL ===${NC}"
echo "Performing comprehensive code review using dual Claude Code perspectives"
echo ""

# Generate git diff
echo -e "${YELLOW}Checking for changes to review...${NC}"

# Check for uncommitted changes or use last commit
if [[ -n $(git status --porcelain) ]]; then
    echo "Reviewing uncommitted changes..."
    # Include both staged and unstaged changes
    DIFF=$(git diff HEAD)
    DIFF_STAT=$(git diff HEAD --stat)
    if [[ -z "$DIFF" ]]; then
        # If no changes vs HEAD, check if there are only unstaged changes
        DIFF=$(git diff)
        DIFF_STAT=$(git diff --stat)
    fi
else
    echo "Reviewing last commit..."
    DIFF=$(git diff HEAD~1 HEAD)
    DIFF_STAT=$(git diff HEAD~1 HEAD --stat)
fi

# Validate we have changes
if [[ -z "$DIFF" ]]; then
    echo -e "${RED}No changes to review!${NC}"
    exit 1
fi

echo -e "${GREEN}Changes found:${NC}"
echo "$DIFF_STAT"
echo ""

# Create review request content
REVIEW_REQUEST="You are an expert code reviewer. Please provide a comprehensive code review.

CHANGE SUMMARY:
$DIFF_STAT

DETAILED CHANGES:
\`\`\`diff
$DIFF
\`\`\`

ONLY report actual problems that need fixing. Do NOT report things that are already correctly implemented.

Please provide:
1. **Overall Assessment** (1-2 sentences about problems found, or 'No significant issues found')
2. **Security Vulnerabilities** (Only actual security problems that need fixing)
3. **Code Quality Problems** (Only actual issues with maintainability, readability, best practices)
4. **Performance Problems** (Only actual bottlenecks or inefficiencies that need fixing)
5. **Logic Errors** (Only actual bugs, edge cases, incorrect implementations)
6. **Missing Tests** (Only actual gaps in test coverage that need addressing)

Format each issue as:
- **Severity**: [CRITICAL|HIGH|MEDIUM|LOW]
- **Type**: [SECURITY|QUALITY|PERFORMANCE|LOGIC|TESTING]
- **File**: path/to/file.ext:line_number
- **Issue**: Brief description
- **Code Snippet**: Show the problematic code
- **Details**: Detailed explanation
- **Recommendation**: Specific fix suggestion
- **Fix Assessment**: [CLAUDE_CAN_FIX|NEEDS_HUMAN_HELP|REQUIRES_REDESIGN]
- **Effort Estimate**: [5min|15min|1hr|4hr+]

Focus on the most dangerous and impactful issues first."

# Perform fresh review (new instance)
echo -e "${BLUE}=== FRESH PERSPECTIVE REVIEW ===${NC}"
echo "Getting unbiased review from fresh Claude Code instance..."
echo ""

# Execute fresh review via Claude CLI
echo "$REVIEW_REQUEST" | claude --no-project --no-memory --print

echo ""
echo -e "${GREEN}Review complete!${NC}"
echo ""
echo "Note: For contextual review with project knowledge, run this script from within Claude Code."
echo "The fresh perspective review above provides an unbiased assessment of the code changes."