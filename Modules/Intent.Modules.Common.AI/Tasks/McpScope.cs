using Intent.Modules.Common.AI.Configuration;
using Microsoft.SemanticKernel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Intent.Modules.Common.AI.Tasks
{
    /// <summary>
    /// Manages the lifecycle of Model Context Protocol (MCP) server connections for a Semantic Kernel instance.
    /// Ensures proper disposal of MCP clients after use.
    /// </summary>
    public sealed class McpScope : IDisposable
    {
        private readonly IReadOnlyList<IAsyncDisposable> _disposables;

        /// <summary>
        /// Initializes a new instance of the <see cref="McpScope"/> class with the specified disposables.
        /// </summary>
        /// <param name="disposables">The list of MCP client disposables to manage.</param>
        private McpScope(IReadOnlyList<IAsyncDisposable> disposables)
        {
            _disposables = disposables;
        }

        /// <summary>
        /// Creates an <see cref="McpScope"/> by wiring up MCP servers for the given kernel.
        /// </summary>
        /// <param name="kernel">The Semantic Kernel instance to associate MCP servers with.</param>
        /// <param name="servers">The collection of MCP server configurations.</param>
        /// <returns>An <see cref="McpScope"/> instance managing the MCP client connections.</returns>
        public static McpScope Create(
            Kernel kernel,
            IEnumerable<McpServer>? servers)
        {
            if (servers == null)
            {
                return new McpScope(Array.Empty<IAsyncDisposable>());
            }

            var serverList = servers as List<McpServer> ?? servers.ToList();
            if (serverList.Count == 0)
            {
                return new McpScope(Array.Empty<IAsyncDisposable>());
            }

            var disposables = McpHelper
                .WireUpMcpAsync(kernel, serverList)
                .GetAwaiter()
                .GetResult()
                .ToList();

            return new McpScope(disposables);
        }

        /// <summary>
        /// Disposes all managed MCP client connections asynchronously.
        /// </summary>
        public void Dispose()
        {
            foreach (var disposable in _disposables)
            {
                try
                {
                    disposable.DisposeAsync().AsTask().GetAwaiter().GetResult();
                }
                catch
                {
                    // Optional: log and continue
                }
            }
        }
    }
}
