#!/bin/bash

# Security-focused review of current changes

set -e

# Color codes
RED='\033[0;31m'
YELLOW='\033[1;33m'
NC='\033[0m'

echo -e "${RED}=== SECURITY REVIEW ===${NC}"
echo "Scanning for security vulnerabilities in current changes..."
echo ""

# Get current changes
if [[ -n $(git status --porcelain) ]]; then
    DIFF=$(git diff HEAD)
    FILES_CHANGED=$(git diff HEAD --name-only)
else
    DIFF=$(git diff HEAD~1 HEAD)
    FILES_CHANGED=$(git diff HEAD~1 HEAD --name-only)
fi

if [[ -z "$DIFF" ]]; then
    echo -e "${YELLOW}No changes to review!${NC}"
    exit 1
fi

echo "Files being reviewed:"
echo "$FILES_CHANGED"
echo ""

# Create security-focused review
REVIEW_REQUEST="You are a security expert. Review these changes ONLY for security vulnerabilities.

FILES CHANGED:
$FILES_CHANGED

CHANGES:
\`\`\`diff
$DIFF
\`\`\`

Look for:
1. **Authentication/Authorization** issues
2. **Input validation** vulnerabilities (SQL injection, XSS, command injection)
3. **Sensitive data exposure** (keys, passwords, tokens)
4. **Insecure dependencies** or imports
5. **Path traversal** vulnerabilities
6. **Race conditions** or timing attacks
7. **Cryptographic** weaknesses
8. **CORS/CSP** misconfigurations
9. **File upload** vulnerabilities
10. **Memory safety** issues

For each vulnerability found:
- **Severity**: [CRITICAL|HIGH|MEDIUM|LOW]
- **CWE ID**: (if applicable)
- **Location**: file:line
- **Vulnerability**: Specific issue
- **Impact**: What could an attacker do?
- **Fix**: Concrete remediation steps

ONLY report actual security vulnerabilities. Do not report code style or non-security issues."

echo "$REVIEW_REQUEST" | claude --no-project --no-memory --print