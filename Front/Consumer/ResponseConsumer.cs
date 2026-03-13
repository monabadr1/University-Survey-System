using Shared;
using System.Net.Http.Json;
using System.Text.Json;
using static System.Net.WebRequestMethods;

namespace Front.Consumer
{
    public class ResponseConsumer
    {
        private readonly HttpClient _http;

        public ResponseConsumer(HttpClient http )
        {
           _http = http;
        }
        public async Task<int> CreateResponseAsync(int surveyId)
        {
            var res = await _http.PostAsJsonAsync("api/Response/response", new { SurveyId = surveyId });
            var body = await res.Content.ReadAsStringAsync();

            if (!res.IsSuccessStatusCode)
                throw new Exception(body); // هيبقى "You already submitted this survey."

            var data = JsonSerializer.Deserialize<ResponseIdResult>(body,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return data!.ResponseId;
        }

        public async Task<int> AddAnswerAsync(int responseId, AnswerDto dto)
        {
            var res = await _http.PostAsJsonAsync($"api/Response/response/{responseId}/answer", dto);
            var body = await res.Content.ReadAsStringAsync();

            if (!res.IsSuccessStatusCode)
                throw new Exception($"AddAnswer failed: {(int)res.StatusCode} - {body}");

            var data = JsonSerializer.Deserialize<AnswerIdResult>(body, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (data == null)
                throw new Exception($"AddAnswer returned invalid JSON: {body}");

            return data.AnswerId;
        }

        public async Task SubmitAsync(int responseId)
        {
            var res = await _http.PostAsync($"api/Response/response/{responseId}/publish", null);
            res.EnsureSuccessStatusCode();
        }
    }
}
