using Shared;
using System.Net.Http.Json;
namespace Front.Consumer
{
    public class SurveyConsumer
    {
        private readonly HttpClient _http;

        public SurveyConsumer(HttpClient http)
        {
            _http = http;
        }
        public async Task<int> CreateSurveyAsync(SurveyDto dto)
        {
            var survey = await _http.PostAsJsonAsync("api/Survey/surveys", dto);
            survey.EnsureSuccessStatusCode();

            var data = await survey.Content.ReadFromJsonAsync<SurveyIdResult>();
            return data!.SurveyId;
        }
        public async Task<int> AddQuestionAsync(int surveyId, CreateQuestionDto dto)
        {
            var result = await _http.PostAsJsonAsync($"api/Survey/surveys/{surveyId}/questions", dto);
            result.EnsureSuccessStatusCode();

            var data = await result.Content.ReadFromJsonAsync<QuestionIdResult>();
            return data!.QuestionId;
        }
        public async Task<int> AddOptionAsync(int questionId, QuestionOptionDto dto)
        {
            dto.QuestionId = questionId;

            var result = await _http.PostAsJsonAsync($"api/Survey/questions/{questionId}/options", dto);

            var body = await result.Content.ReadAsStringAsync();
            if (!result.IsSuccessStatusCode)
                throw new Exception($"API {(int)result.StatusCode}: {body}");

            var data = await result.Content.ReadFromJsonAsync<OptionaIdResult>();
            return data!.OptionId;
        }
        public async Task PublishSurveyAsync(int surveyId)
        {
            var res = await _http.PostAsJsonAsync($"api/Survey/surveys/{surveyId}/publish", new { SurveyId = surveyId });
            res.EnsureSuccessStatusCode();
        }

        public async Task CloseSurveyAsync(int surveyId)
        {
            var res = await _http.PostAsJsonAsync($"api/Survey/surveys/{surveyId}/close", new { SurveyId = surveyId });
            res.EnsureSuccessStatusCode();
        }

        public async Task<List<QuestionDto>> GetSurveyQuestionAsync(int surveyId)
            => await _http.GetFromJsonAsync<List<QuestionDto>>($"api/Survey/surveys/{surveyId}/getquestions")
               ?? new List<QuestionDto>();

        public async Task<List<QuestionOptionDto>> GetQuestionOptionsAsync(int questionId)
    => await _http.GetFromJsonAsync<List<QuestionOptionDto>>($"api/Survey/questions/{questionId}/options")
        ?? new List<QuestionOptionDto>();


    }
}

