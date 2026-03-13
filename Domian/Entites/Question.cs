using Shared;
namespace Domian.Entites
{
    public class Question
    {
        public int QuestionID { get; set; }
        public int SurveyId { get; set; }
        public Survey? Survey { get; set; }

        public string? questiontext { get; set; }

        public QuestionType Type { get; set; }
        public bool isrquired { get; set; }

        public int isoredered { get; set; }

        public ICollection<Answer> answers { get; set; } = new List<Answer>();
        public ICollection<QuestionOption> Questionoption { get; set; } = new List<QuestionOption>();

    }
}
