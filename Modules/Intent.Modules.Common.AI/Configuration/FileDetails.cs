using Intent.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intent.Modules.Common.AI.Configuration
{
    /// <summary>
    /// Represents the details of a file in the codebase, including its path and content.
    /// </summary>
    public class FileDetails : ICodebaseFile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FileDetails"/> class with the specified file path and content.
        /// </summary>
        /// <param name="path">The path of the file.</param>
        /// <param name="content">The content of the file.</param>
        public FileDetails(string path, string content)
        {
            FilePath = path;
            Content = content;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileDetails"/> class with the specified content and no file path.
        /// </summary>
        /// <param name="content">The content of the file.</param>
        public FileDetails(string content)
        {
            FilePath = null;
            Content = content;
        }

        /// <summary>
        /// Gets the path of the file.
        /// </summary>
        public string FilePath { get; }

        /// <summary>
        /// Gets the content of the file.
        /// </summary>
        public string Content { get; }
    }
}
