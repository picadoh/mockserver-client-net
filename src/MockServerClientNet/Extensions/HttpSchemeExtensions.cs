using System;
using MockServerClientNet.Model;

namespace MockServerClientNet.Extensions
{
    public static class HttpSchemeExtensions
    {
        public static string Value(this HttpScheme scheme)
        {
            switch (scheme)
            {
                case HttpScheme.Http: return "http";
                case HttpScheme.Https: return "https";
                default: throw new ArgumentOutOfRangeException($"{scheme} is not a valid HTTP scheme");
            }
        }
    }
}