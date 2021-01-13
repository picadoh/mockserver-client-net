// -----------------------------------------------------------------------
// <copyright file="JsonSerializer.cs" company="Calrom Ltd.">
// Under MIT license
// </copyright>
// -----------------------------------------------------------------------

namespace DotNetMockServerClient.DataContracts.Serializer
{
    using System.Collections.Generic;
    using System.IO;
    using System.Text.Json;
    using System.Threading.Tasks;

    /// <summary>
    /// The Json Serializers.
    /// </summary>
    /// <typeparam name="T">Type of the value to be returned.</typeparam>
    public class JsonSerializer<T>
    {
        /// <summary>
        /// Search data.
        /// </summary>
        /// <param name="payload">the count.</param>
        /// <returns>response.</returns>
        public T[] DeserializeArray(string payload)
        {
            return JsonSerializer.Deserialize<T[]>(payload, new JsonSerializerOptions() { IgnoreNullValues = true });
        }

        /// <summary>
        /// Search data.
        /// </summary>
        /// <param name="payload">the count.</param>
        /// <returns>response.</returns>
        public List<T> DeserializeList(string payload)
        {
            return JsonSerializer.Deserialize<List<T>>(payload, new JsonSerializerOptions() { IgnoreNullValues = true });
        }

        /// <summary>
        /// Search data.
        /// </summary>
        /// <param name="payload">the count.</param>
        /// <returns>response.</returns>
        public T DeserializeObject(string payload)
        {
            return JsonSerializer.Deserialize<T>(payload, new JsonSerializerOptions() { IgnoreNullValues = true });
        }

        /// <summary>
        /// Search data.
        /// </summary>
        /// <param name="something">the count.</param>
        /// <returns>response.</returns>
        public string Serialize(T something)
        {
            return JsonSerializer.Serialize<T>(something, new JsonSerializerOptions() { IgnoreNullValues = true });
        }

        /// <summary>
        /// Search data.
        /// </summary>
        /// <param name="payload">the count.</param>
        /// <returns>response.</returns>
        public async Task<T[]> DeserializeArrayAsync(Stream payload)
        {
            return await JsonSerializer.DeserializeAsync<T[]>(payload, new JsonSerializerOptions() { IgnoreNullValues = true }).ConfigureAwait(false);
        }

        /// <summary>
        /// Search data.
        /// </summary>
        /// <param name="payload">the count.</param>
        /// <returns>response.</returns>
        public async Task<List<T>> DeserializeListAsync(Stream payload)
        {
            return await JsonSerializer.DeserializeAsync<List<T>>(payload, new JsonSerializerOptions() { IgnoreNullValues = true }).ConfigureAwait(false);
        }

        /// <summary>
        /// Search data.
        /// </summary>
        /// <param name="payload">the count.</param>
        /// <returns>response.</returns>
        public ValueTask<T> DeserializeObjectAsync(Stream payload)
        {
            return JsonSerializer.DeserializeAsync<T>(payload, new JsonSerializerOptions() { IgnoreNullValues = true });
        }

        /// <summary>
        /// Search data.
        /// </summary>
        /// <param name="obj">the obj.</param>
        /// <param name="payload">the count.</param>
        /// <returns>response.</returns>
        public async Task SerializeAsync(T obj, Stream payload)
        {
            await JsonSerializer.SerializeAsync<T>(payload, obj, new JsonSerializerOptions() { IgnoreNullValues = true }).ConfigureAwait(false);
        }
    }
}
