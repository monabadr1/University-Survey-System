using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Shared;

namespace Infrastructure.IService
{
    public interface IResponseService
    {
        Task<int> CreateResponse(int surveyId, int userId, Role userRole);

        Task<int> AddAnswers(int responseId, AnswerDto dto, int userId);
        Task publishResponse(int responsedId, int userId);
    }
}
