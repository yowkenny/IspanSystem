﻿using System;
using System.Collections.Generic;

#nullable disable

namespace MSITTeam1Admin.Models
{
    public partial class TSubmittedAnswer
    {
        public int FSn { get; set; }
        public string FMemberAccount { get; set; }
        public string FSubjectId { get; set; }
        public int FQuestionId { get; set; }
        public string FSubmitAnswer { get; set; }
    }
}
