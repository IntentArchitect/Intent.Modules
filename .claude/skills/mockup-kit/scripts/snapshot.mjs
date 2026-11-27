#!/usr/bin/env node
// Takes (or refreshes) an app's mockup snapshot: reads the app's built stylesheets and writes them as ONE
// self-contained intent/.mockups/<app>/styles.css, every @import spliced in and every font and image it names
// embedded, so the snapshot is a single committed file that renders the same on every clone. Also copies the
// mockup kit beside it and records where it all came from (SOURCE.md). Driven by the committed
// intent/.mockups/<app>/snapshot.json, so a refresh is the same command with no decisions left in it.
//
//   node <skill>/scripts/snapshot.mjs intent/.mockups/<app>/snapshot.json [--prepare | --prepare-if-needed]
//
// snapshot.json:
//   {
//     "app": "Storefront",                          // display name; the folder name is the snapshot's id
//     "sourceRoot": "src/Storefront.Web",           // relative to the repository root
//     "prepare": "npm run build",                   // optional; run in sourceRoot with --prepare, or when a stylesheet is missing with --prepare-if-needed
//     "stylesheets": ["dist/assets/index.css"],     // relative to sourceRoot, in the order the app loads them
//     "exclude": ["public/img/photos"],             // optional; files or folders, relative to sourceRoot, never embedded
//     "themeRule": "html.dark",                     // optional; how the app shows dark (see kit/mockup.js)
//     "pins": "#16a34a",                            // optional; the kit's pin colour (default green)
//     "highlight": "#0091ff"                        // optional; mk-new outlines and the notes panel (default electric blue)
//   }
//
// A url() the snapshot can't or mustn't embed becomes `none`: a mockup renders in a frame that can fetch nothing.
// Each one not excluded on purpose is listed when the script runs. Only styles.css, kit/ and SOURCE.md are
// rewritten; templates/ is yours.
import { execSync } from 'node:child_process';
import { cpSync, existsSync, readFileSync, realpathSync, rmSync, statSync, writeFileSync } from 'node:fs';
import path from 'node:path';
import { fileURLToPath } from 'node:url';

const MIME = {
  '.png': 'image/png', '.jpg': 'image/jpeg', '.jpeg': 'image/jpeg', '.gif': 'image/gif', '.webp': 'image/webp',
  '.bmp': 'image/bmp', '.svg': 'image/svg+xml', '.ico': 'image/x-icon', '.avif': 'image/avif',
  '.woff': 'font/woff', '.woff2': 'font/woff2', '.ttf': 'font/ttf', '.otf': 'font/otf', '.eot': 'application/vnd.ms-fontobject',
};
const MAX_IMPORT_DEPTH = 8;
const WARN_BYTES = 8 * 1024 * 1024;

const skillDir = path.resolve(path.dirname(fileURLToPath(import.meta.url)), '..');
const [configArg, ...flags] = process.argv.slice(2);
if (!configArg) {
  console.error('Usage: node snapshot.mjs intent/.mockups/<app>/snapshot.json [--prepare | --prepare-if-needed]');
  process.exit(2);
}

