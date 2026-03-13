using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public class CreateQuestionDto
    {
        public string? questiontext { get; set; }
        public QuestionType Type { get; set; }
        public bool isrquired { get; set; }

    }
}
