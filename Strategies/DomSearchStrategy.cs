using System.Xml;
using StudentPerformanceAnalyzer.Models;

namespace StudentPerformanceAnalyzer.Strategies
{
    public class DomSearchStrategy : IXmlSearchStrategy
    {
        public IList<Student> Search(Stream xmlStream, SearchCriteria criteria)
        {
            var document = new XmlDocument();
            document.Load(xmlStream);
            var results = new List<Student>();

            var studentNodes = document.SelectNodes("//Student");
            if (studentNodes == null) return results;

            foreach (XmlNode node in studentNodes)
            {
                string faculty = node.Attributes?["Faculty"]?.Value ?? "";
                string department = node.Attributes?["Department"]?.Value ?? "";
                string course = node.Attributes?["Course"]?.Value ?? "";

                if (criteria.Matches(faculty, department, course))
                {
                    var student = new Student
                    {
                        FullName = node.Attributes?["FullName"]?.Value ?? "",
                        Faculty = faculty,
                        Department = department,
                        Course = course,
                        Group = node.Attributes?["Group"]?.Value ?? ""
                    };

                    var disciplineNodes = node.SelectNodes("Disciplines/Discipline");
                    if (disciplineNodes != null)
                    {
                        foreach (XmlNode dNode in disciplineNodes)
                        {
                            student.Disciplines.Add(new Discipline
                            {
                                Name = dNode.Attributes?["Name"]?.Value ?? "",
                                Grade = int.TryParse(dNode.Attributes?["Grade"]?.Value, out int g) ? g : 0,
                                Status = dNode.Attributes?["Status"]?.Value ?? ""
                            });
                        }
                    }
                    results.Add(student);
                }
            }
            return results;
        }
    }
}