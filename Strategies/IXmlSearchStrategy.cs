using StudentPerformanceAnalyzer.Models;

namespace StudentPerformanceAnalyzer.Strategies
{
    public interface IXmlSearchStrategy
    {
        IList<Student> Search(Stream xmlStream, SearchCriteria criteria);
    }
}