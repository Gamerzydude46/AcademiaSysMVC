namespace AcademiaSys.Models
{
    public class Students
    {
        public int Id { get; set; }
        public string Fullname { get; set; }
        public string Email { get; set; }
        public string Std { get; set; }
        public long? Contact { get; set; }
        public DateTime DOB { get; set; }
        public char Gender { get; set; }
    }
}
