using Shared;
using System.Net.Http.Json;
using System.Text.Json;

namespace Front.Consumer
{
    public class UserConsumer
    {
        private readonly HttpClient _http;

        public UserConsumer(HttpClient http) { 

            _http = http;
        
        }
        public async Task<List<SurveyDto>> AllSurvey()
           => await _http.GetFromJsonAsync<List<SurveyDto>>("api/User/GetSurvey")
              ?? new List<SurveyDto>();

        public async Task<List<ResponseDto>> AllResponse()
            => await _http.GetFromJsonAsync<List<ResponseDto>>("api/User/GetResponse")
               ?? new List<ResponseDto>();

        public async Task<List<QuestionDto>> GetUserSurveyQuestionAsync(int surveyId)
    => await _http.GetFromJsonAsync<List<QuestionDto>>($"api/User/surveys/{surveyId}/getuserquestions")
       ?? new List<QuestionDto>();

        public async Task<List<QuestionOptionDto>> GetUserQuestionOptionsAsync(int questionId)
=> await _http.GetFromJsonAsync<List<QuestionOptionDto>>($"api/User/questions/{questionId}/Useroptions")
?? new List<QuestionOptionDto>();

        public async Task<ResponseDetailsDto?> GetResponseDetailsAsync(int responseId)
        {
            return await _http.GetFromJsonAsync<ResponseDetailsDto>(
                $"api/User/{responseId}"
            );
        }

        public async Task<SurveyDto?> GetSurveyByIdAsync(int surveyId)
    => await _http.GetFromJsonAsync<SurveyDto>($"api/User/surveys/{surveyId}/SurveyById");
    }
}
