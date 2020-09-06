using System;
using System.Collections.Generic;
using System.Text;
using Intent.Modules.Common.TypeScript.Weaving;
using Xunit;

namespace Intent.Modules.Common.TypeScript.Tests.NodeJs
{
    public class TypeScriptVariablesAndExpressionMergeTests
    {
        [Fact]
        public void SameExistingAndOutputRemainsUnchanged()
        {
            var merger = new TypeScriptWeavingMerger();
            var result = merger.Merge(existingContent: "", outputContent: @"const http = require('http');

const hostname = '127.0.0.1';
const port = 3000;

const server = http.createServer((req, res) => {
  res.statusCode = 200;
  res.setHeader('Content-Type', 'text/plain');
  res.end('Hello World');
});

server.listen(port, hostname, () => {
  console.log(`Server running at http://${hostname}:${port}/`);
});");

            Assert.Equal(@"const http = require('http');

const hostname = '127.0.0.1';
const port = 3000;

const server = http.createServer((req, res) => {
  res.statusCode = 200;
  res.setHeader('Content-Type', 'text/plain');
  res.end('Hello World');
});

server.listen(port, hostname, () => {
  console.log(`Server running at http://${hostname}:${port}/`);
});", result);
        }

        [Fact]
        public void SkipsIgnoredAndUpdatesOthers()
        {
            var merger = new TypeScriptWeavingMerger();
            var result = merger.Merge(
                existingContent: @"
const http = require('http');

@IntentIgnore()
const hostname = '127.0.0.2';
const port = 4000;

@IntentIgnore()
const server = http.createServer((req, res) => {
  res.statusCode = 200;
});

server.listen(port, hostname, () => {
});", 
                outputContent: @"
const http = require('http');

const hostname = '127.0.0.1';
const port = 3000;

const server = http.createServer((req, res) => {
  res.statusCode = 200;
  res.setHeader('Content-Type', 'text/plain');
  res.end('Hello World');
});

server.listen(port, hostname, () => {
  console.log(`Server running at http://${hostname}:${port}/`);
});");

            Assert.Equal(@"
const http = require('http');

@IntentIgnore()
const hostname = '127.0.0.2';
const port = 3000;

@IntentIgnore()
const server = http.createServer((req, res) => {
  res.statusCode = 200;
});

server.listen(port, hostname, () => {
  console.log(`Server running at http://${hostname}:${port}/`);
});", result);
        }

        [Fact]
        public void SkipsIgnoredInCommentsAndUpdatesOthers()
        {
            var merger = new TypeScriptWeavingMerger();
            var result = merger.Merge(
                existingContent: @"
//@IntentIgnore()
const http = require('http-changed');

const hostname = '127.0.0.2';
const port = 4000;

const server = http.createServer((req, res) => {
  res.statusCode = 200;
});

//@IntentIgnore()
server.listen(port, hostname, () => {
  // changed
});",
                outputContent: @"
const http = require('http');

const hostname = '127.0.0.1';
const port = 3000;

const server = http.createServer((req, res) => {
  res.statusCode = 200;
  res.setHeader('Content-Type', 'text/plain');
  res.end('Hello World');
});

server.listen(port, hostname, () => {
  console.log(`Server running at http://${hostname}:${port}/`);
});");

            Assert.Equal(@"
//@IntentIgnore()
const http = require('http-changed');

const hostname = '127.0.0.1';
const port = 3000;

const server = http.createServer((req, res) => {
  res.statusCode = 200;
  res.setHeader('Content-Type', 'text/plain');
  res.end('Hello World');
});

//@IntentIgnore()
server.listen(port, hostname, () => {
  // changed
});", result);
        }
    }
}
