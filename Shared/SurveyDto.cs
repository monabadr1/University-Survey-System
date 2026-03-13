using System;
using System.Collections.Generic;
using System.Text;

namespace Shared
{
    public class SurveyDto
    {
        public int SurveyId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public TargetGroups TargetGroups { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool Allowmultiblesubmission { get; set; } = false;
        public bool IsAnonymous { get; set; } = false;

        public Statues Statue { get; set; }
    }
}
