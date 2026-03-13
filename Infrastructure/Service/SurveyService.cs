using Domian.Entites;
 using Infrastructure.IService;
using Microsoft.EntityFrameworkCore;
using Shared;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Infrastructure.Service
{
    public class SurveyService : ISurveyService
    {
        private readonly AppDbContext _context;

        public SurveyService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<int> AddSurvey(SurveyDto dto)
        {
            var survey = new Survey
            {
                Title = dto.Title,
                Description = dto.Description,
                TargetGroups = dto.TargetGroups,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                Allowmultiblesubmission = dto.Allowmultiblesubmission,
                isAnonymous = dto.IsAnonymous,
                Statue = Statues.Draft
            };
            _context.surveys.Add(survey);
            await _context.SaveChangesAsync();

            return survey.SurveyId;
        }

        public async Task<int> AddQuestion(int surveyId, CreateQuestionDto dto)

        {
            var survey = await _context.surveys.FirstOrDefaultAsync(s => s.SurveyId == surveyId);
            if (survey == null)
                throw new InvalidOperationException("Survey not found.");

            if (survey.Statue != Statues.Draft)
                throw new InvalidOperationException("You can only add questions when the survey is in Draft status.");
            var question = new Question
            {
                SurveyId = surveyId,
                questiontext = dto.questiontext,
                Type = dto.Type,

            };

            _context.questions.Add(question);
            await _context.SaveChangesAsync();
            return question.QuestionID;

        }
        public async Task<int> AddQuestionOption(QuestionOptionDto dto)
        {
            var question = await _context.questions.FirstOrDefaultAsync(q => q.QuestionID == dto.QuestionId);
            if (question == null)
                throw new InvalidOperationException("Question not found");
            var survey = await _context.surveys.FirstOrDefaultAsync(s => s.SurveyId == question.SurveyId);
            if (survey == null)
                throw new InvalidOperationException("Survey not found");
            if (survey.Statue != Statues.Draft)
                throw new InvalidOperationException("You can only add options when the survey is in draft status");

            if (question.Type != QuestionType.SingleChoice && question.Type != QuestionType.MultipleChoice)
                throw new InvalidOperationException("Options can only added to singleChoice or MultipleChoice questions .");

            var option = new QuestionOption
            {
                QuestionId = dto.QuestionId,
                Text = dto.Text,
                value = dto.value,
                isordered = dto.isordered

            };
            _context.questionOptions.Add(option);
            await _context.SaveChangesAsync();
            return option.OptionId;
        }
        public async Task PublishSurvey(PublisherSurvey dto)
        {
            var survey = await _context.surveys.FirstOrDefaultAsync(s => s.SurveyId == dto.SurveyId);
            if (survey == null)
                throw new InvalidOperationException("Survey not found.");

            if (survey.Statue != Statues.Draft)
                throw new InvalidOperationException("Only Draft surveys can be published. ");

            var questions = await _context.questions
                .Where(q => q.SurveyId == dto.SurveyId)
                .ToListAsync();

            if (questions.Count == 0)
                throw new InvalidOperationException("cannot publish a survey with no questions");

            var choiceQuestionIds = questions
                .Where(q => q.Type == QuestionType.SingleChoice || q.Type == QuestionType.MultipleChoice)
                .Select(q => q.QuestionID)
                .ToList();

            if (choiceQuestionIds.Count > 0)
            {
                var optionsByQuestion = await _context.questionOptions
                    .Where(o => choiceQuestionIds.Contains(o.QuestionId))
                    .GroupBy(o => o.QuestionId)
                    .Select(g => new { QustionId = g.Key, count = g.Count() })
                    .ToListAsync();

                var questionIdwithOptions = optionsByQuestion
                    .Where(x => x.count > 0)
                    .Select(x => x.QustionId)
                    .ToHashSet();

                var missingOptions = choiceQuestionIds
                    .Where(id => !questionIdwithOptions.Contains(id))
                    .ToList();
                if (missingOptions.Count > 0)
                    throw new InvalidOperationException("cannot publish : some choice question have no options");


            }
            survey.Statue = Statues.Published;
            await _context.SaveChangesAsync();

        }
        public async Task ClosedSurvey(ClosedSurvey dto)
        {
            var survey = await _context.surveys.FirstOrDefaultAsync(s => s.SurveyId == dto.SurveyId);
            if (survey == null)
                throw new InvalidOperationException("Survey not found.");

            if (survey.Statue != Statues.Published)
                throw new InvalidOperationException("Only Published surveys can be closed. ");

            survey.Statue = Statues.closed;
            await _context.SaveChangesAsync();

        }

        public async Task<List<QuestionDto>> GetSurveyQuestion(int SurveyId)
        {
            var question = _context.questions.AsNoTracking().Where(s => s.SurveyId == SurveyId);

            return await question.Select(r => new QuestionDto
            {
                SurveyId = r.SurveyId,
                questiontext = r.questiontext,
                isrquired = r.isrquired,
                isoredered = r.isoredered,
                QuestionId=r.QuestionID,
                Type = r.Type
            }

                ).ToListAsync();
        }

        public async Task<List<QuestionOptionDto>> GetOption(int QuestionId)
        {
            var option = _context.questionOptions.AsNoTracking().Where(q => q.QuestionId == QuestionId);

            return await option.Select(r => new QuestionOptionDto
            {
                QuestionId = r.QuestionId,
                Text = r.Text,
                value = r.value,
                isordered = r.isordered

            }).ToListAsync();
        }

    }
}
