using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domian.Entites
{
    public class Answer
    {
        [Key]
        public int AnswersId { get; set; }

        public int ResponseId { get; set; }
        public Response? response { get; set; }

        public int QuestionId { get; set; }
        public Question? question { get; set; }
        public int? SelectedOptionId { get; set; }
        public QuestionOption? SelectedOption { get; set; }
        public string? ValueText { get; set; }
    }
}
