using Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.IService
{
    public interface ISurveyService
    {
        public Task<int> AddSurvey(SurveyDto dto);

        public Task<int> AddQuestion(int surveyId, CreateQuestionDto dto);

        public Task<int> AddQuestionOption(QuestionOptionDto dto);

        public Task PublishSurvey(PublisherSurvey dto);
        public Task ClosedSurvey(ClosedSurvey dto);

        public Task<List<QuestionDto>> GetSurveyQuestion(int SurveyId);

        public Task<List<QuestionOptionDto>> GetOption(int QuestionId);


    }
}
