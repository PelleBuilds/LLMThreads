using System.Text;
using System.Text.Json;

namespace ThreadMapLLM.Services
{
    public class OllamaApiService
    {
        private readonly HttpClient HttpClient;
        private readonly string url = "http://localhost:11434/api/generate";

        public OllamaApiService()
        {
            HttpClient = new HttpClient();
        }

        public async Task<string> Query(string query)
        {
            var payload = new
            {
                model = "gemma3:4b",
                prompt = query,
                stream = false
            };

            var jsonPayload = JsonSerializer.Serialize(payload);
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            try
            {
                
                var response = await HttpClient.PostAsync(url, content);
                var responseBody = await response.Content.ReadAsStringAsync();
                Console.WriteLine(responseBody);
                
                using JsonDocument doc = JsonDocument.Parse(responseBody);
                string assistantReply = doc.RootElement
                    .GetProperty("response")
                    .GetString()!;
                

                if (response.IsSuccessStatusCode)
                {
                    return assistantReply;
                }
                else
                {
                    return response.StatusCode + responseBody;
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

        }

    }
}
