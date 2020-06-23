using System;
using System.Collections.Generic;
using System.Text;

namespace Attendance.Models
{
    public class MonthlyAttendance
    {
        public string EmployeeId { get; set; }
        public DateTime AttendanceDate { get; set; }
        public double TotalHours { get; set; }

    }
}
