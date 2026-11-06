using System.Xml.Linq;
using StudentPerformanceAnalyzer.Models;

namespace StudentPerformanceAnalyzer.Strategies
{
    public class LinqSearchStrategy : IXmlSearchStrategy
    {
        public IList<Student> Search(Stream xmlStream, SearchCriteria criteria)
        {
            var document = XDocument.Load(xmlStream);
            var results = new List<Student>();

            var students = document.Descendants("Student")
                .Where(e => criteria.Matches(
                    (string?)e.Attribute("Faculty"),
                    (string?)e.Attribute("Department"),
                    (string?)e.Attribute("Course")));

            foreach (var element in students)
            {
                var student = new Student
                {
                    FullName = (string?)element.Attribute("FullName") ?? "",
                    Faculty = (string?)element.Attribute("Faculty") ?? "",
                    Department = (string?)element.Attribute("Department") ?? "",
                    Course = (string?)element.Attribute("Course") ?? "",
                    Group = (string?)element.Attribute("Group") ?? ""
                };

                student.Disciplines = element.Descendants("Discipline").Select(d => new Discipline
                {
                    Name = (string?)d.Attribute("Name") ?? "",
                    Grade = (int?)d.Attribute("Grade") ?? 0,
                    Status = (string?)d.Attribute("Status") ?? ""
                }).ToList();

                results.Add(student);
            }

            return results;
        }
    }
}