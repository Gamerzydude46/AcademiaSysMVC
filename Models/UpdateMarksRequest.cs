﻿namespace AcademiaSys.Models
{
    public class UpdateMarksRequest
    {
        public int? Id { get; set; }
        public int? Mat { get; set; }
        public int? Eng { get; set; }
        public int? Sci { get; set; }
        public int? His { get; set; }
        public int? Geo { get; set; }
        public int? Lab { get; set; }
    }
}