const configPath = path.resolve(configArg);
const snapshotDir = path.dirname(configPath);
const config = JSON.parse(readFileSync(configPath, 'utf8'));
const git = (args, cwd) => {
  try { return execSync(`git ${args}`, { cwd, stdio: ['ignore', 'pipe', 'ignore'] }).toString().trim(); } catch { return null; }
};
const repoRoot = realpathSync(git('rev-parse --show-toplevel', snapshotDir) ?? process.cwd());
const sourceRoot = path.resolve(repoRoot, config.sourceRoot ?? '.');
if (!existsSync(sourceRoot)) throw new Error(`sourceRoot not found: ${sourceRoot}`);
const sourceRelative = path.relative(repoRoot, realpathSync(sourceRoot));
if (sourceRelative.startsWith('..') || path.isAbsolute(sourceRelative)) throw new Error(`sourceRoot must be inside the repository: ${config.sourceRoot}`);
if (!Array.isArray(config.stylesheets) || config.stylesheets.length === 0) throw new Error('snapshot.json needs a non-empty "stylesheets" list.');
// The kit's own colours for this app, when its look clashes with the defaults (green pins, electric blue highlights).
const colors = Object.entries({ pins: '--mk-pin', highlight: '--mk-highlight' }).flatMap(([key, property]) => {
  if (config[key] == null) return [];
  if (typeof config[key] !== 'string' || !/^(#[0-9a-f]{3,8}|[a-z]+|(rgb|hsl|oklch)a?\([\d\s.,%/-]+\))$/i.test(config[key].trim())) {
    throw new Error(`"${key}" must be one CSS colour, such as "#16a34a": ${JSON.stringify(config[key])}`);
  }
  return [`  ${property}: ${config[key].trim()};`];
});

const unbuilt = () => config.stylesheets.some((sheet) => !existsSync(path.resolve(sourceRoot, sheet)));
if (config.prepare && (flags.includes('--prepare') || (flags.includes('--prepare-if-needed') && unbuilt()))) {
  console.log(`Preparing in ${sourceRoot}: ${config.prepare}`);
  // Intent runs this script with its own Electron as node; the app's build must get a plain environment.
  const { ELECTRON_RUN_AS_NODE: _intentOnly, ...env } = process.env;
  try {
    execSync(config.prepare, { cwd: sourceRoot, stdio: 'inherit', env });
  } catch (error) {
    console.error(`prepare failed${error.status != null ? ` with exit code ${error.status}` : ''}: ${config.prepare}`);
    process.exit(1);
  }
}

const excluded = (config.exclude ?? []).map((entry) => path.resolve(sourceRoot, entry));
const isExcluded = (file) => excluded.some((entry) => file === entry || file.startsWith(entry + path.sep));
const within = (file) => { const rel = path.relative(repoRoot, file); return !!rel && !rel.startsWith('..') && !path.isAbsolute(rel); };
const shown = (file) => path.relative(repoRoot, file).replace(/\\/g, '/');
const missing = new Set();
const embedded = new Map(); // url(data:…) → its index, so each asset is read into the output once

/** The file a relative reference names, or null (listed as missing unless excluded) when it can't be embedded. */
function resolve(ref, directory) {
  const relative = ref.trim().split(/[?#]/)[0];
  if (!relative || /^([a-z][a-z0-9+.-]*:|\/|#)/i.test(relative)) return null;
  let decoded = relative;
  try { decoded = decodeURIComponent(relative); } catch { /* as written */ }
  const file = path.resolve(directory, decoded);
  if (isExcluded(file)) return null;
  if (!within(file) || !existsSync(file) || !statSync(file).isFile()) { missing.add(shown(file)); return null; }
  return file;
}

/** Chromium always takes a @font-face's woff2, so its eot/ttf/svg fallbacks would only multiply the size. */
const woff2Only = (css) => css.replace(/@font-face\s*\{[^}]*\}/gi, (block) => !/\.woff2\b/i.test(block) ? block
  : block.replace(/\bsrc\s*:([^;}]*);?/gi, (_all, list) => {
    const kept = list.split(/,(?![^(]*\))/).filter((entry) => /\.woff2\b|^\s*local\(/i.test(entry));
    return kept.length ? `src:${kept.join(',')};` : '';
  }));

/** A stylesheet with its @imports spliced in and every url() embedded, each against the file it was written in. */
function inline(file, depth = 0) {
  const directory = path.dirname(file);
  const imports = [];
  const own = readFileSync(file, 'utf8')
    .replace(/\/\*[\s\S]*?\*\//g, '')
    .replace(/@charset\s+[^;]+;/gi, '')
    .replace(/@import\s+(?:url\(\s*(['"]?)([^'")]+)\1\s*\)|(['"])([^'"]+)\3)\s*([^;]*);/gi, (_all, _q1, urlRef, _q2, stringRef, media) => {
      const sheet = depth < MAX_IMPORT_DEPTH ? resolve(urlRef ?? stringRef, directory) : null;
      if (!sheet) return '';
      const inner = inline(sheet, depth + 1);
      imports.push(media.trim() ? `@media ${media.trim()} {\n${inner}\n}` : inner);
      return `\u0000import-${imports.length - 1}\u0000`;
    });
  return woff2Only(own)
    .replace(/url\(\s*(['"]?)([^'")]*?)\1\s*\)/gi, (all, _q, ref) => {
      if (/^\s*(data:|#)/i.test(ref)) return all;
      const asset = resolve(ref, directory);
      const mime = asset && MIME[path.extname(asset).toLowerCase()];
      if (!mime) return 'none';
      const fragment = mime === 'image/svg+xml' ? /#.*$/.exec(ref)?.[0] ?? '' : '';
      const uri = `url(data:${mime};base64,${readFileSync(asset).toString('base64')}${fragment})`;
      if (!embedded.has(uri)) embedded.set(uri, embedded.size);
      return `\u0001${embedded.get(uri)}\u0001`;
    })
    .replace(/\u0000import-(\d+)\u0000/g, (_all, index) => imports[Number(index)]);
}

const sheets = config.stylesheets.map((sheet) => {
  const file = path.resolve(sourceRoot, sheet);
  if (!existsSync(file)) throw new Error(`Stylesheet not found (build the app first, or fix "stylesheets"): ${file}`);
  return `/* ${sheet.replace(/\\/g, '/')} */\n${inline(file).trim()}`;
});

// An image several rules use is embedded once, as a custom property; a @font-face can't read one, so fonts stay inline.
const uris = [...embedded.keys()];
const fontFaces = (css) => css.match(/@font-face\s*\{[^}]*\}/gi)?.join('') ?? '';
const all = sheets.join('\n');
const uses = new Map();
for (const [, index] of all.matchAll(/\u0001(\d+)\u0001/g)) uses.set(index, (uses.get(index) ?? 0) + 1);
for (const [, index] of fontFaces(all).matchAll(/\u0001(\d+)\u0001/g)) uses.set(index, 0);
const shared = [...uses].filter(([, count]) => count > 1).map(([index]) => index);
const styles = [
  '/* Generated by the mockup-kit snapshot script from snapshot.json. Do not edit. */',
  ...(colors.length ? [`:root {\n${colors.join('\n')}\n}`] : []),
  ...(shared.length ? [`:root {\n${shared.map((index) => `  --snapshot-asset-${index}: ${uris[index]};`).join('\n')}\n}`] : []),
  ...sheets,
].join('\n\n').replace(/\u0001(\d+)\u0001/g, (_all, index) => (shared.includes(index) ? `var(--snapshot-asset-${index})` : uris[index])) + '\n';
writeFileSync(path.join(snapshotDir, 'styles.css'), styles);

// What an earlier version of this script wrote beside styles.css.
rmSync(path.join(snapshotDir, 'assets'), { recursive: true, force: true });
rmSync(path.join(snapshotDir, 'known-missing.json'), { force: true });

const kitDir = path.join(snapshotDir, 'kit');
rmSync(kitDir, { recursive: true, force: true });
cpSync(path.join(skillDir, 'kit'), kitDir, { recursive: true });

const commit = git('rev-parse --short HEAD', sourceRoot) ?? 'unknown';
const branch = git('rev-parse --abbrev-ref HEAD', sourceRoot) ?? 'unknown';
const dirty = git('status --porcelain', sourceRoot) ? ' (with uncommitted changes)' : '';
writeFileSync(path.join(snapshotDir, 'SOURCE.md'), `# ${config.app ?? path.basename(snapshotDir)} mockup snapshot

The app's built styles, with their fonts and images embedded, so mockups render like it in both themes. Generated:
do not edit \`styles.css\`, \`kit/\` or this file by hand. \`templates/\` is hand-built from the app's real markup.

- Source: \`${path.relative(repoRoot, sourceRoot).replace(/\\/g, '/') || '.'}\`
- Branch: ${branch}
- Commit: ${commit}${dirty}
- Taken: ${new Date().toISOString().slice(0, 16).replace('T', ' ')} UTC
- Dark theme: \`${config.themeRule ?? 'body[data-theme=dark]'}\`

Refresh: \`node <mockup-kit skill>/scripts/snapshot.mjs ${path.relative(repoRoot, configPath).replace(/\\/g, '/')} --prepare\`
`);

if (missing.size) {
  console.warn(`Named by the stylesheets but not found, so left out (add a folder to "exclude" to silence one you leave out on purpose):\n  ${[...missing].sort().join('\n  ')}`);
}
const size = Buffer.byteLength(styles);
if (size > WARN_BYTES) console.warn(`styles.css is ${(size / 1048576).toFixed(1)} MB: "exclude" images no mockup needs.`);
console.log(`Snapshot written to ${shown(path.join(snapshotDir, 'styles.css'))} (${(size / 1048576).toFixed(1)} MB) from ${branch}@${commit}${dirty}`);
