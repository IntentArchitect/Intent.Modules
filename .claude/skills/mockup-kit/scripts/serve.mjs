#!/usr/bin/env node
// Zero-dependency static server for mockups, rooted at the repository's intent/ folder:
//   node <skill>/scripts/serve.mjs [port]   → http://localhost:5510/.specs/<slug>/mockups/<screen>/after.html
// Read-only, loopback-only, never caches, so a reload always shows the latest edit.
import { execSync } from 'node:child_process';
import { createServer } from 'node:http';
import { readFile, stat } from 'node:fs/promises';
import { extname, join, normalize, relative, resolve, isAbsolute } from 'node:path';

const repoRoot = (() => {
  try { return execSync('git rev-parse --show-toplevel', { stdio: ['ignore', 'pipe', 'ignore'] }).toString().trim(); } catch { return process.cwd(); }
})();
const root = resolve(repoRoot, 'intent');
const port = Number(process.argv[2] ?? process.env.PORT ?? 5510);

const types = {
  '.html': 'text/html; charset=utf-8', '.css': 'text/css; charset=utf-8', '.js': 'text/javascript; charset=utf-8',
  '.mjs': 'text/javascript; charset=utf-8', '.json': 'application/json; charset=utf-8', '.svg': 'image/svg+xml',
  '.png': 'image/png', '.jpg': 'image/jpeg', '.jpeg': 'image/jpeg', '.gif': 'image/gif', '.webp': 'image/webp',
  '.ico': 'image/x-icon', '.woff': 'font/woff', '.woff2': 'font/woff2', '.ttf': 'font/ttf', '.otf': 'font/otf',
  '.eot': 'application/vnd.ms-fontobject', '.md': 'text/markdown; charset=utf-8',
};

createServer(async (request, response) => {
  const path = normalize(decodeURIComponent(new URL(request.url, 'http://localhost').pathname));
  let file = join(root, path);
  const inside = relative(root, file);
  if (inside.startsWith('..') || isAbsolute(inside)) {
    response.writeHead(403).end();
    return;
  }
  try {
    if ((await stat(file)).isDirectory()) file = join(file, 'index.html');
    const body = await readFile(file);
    response.writeHead(200, { 'Content-Type': types[extname(file).toLowerCase()] ?? 'application/octet-stream', 'Cache-Control': 'no-store' });
    response.end(body);
  } catch {
    response.writeHead(404, { 'Content-Type': 'text/plain' }).end(`Not found: ${path}`);
  }
}).listen(port, '127.0.0.1', () => console.log(`Mockups served from ${root} at http://localhost:${port}/`));
