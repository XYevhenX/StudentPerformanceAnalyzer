namespace StudentPerformanceAnalyzer.Models
{
    public class Student
    {
        public string FullName { get; set; } = string.Empty;
        public string Faculty { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string Course { get; set; } = string.Empty;
        public string Group { get; set; } = string.Empty;

        public List<Discipline> Disciplines { get; set; } = new();
    }
}