using Group6.NET1704.SW392.AIDiner.Common.Request;
using Group6.NET1704.SW392.AIDiner.Common.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;

namespace Group6.NET1704.SW392.AIDiner.Services.Implementation
{
    public class GeminiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private const string BaseUrl = "https://generativelanguage.googleapis.com/v1beta/tunedModels/dishhub-ehe7hbwueb2e:generateContent"; // Thay đổi theo mô hình và endpoint bạn sử dụng

        public GeminiService(string apiKey) // Sử dụng DI nếu có thể.
        {
            _httpClient = new HttpClient();
            _apiKey = apiKey;
        }

        public async Task<GeminiResponse> ProcessUserMessage(string message)
        {
            var payload = new
            {
                contents = new[]
                {
        new
        {
            parts = new[]
            {
                new
                {
                    text = message
                }
            }
        }
    },
                generationConfig = new
                {
                    temperature = 0
                }
            };

            string jsonPayload = JsonConvert.SerializeObject(payload);  
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            string url = $"{BaseUrl}?key={_apiKey}";

            HttpResponseMessage response = await _httpClient.PostAsync(url, content);

            string responseContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                string jsonResponse = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Full Gemini Response: {jsonResponse}");

                dynamic responseObject = JsonConvert.DeserializeObject(jsonResponse);

                string geminiOutput = responseObject.candidates[0].content.parts[0].text.ToString().Trim();
                geminiOutput = geminiOutput.Replace("`", "").Trim();

                // Tìm vị trí của dấu ngoặc nhọn mở đầu JSON
                int jsonStartIndex = geminiOutput.LastIndexOf('{');

                string responseText = "";
                string jsonString = "";

                if (jsonStartIndex != -1)
                {
                    // Tách chuỗi
                    responseText = geminiOutput.Substring(0, jsonStartIndex).Trim();
                    jsonString = geminiOutput.Substring(jsonStartIndex);
                }
                else
                {
                    // Nếu không tìm thấy JSON, coi toàn bộ là ResponseText và Intent là Unknown
                    responseText = geminiOutput.Trim();
                    return new GeminiResponse { Intent = "Unknown", ResponseText = responseText };
                }


                Console.WriteLine($"Response Text: {responseText}");
                Console.WriteLine($"JSON String: {jsonString}");

                try
                {
                    // Sửa JSON bằng biểu thức chính quy
                    string fixedJsonString = Regex.Replace(jsonString, @"(\w+):", @"""$1"":");
                    fixedJsonString = "{" + fixedJsonString + "}"; // Thêm ngoặc nhọn nếu cần

                    JToken.Parse(fixedJsonString);

                    GeminiResponse geminiResponse = JsonConvert.DeserializeObject<GeminiResponse>(fixedJsonString);
                    geminiResponse.ResponseText = responseText; // Gán responseText
                    return geminiResponse;
                }
                catch (JsonReaderException ex)
                {
                    Console.WriteLine($"Invalid JSON: {ex.Message}");
                    Console.WriteLine($"Gemini Output: {geminiOutput}");
                    return new GeminiResponse { Intent = "Unknown", ResponseText = "Tôi xin lỗi, tôi không hiểu câu hỏi của bạn." };
                }
            }
            else
            {
                Console.WriteLine($"Error: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
                return new GeminiResponse { Intent = "Unknown", ResponseText = "Có lỗi xảy ra khi xử lý yêu cầu của bạn." };
            }
        }
    }
}
