// -----------------------------------------------------------------------
// <copyright file="Cookies.cs" company="Calrom Ltd.">
// Under MIT license
// </copyright>
// -----------------------------------------------------------------------

namespace DotNetMockServerClient.DataContracts
{
    using System.Runtime.Serialization;
    using System.Text.Json.Serialization;

    /// <summary>
    /// The mock server http request.
    /// </summary>
    [DataContract]
    public class Cookies
    {
        /// <summary>
        /// Gets or sets the method.
        /// </summary>
        [DataMember(Name = "(?).*testruncontext.*")]
        [JsonPropertyName("(?).*testruncontext.*")]
        public string TestRunContext { get; set; }
    }
}
