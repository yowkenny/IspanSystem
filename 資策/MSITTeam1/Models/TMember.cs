﻿using System;
using System.Collections.Generic;

#nullable disable

namespace MSITTeam1.Models
{
    public partial class TMember
    {
        public string FAccount { get; set; }
        public byte[] FPassword { get; set; }
        public byte[] FSalt { get; set; }
        public int? FMemberType { get; set; }
        public string FGuid { get; set; }
        public string FDateTime { get; set; }
    }
}
