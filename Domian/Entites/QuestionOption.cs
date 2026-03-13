using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domian.Entites
{
    public class QuestionOption
    {
        [Key]
        public int OptionId { get; set; }
        public int QuestionId { get; set; }
        public Question? question { get; set; }
        public string? Text { get; set; }
        public int? value { get; set; }
        public int isordered { get; set; }

        public ICollection<Answer> Answers { get; set; } = new List<Answer>();
    }
}
