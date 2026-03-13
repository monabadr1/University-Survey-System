using System;
using System.Collections.Generic;
using System.Text;

namespace Shared
{
    public class ResponseDto
    {
        public int ResponseId { get; set; }
        public string? Title { get; set; }

        public int SurveyId { get; set; }
        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;

        public int? UserId { get; set; }

        public string? First_name { get; set; }
    }
}
