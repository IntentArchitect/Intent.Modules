using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Intent.Modules.Common.Templates.AIStaticContent
{
    internal static class MarkdownContentHash
    {
        private const string FieldName = "contentHash";

        public static string AddOrUpdateContentHash(string markdown)
        {
            var newline = DetectNewline(markdown);

            if (!TrySplitFrontMatter(markdown, out var frontMatter, out var body, out var frontMatterStart, out var frontMatterEnd))
            {
                frontMatter = string.Empty;
                body = markdown;
            }

            var frontMatterWithoutHash = RemoveContentHashField(frontMatter, newline);
            var documentWithoutHash = BuildDocument(frontMatterWithoutHash, body, newline, hasFrontMatter: !string.IsNullOrEmpty(frontMatter) || frontMatterStart == 0 && frontMatterEnd > 0);
            var hash = ComputeSha256(documentWithoutHash);

            var updatedFrontMatter = InsertContentHashField(frontMatterWithoutHash, hash, newline);

            return BuildDocument(updatedFrontMatter, body, newline, hasFrontMatter: true);
        }

        public static bool HasChanged(string markdown)
        {
            if (!TrySplitFrontMatter(markdown, out var frontMatter, out var body, out _, out _))
                return false;

            var existingHash = ExtractContentHash(frontMatter);
            if (string.IsNullOrWhiteSpace(existingHash))
                return false;

            var newline = DetectNewline(markdown);
            var frontMatterWithoutHash = RemoveContentHashField(frontMatter, newline);
            var documentWithoutHash = BuildDocument(frontMatterWithoutHash, body, newline, hasFrontMatter: true);
            var computedHash = ComputeSha256(documentWithoutHash);

            return !string.Equals(existingHash, computedHash, StringComparison.OrdinalIgnoreCase);
        }

        public static bool CanOverwrite(string markdown) => !HasChanged(markdown);

        public static void AddOrUpdateContentHashInFile(string filePath)
        {
            var content = File.ReadAllText(filePath);
            var updated = AddOrUpdateContentHash(content);
            File.WriteAllText(filePath, updated);
        }

        public static bool HasFileChanged(string filePath)
        {
            var content = File.ReadAllText(filePath);
            return HasChanged(content);
        }

        public static bool CanOverwriteFile(string filePath)
        {
            var content = File.ReadAllText(filePath);
            return CanOverwrite(content);
        }

        private static bool TrySplitFrontMatter(
            string markdown,
            out string frontMatter,
            out string body,
            out int frontMatterStart,
            out int frontMatterEnd)
        {
            frontMatter = string.Empty;
            body = markdown;
            frontMatterStart = -1;
            frontMatterEnd = -1;

            if (string.IsNullOrEmpty(markdown))
                return false;

            var match = Regex.Match(
                markdown,
                @"\A---(?:\r\n|\n)(.*?)(?:\r\n|\n)---(?:\r\n|\n|$)",
                RegexOptions.Singleline);

            if (!match.Success)
                return false;

            frontMatterStart = 0;
            frontMatterEnd = match.Length;
            frontMatter = match.Groups[1].Value;
            body = markdown.Substring(match.Length);

            return true;
        }

        private static string? ExtractContentHash(string frontMatter)
        {
            var match = Regex.Match(
                frontMatter,
                @"(?mi)^[ \t]*contentHash[ \t]*:[ \t]*(.+?)[ \t]*$");

            return match.Success ? match.Groups[1].Value.Trim() : null;
        }

        private static string RemoveContentHashField(string frontMatter, string newline)
        {
            if (string.IsNullOrEmpty(frontMatter))
                return frontMatter;

            var result = Regex.Replace(
                frontMatter,
                @"(?mi)^[ \t]*contentHash[ \t]*:.*(?:\r\n|\n|$)",
                string.Empty);

            result = NormalizeBlankLines(result, newline);
            return result.TrimEnd('\r', '\n', ' ', '\t');
        }

        private static string InsertContentHashField(string frontMatter, string hash, string newline)
        {
            var hashLine = $"{FieldName}: {hash}";

            if (string.IsNullOrWhiteSpace(frontMatter))
                return hashLine;

            return frontMatter.TrimEnd('\r', '\n') + newline + hashLine;
        }

        private static string BuildDocument(string frontMatter, string body, string newline, bool hasFrontMatter)
        {
            if (!hasFrontMatter && string.IsNullOrEmpty(frontMatter))
                return body;

            var sb = new StringBuilder();
            sb.Append("---").Append(newline);

            if (!string.IsNullOrEmpty(frontMatter))
            {
                sb.Append(frontMatter.TrimEnd('\r', '\n')).Append(newline);
            }

            sb.Append("---");

            if (!string.IsNullOrEmpty(body))
            {
                sb.Append(newline);
                sb.Append(body);
            }

            return sb.ToString();
        }

        private static string ComputeSha256(string content)
        {
            var normalized = NormalizeLineEndings(content);
            var bytes = Encoding.UTF8.GetBytes(normalized);
            using var sha = SHA256.Create();
            var hash = sha.ComputeHash(bytes);
            return Convert.ToHexString(hash);
        }

        private static string DetectNewline(string content)
        {
            return content.Contains("\r\n") ? "\r\n" : "\n";
        }

        private static string NormalizeLineEndings(string content)
        {
            return content.Replace("\r\n", "\n").Replace("\r", "\n");
        }

        private static string NormalizeBlankLines(string text, string newline)
        {
            var normalized = NormalizeLineEndings(text);
            normalized = Regex.Replace(normalized, @"\n{3,}", "\n\n");
            return newline == "\r\n" ? normalized.Replace("\n", "\r\n") : normalized;
        }
    }
}
