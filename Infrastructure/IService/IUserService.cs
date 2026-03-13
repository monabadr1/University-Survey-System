using Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.IService
{
    public interface IUserService
    {
        public Task<List<SurveyDto>> GetAllSurvey(int userId, Role role);
        public Task<List<ResponseDto>> GetAllResponse(int userId, Role role);

        public Task<List<QuestionDto>> GetUserSurveyQuestion(int SurveyId);

        public Task<List<QuestionOptionDto>> GetUserOption(int QuestionId);
        public Task<ResponseDetailsDto?> GetResponseDetailsAsync(int responseId, int currentUserId, Role currentRole);

        public Task<SurveyDto> GetSurveyId(int surveyId);




    }
}
