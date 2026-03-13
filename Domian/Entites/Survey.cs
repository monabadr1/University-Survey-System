using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared;

namespace Domian.Entites
{
    public class Survey
    {
        public int SurveyId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public TargetGroups TargetGroups { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool Allowmultiblesubmission { get; set; } = false;

        public Statues Statue { get; set; }

        public bool isAnonymous { get; set; } = false;
    }
}
