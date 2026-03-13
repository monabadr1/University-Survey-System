using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public class ResponseDetailsDto
    {
        public int ResponseId { get; set; }
        public int SurveyId { get; set; }
        public string SurveyTitle { get; set; } = "";
        public string? First_name { get; set; }
        public int UserId { get; set; }
        public DateTime SubmittedAt { get; set; }
        public List<ResponseQuestionDto> Items { get; set; } = new();
    }
}
