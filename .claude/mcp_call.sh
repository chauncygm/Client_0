#!/bin/bash
# Usage: mcp_call.sh <method> [json_args]
SESSION="QvvdzZLpLgJ9uy2j79HPKg"
METHOD="$1"
ARGS="${2:-{}}"

RESP=$(curl -s --max-time 30 -X POST "http://localhost:20508/" \
  --noproxy '*' \
  -H 'Authorization: Bearer A2NClRmMd1Z6UkWARhrG4YkG1gfkm05ad_djTvMZ_0A' \
  -H "Mcp-Session-Id: $SESSION" \
  -H 'Content-Type: application/json' \
  -H 'Accept: application/json, text/event-stream' \
  -d "{\"jsonrpc\":\"2.0\",\"id\":$(date +%s),\"method\":\"$METHOD\",\"params\":$ARGS}")

# Strip SSE wrapper
echo "$RESP" | sed 's/^event: message\ndata: //' | sed 's/^data: //' | python -c "import sys,json; d=json.load(sys.stdin); print(json.dumps(d.get('result', d), indent=2, ensure_ascii=False))" 2>/dev/null || echo "$RESP"
