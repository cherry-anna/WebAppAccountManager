﻿using System;
using System.ComponentModel.DataAnnotations;

namespace WebAppAccountManager.Dto
{
    public class PostReportDto
    {
        public int ProjectId { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime JobDate { get; set; }

        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:hh:mm}", ApplyFormatInEditMode = true)]
        public TimeSpan Duration { get; set; }
        public string Description { get; set; }
    }
}