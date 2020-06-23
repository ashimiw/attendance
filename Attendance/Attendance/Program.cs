using Attendance.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Attendance
{
    public class Program
    {
        static void Main(string[] args)
        {

            string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"AttendanceDevice.json");

            var myJsonString = File.ReadAllText(path);
            List<AttendanceDevice> attendanceDevice = new List<AttendanceDevice>();
            var format = "yyyy’-‘MM’-‘dd’T’HH’:’mm’:’ss"; // your datetime format
            var dateTimeConverter = new IsoDateTimeConverter { DateTimeFormat = format };

            attendanceDevice = JsonConvert.DeserializeObject<List<AttendanceDevice>>(myJsonString, dateTimeConverter);

            
           var monthlyAttendances = GetMonthlyAttendance(attendanceDevice.OrderBy(a => a.EmployeeId).ThenBy(a => a.EntryDate).ToList());

            CalculateAverageHour(monthlyAttendances);
        }
        public static List<MonthlyAttendance> GetMonthlyAttendance(List<AttendanceDevice> attendanceDevice)
        {
            var loginStaus = false;
            DateTime loginTime = DateTime.Now;
            double totalHrs = 0;
            string employeeId = "";

            string date = "";

            

            List<MonthlyAttendance> monthlyAttendances = new List<MonthlyAttendance>();
            foreach (var attendance in attendanceDevice)
            {
                if (String.IsNullOrEmpty(employeeId))
                {
                    employeeId = attendance.EmployeeId.ToString();
                    date = attendance.EntryDate.ToShortDateString();
                }
                else if (employeeId != attendance.EmployeeId.ToString() || date != attendance.EntryDate.ToShortDateString())
                {
                    MonthlyAttendance monthlyAttendance = new MonthlyAttendance()
                    {
                        EmployeeId = employeeId,
                        AttendanceDate = attendance.EntryDate,
                        TotalHours = totalHrs
                    };
                    monthlyAttendances.Add(monthlyAttendance);
                    totalHrs = 0;
                    employeeId = attendance.EmployeeId.ToString();
                    date = attendance.EntryDate.ToShortDateString();
                    
                }
                if (!loginStaus)
                {

                    loginTime = attendance.EntryDate;
                    loginStaus = true;
                }
                else
                {
                    totalHrs += (attendance.EntryDate - loginTime).TotalHours;
                    loginStaus = false;
                }

                if (attendanceDevice.IndexOf(attendance) == attendanceDevice.Count - 1)
                {
                    MonthlyAttendance monthlyAttendance = new MonthlyAttendance()
                    {
                        EmployeeId = employeeId,
                        AttendanceDate = attendance.EntryDate,
                        TotalHours = totalHrs
                    };
                    monthlyAttendances.Add(monthlyAttendance);

                }
            }
            return monthlyAttendances;
        }

        public static void CalculateAverageHour(List<MonthlyAttendance> monthlyAttendances)
        {
            
            //This extracts the average of number of working hours that the employee has worked for. 
            //for instance if the employee has worked for 5 days it gives the average result of working hour of 5 days.
            var results = monthlyAttendances.GroupBy(i => i.EmployeeId)
                          .Select(g => new
                          {
                              EmployeeId = g.Key,
                              Average = g.Average(a => a.TotalHours)
                          });
            foreach (var result in results.OrderBy(a => a.Average))
            {
                Console.WriteLine("{0}  {1}", result.EmployeeId, result.Average);
            }

            Console.ReadLine();
        }
    }
}
