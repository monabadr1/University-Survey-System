using System;
using System.Collections.Generic;
using System.Text;

namespace Shared
{
    public class QuestionDto
    {
        public int SurveyId { get; set; }

        public int QuestionId { get; set; }

        public string? questiontext { get; set; }

        public QuestionType Type { get; set; }
        public bool isrquired { get; set; }

        public int isoredered { get; set; }

        public List<QuestionOptionDto> Options { get; set; } = new();


    }
}
