using Infrastructure.IService;
using Infrastructure.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared;
using System.Data;
using System.Security.Claims;

namespace Api.Controllers
{
    [Authorize(Roles = "Admin,Student,Employee")]
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        private int CurrentUserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        private Role CurrentUserRole => Enum.Parse<Role>(User.FindFirstValue(ClaimTypes.Role)!, ignoreCase: true);

        [HttpGet("GetSurvey")]

        public async Task<IActionResult> AllSurvey()
        {
            return Ok(await _userService.GetAllSurvey(CurrentUserId, CurrentUserRole));
        }
        [HttpGet("GetResponse")]

        public async Task<IActionResult> AllResponse()
        {
            return Ok(await _userService.GetAllResponse(CurrentUserId, CurrentUserRole));
        }

        [HttpGet("surveys/{surveyId:int}/getuserquestions")]
        public async Task<IActionResult> GetUserAllQuestion(int surveyId)
        {
            var question = await _userService.GetUserSurveyQuestion(surveyId);
            return Ok(question);
        }

        [HttpGet("questions/{questionId:int}/Useroptions")]
        public async Task<IActionResult> GetUserAllOption(int questionId)
        {
            var option = await _userService.GetUserOption(questionId);
            return Ok(option);
        }

        [HttpGet("{responseId:int}")]
        public async Task<ActionResult<ResponseDetailsDto>> GetResponseDetails(int responseId)
        {
            var dto = await _userService.GetResponseDetailsAsync(responseId, CurrentUserId, CurrentUserRole);
            if (dto == null) return NotFound();

            return Ok(dto);
        }

        [HttpGet("surveys/{surveyId:int}/SurveyById")]
        public async Task<IActionResult> GetSurveyById(int surveyId)
        {
            var survey = await _userService.GetSurveyId(surveyId);
            return Ok(survey);
        }
    }

}
