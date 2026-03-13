using Infrastructure.IService;
using Microsoft.EntityFrameworkCore;
using Shared;
using System;
using System.Collections.Generic;
using System.Text;
using Domian.Entites;

namespace Infrastructure.Service
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;
        public UserService(AppDbContext context)
        {
            _context = context;
        }
        public async Task<List<SurveyDto>> GetAllSurvey(int userId, Role role)
        {
            var now = DateTime.Now;
            var surveys = _context.surveys.AsNoTracking()
                .Where(s => s.Statue == Statues.Published)
                .Where(s => s.StartDate <= now && s.EndDate >= now);

            if (role == Role.Student)
                surveys = surveys.Where(s => s.TargetGroups == TargetGroups.Student);
            else if (role == Role.Employee)
                surveys = surveys.Where(s => s.TargetGroups == TargetGroups.Employee);

            return await surveys.Select(s => new SurveyDto
            {
                SurveyId = s.SurveyId,
                Title = s.Title,
                Description = s.Description,
                StartDate = s.StartDate,
                EndDate = s.EndDate,
                TargetGroups = s.TargetGroups,
            }).ToListAsync();
        }

        public async Task<List<ResponseDto>> GetAllResponse(int userId, Role role)
        {
            var response = _context.responses.AsNoTracking()
                .Include(r=>r.survey)
                .Include(r=>r.users)
                .AsQueryable();

            if (role != Role.Admin)
                response = response.Where(r => r.UserId == userId);

            return await response.Where(r=>r.statue==Statue.sumbitted)
                .OrderByDescending(r=>r.SubmittedAt)

                .Select(r => new ResponseDto
            {
                ResponseId = r.ResponseId,
                Title = r.Title,
                SurveyId = r.SurveyId,
                SubmittedAt = r.SubmittedAt,
                UserId = r.UserId,
                First_name=r.users!=null? r.users.First_name:null
            }

                ).ToListAsync();

        }

        public async Task<List<QuestionDto>> GetUserSurveyQuestion(int SurveyId)
        {
            var question = _context.questions.AsNoTracking().Where(s => s.SurveyId == SurveyId);

            return await question.Select(r => new QuestionDto
            {
                SurveyId = r.SurveyId,
                questiontext = r.questiontext,
                isrquired = r.isrquired,
                isoredered = r.isoredered,
                QuestionId = r.QuestionID,
                Type = r.Type
            }

                ).ToListAsync();
        }
        public async Task<List<QuestionOptionDto>> GetUserOption(int QuestionId)
        {
            var option = _context.questionOptions.AsNoTracking().Where(q => q.QuestionId == QuestionId);

            return await option.Select(r => new QuestionOptionDto
            {
                OptionId = r.OptionId,
                QuestionId = r.QuestionId,
                Text = r.Text,
                value = r.value,
                isordered = r.isordered

            }).ToListAsync();
        }
        public async Task<ResponseDetailsDto?> GetResponseDetailsAsync(int responseId ,int currentUserId, Role currentRole)
        {
            var response = await _context.responses
                .AsNoTracking()
                .Include(r => r.survey)
                .Include(r=>r.users)
                .FirstOrDefaultAsync(r => r.ResponseId == responseId);

            if (response == null) return null;
            if (currentRole != Role.Admin && response.UserId != currentUserId)
                throw new UnauthorizedAccessException("You are not allowed to view this response.");

            var questions = await _context.questions
                .AsNoTracking()
                .Where(q => q.SurveyId == response.SurveyId)
                .OrderBy(q => q.isoredered)
                .ToListAsync();

            var answers = await _context.answers
                .AsNoTracking()
                .Where(a => a.ResponseId == responseId)
                .ToListAsync();

            var optionIds = answers
                .Where(a => a.SelectedOptionId != null)
                .Select(a => a.SelectedOptionId!.Value)
                .Distinct()
                .ToList();

            var options = await _context.questionOptions
                .AsNoTracking()
                .Where(o => optionIds.Contains(o.OptionId))
                .ToDictionaryAsync(o => o.OptionId, o => o.Text);

            var dto = new ResponseDetailsDto
            {
                ResponseId = response.ResponseId,
                SurveyId = response.SurveyId,
                SurveyTitle = response.survey?.Title ?? $"Survey #{response.SurveyId}",
                UserId = response.UserId ?? 0,
                SubmittedAt = response.SubmittedAt ,
                First_name=response.users?.First_name,
                
                Items = questions.Select(q =>
                {
                    var qAnswers = answers.Where(a => a.QuestionId == q.QuestionID).ToList();

                    var texts = new List<string>();
                    foreach (var a in qAnswers)
                    {
                        if (q.Type == QuestionType.Text)
                        {
                            if (!string.IsNullOrWhiteSpace(a.ValueText))
                                texts.Add(a.ValueText);
                        }
                        else
                        {
                            if (a.SelectedOptionId != null &&
                                options.TryGetValue(a.SelectedOptionId.Value, out var optText))
                                texts.Add(optText);
                        }
                    }

                    return new ResponseQuestionDto
                    {
                        QuestionId = q.QuestionID,
                        QuestionText = q.questiontext,
                        Type = q.Type,
                        IsRequired = q.isrquired,
                        Answers = texts
                    };
                }).ToList()
            };

            return dto;
        }
        public async Task<SurveyDto?> GetSurveyId(int surveyId)
        {
            return await _context.surveys.AsNoTracking().Where(s => s.SurveyId == surveyId)

           .Select(s => new SurveyDto
           {
               SurveyId = s.SurveyId,
               Title = s.Title,
           }


            ).FirstOrDefaultAsync();
        }


    }
}
