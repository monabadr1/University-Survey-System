using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public class ResponseQuestionDto
    {
        public int QuestionId { get; set; }
        public string QuestionText { get; set; } = "";
        public QuestionType Type { get; set; }
        public bool IsRequired { get; set; }
        public List<string> Answers { get; set; } = new();
    }
}
