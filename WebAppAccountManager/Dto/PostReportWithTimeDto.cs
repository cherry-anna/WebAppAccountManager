using System;
using System.ComponentModel.DataAnnotations;

namespace WebAppAccountManager.Dto
{
    public class PostReportWithTimeDto
    {
        public int ProjectId { get; set; }
        public int EmployeeId { get; set; }


        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime JobDate { get; set; }

        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:hh:mm}", ApplyFormatInEditMode = true)]
        public TimeSpan StartJobTime { get; set; }

        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:hh:mm}", ApplyFormatInEditMode = true)]
        public TimeSpan Duration { get; set; }
        public string Description { get; set; }
    }
}