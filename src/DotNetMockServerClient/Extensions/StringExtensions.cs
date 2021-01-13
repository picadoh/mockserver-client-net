// -----------------------------------------------------------------------
// <copyright file="StringExtensions.cs" company="Calrom Ltd.">
// Under MIT license
// </copyright>
// -----------------------------------------------------------------------

namespace DotNetMockServerClient.Extensions
{
    using System;

    /// <summary>
    /// The string extensions class.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>Prefixes the with.</summary>
        /// <param name="input">The input.</param>
        /// <param name="prefix">The prefix.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        /// <exception cref="ArgumentNullException">input.</exception>
        public static string PrefixWith(this string input, string prefix)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            return (!input.StartsWith(prefix, StringComparison.CurrentCulture) ? prefix : string.Empty) + input;
        }

        /// <summary>
        /// Removes the prefix.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="prefix">The prefix.</param>
        /// <returns>With the prefix removed.</returns>
        public static string RemovePrefix(this string input, string prefix)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            if (prefix == null)
            {
                throw new ArgumentNullException(nameof(prefix));
            }

            return input.StartsWith(prefix, StringComparison.CurrentCulture) ? input.Substring(prefix.Length) : input;
        }

        /// <summary>
        /// Suffixes the with.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="suffix">The suffix.</param>
        /// <returns>With the suffix.</returns>
        public static string SuffixWith(this string input, string suffix)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            return input + (!input.EndsWith(suffix, StringComparison.CurrentCulture) ? suffix : string.Empty);
        }
    }
}
