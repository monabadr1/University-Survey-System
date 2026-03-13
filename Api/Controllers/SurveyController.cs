using Domian.Entites;
using Infrastructure;
using Infrastructure.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Shared;

namespace Api.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class SurveyController : ControllerBase
    {
        private readonly ISurveyService _surveyService;

        public SurveyController(ISurveyService surveyService)
        {
            _surveyService = surveyService;
        }

        [HttpPost("surveys")]

        public async Task<IActionResult> CreateSurvey([FromBody] SurveyDto dto)
        {
            var id = await _surveyService.AddSurvey(dto);
            return Ok(new { SurveyId = id });
        }
        [HttpPost("surveys/{surveyId:int}/questions")]
        public async Task<IActionResult> CreateQuestion([FromRoute] int surveyId, [FromBody] CreateQuestionDto dto)
        {
            var questionId = await _surveyService.AddQuestion(surveyId, dto);
            return Ok(new { QuestionId = questionId });

        }
        [HttpPost("questions/{questionId:int}/options")]
        public async Task<IActionResult> AddOption([FromRoute] int questionId, [FromBody] QuestionOptionDto dto)
        {
            dto.QuestionId = questionId;
            var id = await _surveyService.AddQuestionOption(dto);
            return Ok(new { OptionId = id });
        }
        [HttpPost("surveys/{surveyId:int}/publish")]
        public async Task<IActionResult> Publish([FromRoute] int surveyId)
        {
            await _surveyService.PublishSurvey(new PublisherSurvey { SurveyId = surveyId });
            return Ok("Published");
        }
        [HttpPost("surveys/{surveyId:int}/close")]
        public async Task<IActionResult> Closed([FromRoute] int surveyId)
        {
            await _surveyService.ClosedSurvey(new ClosedSurvey { SurveyId = surveyId });
            return Ok("closed Survey");
        }

        [HttpGet("surveys/{surveyId:int}/getquestions")]
        public async Task<IActionResult> GetAllQuestion(int surveyId)
        {
            var question = await _surveyService.GetSurveyQuestion(surveyId);
            return Ok(question);
        }

        [HttpGet("questions/{questionId:int}/options")]
        public async Task<IActionResult> GetAllOption(int questionId)
        {
            var option = await _surveyService.GetOption(questionId);
            return Ok(option);
        }



    }
}
