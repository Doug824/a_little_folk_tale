#!/bin/bash

# Review specific file - Detailed review of a single file

set -e

# Color codes
BLUE='\033[0;34m'
RED='\033[0;31m'
NC='\033[0m'

if [ $# -eq 0 ]; then
    echo -e "${RED}Usage: $0 <file_path>${NC}"
    exit 1
fi

FILE_PATH="$1"

if [ ! -f "$FILE_PATH" ]; then
    echo -e "${RED}File not found: $FILE_PATH${NC}"
    exit 1
fi

echo -e "${BLUE}Reviewing file: $FILE_PATH${NC}"

# Get file content
FILE_CONTENT=$(cat "$FILE_PATH")

# Create review request
REVIEW_REQUEST="Please review this file for code quality, security, and best practices.

File: $FILE_PATH

\`\`\`$(basename "$FILE_PATH" | sed 's/.*\.//')
$FILE_CONTENT
\`\`\`

Focus on:
1. Security vulnerabilities
2. Code organization and readability
3. Performance optimizations
4. Best practices for the language/framework
5. Potential bugs or edge cases

Only report actual issues that need fixing."

echo "$REVIEW_REQUEST" | claude --no-project --no-memory --print