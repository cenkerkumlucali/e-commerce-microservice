using System.Net.Http.Json;

namespace WebApp.Extensions;

public static class HttpClientExtension
{
    public async static Task<TResult> PostGetResponseAsync<TResult, TValue>(this HttpClient client, string url, TValue value)
    {
        var httpRes = await client.PostAsJsonAsync(url, value);

        return httpRes.IsSuccessStatusCode ? await httpRes.Content.ReadFromJsonAsync<TResult>() : default;
    }

    public async static Task TaskAsync<TValue>(this HttpClient client, string url, TValue value)
    {
        await client.PostAsJsonAsync(url, value);
    }

    public async static Task<T> GetResponseAsync<T>(this HttpClient client, string url)
    {
        return await client.GetFromJsonAsync<T>(url);   
    }
}