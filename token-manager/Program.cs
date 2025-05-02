using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Timers;

class Token
{
    public string AccessToken { get; set; }
    public DateTime ExpireAt { get; set; }
}

class Program
{
    static Token token;
    static int tokenCount = 0;
    static HttpClient client = new HttpClient();

    static async Task<string> GetToken()
    {
        if (token != null && token.ExpireAt > DateTime.UtcNow)
            return token.AccessToken;

        if (tokenCount >= 5)
            throw new Exception("Rate limittt");

        var res = await client.PostAsync("https://api.example.com/token", new StringContent(""));
        var body = await res.Content.ReadAsStringAsync();
        var json = JsonSerializer.Deserialize<JsonElement>(body);
        var accessToken = json.GetProperty("access_token").GetString();
        var expiresIn = json.GetProperty("expires_in").GetInt32();

        token = new Token
        {
            AccessToken = accessToken,
            ExpireAt = DateTime.UtcNow.AddSeconds(expiresIn)
        };

        tokenCount++;
        return token.AccessToken;
    }

    static async Task GetOrders()
    {
        try
        {
            var t = await GetToken();
            var req = new HttpRequestMessage(HttpMethod.Get, "https://api.example.com/orders");
            req.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", t);
            var res = await client.SendAsync(req);
            var data = await res.Content.ReadAsStringAsync();
            Console.WriteLine(data);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    static async Task Main(string[] args)
    {
        var orderTimer = new System.Timers.Timer(5 * 60 * 1000);
        orderTimer.Elapsed += async (s, e) => await GetOrders();
        orderTimer.Start();

        var resetTimer = new System.Timers.Timer(60 * 60 * 1000);
        resetTimer.Elapsed += (s, e) => tokenCount = 0;
        resetTimer.Start();

        await GetOrders();
        await Task.Delay(-1);
    }
}
