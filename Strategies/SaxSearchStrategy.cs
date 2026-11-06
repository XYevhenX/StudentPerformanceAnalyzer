using System.Xml;
using StudentPerformanceAnalyzer.Models;

namespace StudentPerformanceAnalyzer.Strategies
{
    public class SaxSearchStrategy : IXmlSearchStrategy
    {
        public IList<Student> Search(Stream xmlStream, SearchCriteria criteria)
        {
            var results = new List<Student>();
            using var reader = XmlReader.Create(xmlStream);

            Student? currentStudent = null;

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element && reader.Name == "Student")
                {
                    string faculty = reader.GetAttribute("Faculty") ?? "";
                    string department = reader.GetAttribute("Department") ?? "";
                    string course = reader.GetAttribute("Course") ?? "";

                    if (criteria.Matches(faculty, department, course))
                    {
                        currentStudent = new Student
                        {
                            FullName = reader.GetAttribute("FullName") ?? "",
                            Faculty = faculty,
                            Department = department,
                            Course = course,
                            Group = reader.GetAttribute("Group") ?? ""
                        };
                    }
                    else
                    {
                        currentStudent = null;
                    }
                }
                else if (reader.NodeType == XmlNodeType.Element && reader.Name == "Discipline" && currentStudent != null)
                {
                    currentStudent.Disciplines.Add(new Discipline
                    {
                        Name = reader.GetAttribute("Name") ?? "",
                        Grade = int.TryParse(reader.GetAttribute("Grade"), out int g) ? g : 0,
                        Status = reader.GetAttribute("Status") ?? ""
                    });
                }
                else if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "Student" && currentStudent != null)
                {
                    results.Add(currentStudent);
                    currentStudent = null;
                }
            }

            return results;
        }
    }
}