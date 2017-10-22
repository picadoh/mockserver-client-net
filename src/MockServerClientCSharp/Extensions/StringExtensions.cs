namespace MockServerClientCSharp.Extensions
{
  using System;

  public static class StringExtensions
  {
    public static string PrefixWith(this string input, string prefix)
    {
      return (!input.StartsWith(prefix, StringComparison.CurrentCulture) ? prefix : string.Empty) + input;
    }

    public static string SuffixWith(this string input, string suffix)
    {
      return input + (!input.EndsWith(suffix, StringComparison.CurrentCulture) ? suffix : string.Empty);
    }

    public static string RemovePrefix(this string input, string prefix)
    {
      return input.StartsWith(prefix, StringComparison.CurrentCulture) ? input.Substring(prefix.Length) : input;
    }
  }
}
