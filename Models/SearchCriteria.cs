namespace StudentPerformanceAnalyzer.Models
{
    public class SearchCriteria
    {
        public string? Faculty { get; set; }
        public string? Department { get; set; }
        public string? Course { get; set; }

        public bool Matches(string? faculty, string? department, string? course)
        {
            if (!string.IsNullOrEmpty(Faculty) && Faculty != faculty) return false;
            if (!string.IsNullOrEmpty(Department) && Department != department) return false;
            if (!string.IsNullOrEmpty(Course) && Course != course) return false;
            return true;
        }
    }
}