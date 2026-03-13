using Domian.Entites;
using Infrastructure.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared;
using System.Data;
using System.Security.Claims;

namespace Api.Controllers
{
    [Authorize(Roles = "Student,Employee")]
    [ApiController]
    [Route("api/[controller]")]
    public class ResponseController : ControllerBase

    {
        private readonly IResponseService _responseService;

        public ResponseController(IResponseService responseService)
        {

            _responseService = responseService;
        }
        private int CurrentUserId =>
            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        private Role CurrentUserRole =>
            Enum.Parse<Role>(User.FindFirstValue(ClaimTypes.Role)!, ignoreCase: true);
        [HttpPost("response")]

        public async Task<IActionResult> AddResponse([FromBody] CreateResponseDto dto)
        {
            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                var id = await _responseService.CreateResponse(dto.SurveyId, CurrentUserId, CurrentUserRole);
                return Ok(new { ResponseId = id });
            }catch(InvalidOperationException ex)
            {
                if (ex.Message.Contains("already submitted", StringComparison.OrdinalIgnoreCase))
                    return Conflict(ex.Message);

                return BadRequest(ex.Message);
            }

        }

        [HttpPost("response/{responseId:int}/answer")]
        public async Task<IActionResult> AddAnswer([FromRoute] int responseId, [FromBody] AnswerDto dto)

        {
            dto.ResponseId = responseId;
            var id = await _responseService.AddAnswers(responseId, dto, CurrentUserId);
            return Ok(new { AnswerId = id });
        }
        [HttpPost("response/{responseId:int}/publish")]

        public async Task<IActionResult> publish([FromRoute] int responseId)
        {
            await _responseService.publishResponse(responseId, CurrentUserId);
            return Ok("publish");
        }


    }
}
