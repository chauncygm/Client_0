"""Simple MCP client for Unity MCP Server."""
import json
import sys
import subprocess
import re

MCP_URL = "http://localhost:20508/"
AUTH = "Bearer A2NClRmMd1Z6UkWARhrG4YkG1gfkm05ad_djTvMZ_0A"

def mcp_request(method, params=None, session_id=None):
    """Make a JSON-RPC request to the MCP server."""
    headers = [
        'Authorization', AUTH,
        'Content-Type', 'application/json',
        'Accept', 'application/json, text/event-stream',
    ]
    if session_id:
        headers += ['Mcp-Session-Id', session_id]

    payload = {
        "jsonrpc": "2.0",
        "id": 1,
        "method": method,
        "params": params or {}
    }

    cmd = ['curl', '-s', '--max-time', '30', '--noproxy', '*', '-X', 'POST', MCP_URL,
           '-D', '/tmp/mcp_resp_headers.txt']
    for i in range(0, len(headers), 2):
        cmd += ['-H', f'{headers[i]}: {headers[i+1]}']
    cmd += ['-d', json.dumps(payload)]

    result = subprocess.run(cmd, capture_output=True, text=True)

    # Parse SSE response
    output = result.stdout.strip()
    if 'event: message' in output or 'data:' in output:
        # Extract data from SSE
        match = re.search(r'data:\s*(\{.*\})', output, re.DOTALL)
        if match:
            return json.loads(match.group(1))

    # Try to parse as JSON directly
    try:
        return json.loads(output)
    except:
        return {"raw": output, "stderr": result.stderr}

    return None

def get_session_id():
    """Initialize and get a new session ID."""
    result = subprocess.run(
        ['curl', '-s', '--max-time', '10', '--noproxy', '*', '-X', 'POST', MCP_URL,
         '-D', '/tmp/mcp_resp_headers.txt',
         '-H', 'Authorization: ' + AUTH,
         '-H', 'Content-Type: application/json',
         '-H', 'Accept: application/json, text/event-stream',
         '-d', json.dumps({
             "jsonrpc": "2.0", "id": 1, "method": "initialize",
             "params": {
                 "protocolVersion": "2024-11-05",
                 "capabilities": {},
                 "clientInfo": {"name": "claude-code", "version": "1.0"}
             }
         })],
        capture_output=True, text=True
    )

    # Read session ID from headers
    try:
        with open('/tmp/mcp_resp_headers.txt') as f:
            headers = f.read()
            match = re.search(r'Mcp-Session-Id:\s*(\S+)', headers)
            if match:
                return match.group(1)
    except:
        pass
    return None

def call_tool(session_id, tool_name, arguments=None):
    """Call an MCP tool."""
    resp = mcp_request("tools/call", {
        "name": tool_name,
        "arguments": arguments or {}
    }, session_id)
    return resp

if __name__ == "__main__":
    if len(sys.argv) < 2:
        print("Usage: mcp_client.py <command> [args...]")
        print("  mcp_client.py init                    - Get session ID")
        print("  mcp_client.py tools <session_id>      - List tools")
        print("  mcp_client.py call <session_id> <tool> [json_args] - Call tool")
        sys.exit(1)

    cmd = sys.argv[1]

    if cmd == "init":
        sid = get_session_id()
        if sid:
            print(f"SESSION_ID={sid}")
        else:
            print("ERROR: Could not get session ID")

    elif cmd == "tools":
        sid = sys.argv[2]
        resp = mcp_request("tools/list", {}, sid)
        print(json.dumps(resp, indent=2, ensure_ascii=False))

    elif cmd == "call":
        sid = sys.argv[2]
        tool = sys.argv[3]
        args = json.loads(sys.argv[4]) if len(sys.argv) > 4 else {}
        resp = call_tool(sid, tool, args)
        print(json.dumps(resp, indent=2, ensure_ascii=False))
