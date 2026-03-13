using System;
using System.Collections.Generic;
using System.Text;

namespace Shared
{
    public class AnswerDto
    {
        public int AnswerId { get; set; }

        public int ResponseId { get; set; }

        public int QuestionId { get; set; }

        public int? SelectedOptionId { get; set; }

        public string? ValueText { get; set; }
    }
}
