using System;
using System.Collections.Generic;
using System.Text;

namespace Shared
{
    public class QuestionOptionDto
    {
        public int QuestionId { get; set; }

        public int OptionId { get; set; }

        public string? Text { get; set; } 

        public int? value { get; set; }

        public int isordered { get; set; }
    }
}
