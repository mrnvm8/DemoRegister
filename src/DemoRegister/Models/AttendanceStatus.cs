namespace DemoRegister.Models
{
    public class AttendanceStatus
    {
        public string StatusType { get; set; } // e.g., "Present", "Leave", "Sick", "Unpaid Leave"
        public string AdditionalInfo { get; set; } // e.g., "Half Day", "Early Leave", etc.

        public AttendanceStatus()
        {
            
        }
    }
}
