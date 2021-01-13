// -----------------------------------------------------------------------
// <copyright file="JsonSerializer.cs" company="Calrom Ltd.">
// Under MIT license
// </copyright>
// -----------------------------------------------------------------------

namespace DotNetMockServerClient.Serializer
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
        /// <param name="obj">the count.</param>
        /// <returns>response.</returns>
        public string Serialize(T obj)
        {
            return JsonSerializer.Serialize<T>(obj, new JsonSerializerOptions() { IgnoreNullValues = true });
        }

        /// <summary>
        /// Deserializes the array asynchronous.
        /// </summary>
        /// <param name="payload">The payload.</param>
        /// <returns>The task.</returns>
        public async Task<T[]> DeserializeArrayAsync(Stream payload)
        {
            return await JsonSerializer.DeserializeAsync<T[]>(payload, new JsonSerializerOptions() { IgnoreNullValues = true }).ConfigureAwait(false);
        }

        /// <summary>
        /// Deserializes the list asynchronous.
        /// </summary>
        /// <param name="payload">The payload.</param>
        /// <returns>The task.</returns>
        public async Task<List<T>> DeserializeListAsync(Stream payload)
        {
            return await JsonSerializer.DeserializeAsync<List<T>>(payload, new JsonSerializerOptions() { IgnoreNullValues = true }).ConfigureAwait(false);
        }

        /// <summary>
        /// Search data.
        /// </summary>
        /// <param name="payload">the count.</param>
        /// <returns>response.</returns>
        public async Task<T> DeserializeObjectAsync(Stream payload)
        {
            return await JsonSerializer.DeserializeAsync<T>(payload, new JsonSerializerOptions() { IgnoreNullValues = true }).ConfigureAwait(false);
        }

        /// <summary>
        /// Serializes the asynchronous.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="payload">The payload.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task SerializeAsync(T obj, Stream payload)
        {
            await JsonSerializer.SerializeAsync<T>(payload, obj, new JsonSerializerOptions() { IgnoreNullValues = true }).ConfigureAwait(false);
        }
    }
}
