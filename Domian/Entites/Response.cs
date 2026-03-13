using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared;

namespace Domian.Entites
{
    public class Response
    {
        public int ResponseId { get; set; }

        public int SurveyId { get; set; }
        public string? Title { get; set; }
        public Survey? survey { get; set; }
        public Statue statue { get; set; }

        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;

        public int? UserId { get; set; }

        public User? users { get; set; }

        public ICollection<Answer> answers { get; set; } = new List<Answer>();

    }
    public enum Statue
    {
        sumbitted = 1,
        InProgress = 2
    }
}
