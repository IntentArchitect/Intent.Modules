using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intent.Modules.Common.AI.Configuration
{
    /// <summary>
    /// Provides functionality to guess and score templates based on token matches from a given name or path.
    /// </summary>
    public static class TemplateGuesser
    {
        /// <summary>
        /// Represents the result of a template guess operation, including the best template ID, score, confidence, leaf segment, tokens, and top candidates.
        /// </summary>
        public sealed class GuessResult
        {
            /// <summary>
            /// Gets the ID of the best-matching template.
            /// </summary>
            public string TemplateId { get; init; }

            /// <summary>
            /// Gets the score assigned to the best-matching template.
            /// </summary>
            public double Score { get; init; }

            /// <summary>
            /// Gets the confidence value (0..1) for the match, normalized against the template's maximum positive weight.
            /// </summary>
            public double Confidence { get; init; } // 0..1 against that template’s max positive weight

            /// <summary>
            /// Gets the leaf segment (last path segment) used for tokenization.
            /// </summary>
            public string Leaf { get; init; } = "";

            /// <summary>
            /// Gets the list of tokens extracted from the leaf and optionally group segments.
            /// </summary>
            public IReadOnlyList<string> Tokens { get; init; } = Array.Empty<string>();

            /// <summary>
            /// Gets the top candidate templates, including their IDs, scores, and priorities.
            /// </summary>
            public IReadOnlyList<(string TemplateId, double Score, int Priority)> TopCandidates { get; init; } = Array.Empty<(string, double, int)>();
        }

        /// <summary>
        /// Scores templates using token matches from the leaf (last path segment).
        /// Set <paramref name="includeGroupTokens"/> to true to also include tokens from path segments before the leaf.
        /// </summary>
        /// <param name="cfg">The prompt configuration containing templates to score.</param>
        /// <param name="nameOrPath">The name or path to tokenize and match against templates.</param>
        /// <param name="includeGroupTokens">Whether to include tokens from group segments before the leaf.</param>
        /// <returns>
        /// A <see cref="GuessResult"/> containing the best match and top candidates, or <c>null</c> if no confident match is found.
        /// </returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="cfg"/> is null.</exception>
        public static GuessResult Guess(PromptConfig cfg, string nameOrPath, bool includeGroupTokens = false)
        {
            if (cfg is null) throw new ArgumentNullException(nameof(cfg));
            if (cfg.Templates is null || cfg.Templates.Count == 0)
                return new GuessResult { TemplateId = null, Score = 0, Confidence = 0, Leaf = GetLeaf(nameOrPath) };

            var (leaf, tokens) = BuildTokens(nameOrPath, includeGroupTokens);

            // Precompute a set for O(1) membership checks
            var tokenSet = new HashSet<string>(tokens, StringComparer.OrdinalIgnoreCase);

            // Score each template: sum(keywords in tokens) - sum(negatives in tokens)
            var scored = cfg.Templates.Select(t =>
            {
                var pos = (t.Match?.Keywords ?? new()).Sum(k => tokenSet.Contains((k.Word ?? "").ToLowerInvariant()) ? k.Weight : 0);
                var neg = (t.Match?.Negatives ?? new()).Sum(k => tokenSet.Contains((k.Word ?? "").ToLowerInvariant()) ? k.Weight : 0);
                var score = pos - neg;
                var priority = t.Match?.Priority ?? 0;

                // For confidence, normalize by the max achievable positive weight for THIS template
                var maxPos = (t.Match?.Keywords ?? new()).Sum(k => k.Weight);
                var confidence = maxPos > 0 ? Clamp01(score / maxPos) : 0;

                return new
                {
                    TemplateId = t.Id,
                    Score = score,
                    Priority = priority,
                    Confidence = confidence
                };
            })
            .OrderByDescending(x => x.Score)
            .ThenByDescending(x => x.Priority) // deterministic tie-breaker
            .ToList();

            var best = scored.First();

            if (best.Confidence == 0 || best.Score == 0)
                return null;

            return new GuessResult
            {
                TemplateId = best.TemplateId,
                Score = best.Score,
                Confidence = Clamp01(best.Confidence),
                Leaf = leaf,
                Tokens = tokens,
                TopCandidates = scored
                    .Take(3)
                    .Select(x => (x.TemplateId, Math.Round(x.Score, 2), x.Priority))
                    .ToList()
            };
        }

        // ---------- helpers ----------

        /// <summary>
        /// Clamps a double value to the range [0, 1].
        /// </summary>
        /// <param name="v">The value to clamp.</param>
        /// <returns>The clamped value.</returns>
        private static double Clamp01(double v) => v < 0 ? 0 : (v > 1 ? 1 : v);

        /// <summary>
        /// Gets the leaf segment (last path segment) from a name or path.
        /// </summary>
        /// <param name="nameOrPath">The name or path string.</param>
        /// <returns>The leaf segment.</returns>
        private static string GetLeaf(string nameOrPath)
        {
            var s = nameOrPath ?? "";
            var parts = s.Split('/', StringSplitOptions.RemoveEmptyEntries);
            return parts.Length == 0 ? s : parts[^1];
        }

        /// <summary>
        /// Builds tokens from the leaf segment and, optionally, group segments of a name or path.
        /// </summary>
        /// <param name="nameOrPath">The name or path to tokenize.</param>
        /// <param name="includeGroupTokens">Whether to include tokens from group segments before the leaf.</param>
        /// <returns>A tuple containing the leaf segment and the list of tokens.</returns>
        private static (string leaf, List<string> tokens) BuildTokens(string nameOrPath, bool includeGroupTokens)
        {
            var s = nameOrPath ?? "";
            var parts = s.Split('/', StringSplitOptions.RemoveEmptyEntries);
            var leaf = parts.Length == 0 ? s : parts[^1];

            var tokens = SplitTokens(leaf);

            if (includeGroupTokens && parts.Length > 1)
            {
                for (int i = 0; i < parts.Length - 1; i++)
                    tokens.AddRange(SplitTokens(parts[i]));
            }

            // lower-case everything for matching against config words
            for (int i = 0; i < tokens.Count; i++)
                tokens[i] = tokens[i].ToLowerInvariant();

            return (leaf, tokens);
        }

        /// <summary>
        /// Splits a string segment into tokens, aware of casing, numbers, and common separators.
        /// Examples:
        ///  - "AddOrder"   -> ["Add","Order"]
        ///  - "OrderAdd"   -> ["Order","Add"]
        ///  - "Customer-Create" -> ["Customer","Create"]
        ///  - "Customer1A" -> ["Customer","1","A"]
        /// </summary>
        /// <param name="segment">The string segment to tokenize.</param>
        /// <returns>A list of tokens extracted from the segment.</returns>
        private static List<string> SplitTokens(string segment)
        {
            var t = (segment ?? "").Replace('_', ' ').Replace('-', ' ').Replace('.', ' ');
            var tokens = new List<string>();
            int start = 0;

            bool IsBoundary(int i)
            {
                if (i <= 0 || i >= t.Length) return false;
                char prev = t[i - 1], curr = t[i];
                if (prev == ' ' || curr == ' ') return true;                          // separators
                if (char.IsLower(prev) && char.IsUpper(curr)) return true;            // fooBar
                if (char.IsLetter(prev) && char.IsDigit(curr)) return true;           // Foo1
                if (char.IsDigit(prev) && char.IsLetter(curr)) return true;           // 1Foo
                                                                                      // XMLParser → split before 'P': XML | Parser
                if (char.IsUpper(prev) && char.IsUpper(curr) && i + 1 < t.Length && char.IsLower(t[i + 1])) return true;
                return false;
            }

            void Push(int i)
            {
                var part = t.AsSpan(start, i - start).ToString().Trim();
                if (part.Length > 0) tokens.Add(part);
                start = i;
            }

            for (int i = 1; i < t.Length; i++)
                if (IsBoundary(i)) Push(i);
            Push(t.Length);

            // collapse multi-spaces and drop empties
            return tokens
                .SelectMany(x => x.Split(' ', StringSplitOptions.RemoveEmptyEntries))
                .ToList();
        }
    }
}