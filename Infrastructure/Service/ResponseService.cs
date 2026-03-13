using Infrastructure.IService;
using Shared;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Domian.Entites;
namespace Infrastructure.Service
{
    public class ResponseService : IResponseService
    {
        private readonly AppDbContext _context;

        public ResponseService(AppDbContext context)
        {
            _context = context;
        }
        public async Task<int> CreateResponse(int surveyId, int userId, Role userRole)
        {
            var survey = await _context.surveys.FirstOrDefaultAsync(s => s.SurveyId == surveyId);
            if (survey == null)
                throw new InvalidOperationException("Survey not found.");

            if (survey.Statue != Statues.Published)
                throw new InvalidOperationException("You can't make Response When the survey is not published.");

            if (userRole == Role.Admin)
                throw new InvalidOperationException("Admin cannot create responses");

            if (survey.TargetGroups == TargetGroups.Student && userRole != Role.Student)
                throw new InvalidOperationException("This survey is for students only");

            if (survey.TargetGroups == TargetGroups.Employee && userRole != Role.Employee)
                throw new InvalidOperationException("This survey is for employee only");

            var now = DateTime.UtcNow;
            if (now < survey.StartDate) throw new InvalidOperationException("Survey has not started yet");
            if (now > survey.EndDate) throw new InvalidOperationException("Survey has ended");

            if (!survey.Allowmultiblesubmission)
            {
                var alreadySubmitted = await _context.responses.AnyAsync
                    (
                    r => r.SurveyId == surveyId &&
                    r.UserId == userId &&
                    r.statue == Statue.sumbitted);
                if (alreadySubmitted)
                    throw new InvalidOperationException("You already submitted this survey.");
            }

            var existing = await _context.responses.FirstOrDefaultAsync(r =>
               r.SurveyId == surveyId &&
               r.UserId == userId &&
               r.statue == Statue.InProgress
               
               
               );

            if (existing != null)
                return existing.ResponseId;

            var response = new Response
            {
                SurveyId = surveyId,
                UserId = userId,
                statue = Statue.InProgress,
                Title=survey.Title
            };
            _context.responses.Add(response);
            await _context.SaveChangesAsync();
            return response.ResponseId;
        }
        public async Task<int> AddAnswers(int responseId, AnswerDto dto, int userId)
        {
            // Check if response exists
            var response = await _context.responses.FirstOrDefaultAsync(r => r.ResponseId == responseId);
            if (response == null)
                throw new InvalidOperationException("Response not found.");

            if (response.statue != Statue.InProgress)
                throw new InvalidOperationException("You can only add answers when response is in progress.");

            if (response.UserId != null && response.UserId != userId)
                throw new InvalidOperationException("You are not allowed to modify this response.");

            // Check if the survey exists and is published
            var survey = await _context.surveys.FirstOrDefaultAsync(s => s.SurveyId == response.SurveyId);
            if (survey == null)
                throw new InvalidOperationException("Survey not found.");

            if (survey.Statue != Statues.Published)
                throw new InvalidOperationException("This survey is not accepting answers.");

            var now = DateTime.UtcNow;
            if (now < survey.StartDate) throw new InvalidOperationException("Survey has not started yet.");
            if (now > survey.EndDate) throw new InvalidOperationException("Survey has ended.");

            // Check if the question exists
            var question = await _context.questions.FirstOrDefaultAsync(q => q.QuestionID == dto.QuestionId);
            if (question == null)
                throw new InvalidOperationException("Question not found.");

            if (question.SurveyId != response.SurveyId)
                throw new InvalidOperationException("This question does not belong to the same survey.");

            // Handle multiple choice / single choice
            if (question.Type == QuestionType.SingleChoice || question.Type == QuestionType.MultipleChoice)
            {
                if (dto.SelectedOptionId == null)
                    throw new InvalidOperationException("SelectedOptionId is required for choice questions.");

                var optionExists = await _context.questionOptions.AnyAsync(o =>
                    o.OptionId == dto.SelectedOptionId && o.QuestionId == dto.QuestionId);

                if (!optionExists)
                    throw new InvalidOperationException("Selected option does not belong to this question.");

                var duplicated = await _context.answers
                    .AnyAsync(a =>
                        a.ResponseId == responseId &&
                        a.QuestionId == dto.QuestionId &&
                        a.SelectedOptionId == dto.SelectedOptionId
                    );
                if (duplicated)
                    throw new InvalidOperationException("This option is already selected.");
            }
            else if (question.Type == QuestionType.Text)
            {
                // For Text questions, do not use SelectedOptionId, use ValueText instead
                if (string.IsNullOrWhiteSpace(dto.ValueText))
                    throw new InvalidOperationException("ValueText is required for text questions.");
            }
            else if (question.Type == QuestionType.YesNo)
            {
                if (dto.ValueText != "true" && dto.ValueText != "false")
                    throw new InvalidOperationException("YesNo requires ValueText to be 'true' or 'false'.");
            }
            else if (question.Type == QuestionType.Rating)
            {
                if (!int.TryParse(dto.ValueText, out var rating) || rating < 1 || rating > 5)
                    throw new InvalidOperationException("Rating must be a number between 1 and 5.");
            }

            // Create the answer record
            var answer = new Answer
            {
                ResponseId = responseId,
                QuestionId = dto.QuestionId,
                ValueText = dto.ValueText,
                SelectedOptionId = dto.SelectedOptionId

            };

            _context.answers.Add(answer);
            await _context.SaveChangesAsync();
            return answer.AnswersId;
        }

        public async Task publishResponse(int responsId, int userId)
        {
            var response = await _context.responses.FirstOrDefaultAsync(r => r.ResponseId == responsId);
            if (response == null)
                throw new InvalidOperationException("Survey not found.");

            if (response.statue != Statue.InProgress)
                throw new InvalidOperationException("Your response is not in progress ");

            if (response.UserId != userId)
                throw new InvalidOperationException("You are not allowed to submit this response.");

            var survey = await _context.surveys.FirstOrDefaultAsync(s => s.SurveyId == response.SurveyId);
            if (survey == null)
                throw new InvalidOperationException("Survey not found.");

            if (survey.Statue != Statues.Published)
                throw new InvalidOperationException("This survey is not accepting submissions.");

            var now = DateTime.UtcNow;
            if (now < survey.StartDate) throw new InvalidOperationException("Survey has not started yet.");
            if (now > survey.EndDate) throw new InvalidOperationException("Survey has ended.");

            var hasanswer = await _context.answers.AnyAsync(r => r.ResponseId == responsId);
            if (!hasanswer)
                throw new InvalidOperationException("survey not have answer  ");

            response.statue = Statue.sumbitted;
            response.SubmittedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

        }

    }
}
