using System.Diagnostics;
using StudentPerformanceAnalyzer.Models;

namespace StudentPerformanceAnalyzer.Strategies
{
    public class SearchTimingDecorator : IXmlSearchStrategy
    {
        private readonly IXmlSearchStrategy _innerStrategy;

        public TimeSpan LastExecutionTime { get; private set; }

        public SearchTimingDecorator(IXmlSearchStrategy innerStrategy)
        {
            _innerStrategy = innerStrategy;
        }

        public IList<Student> Search(Stream xmlStream, SearchCriteria criteria)
        {
            var stopwatch = Stopwatch.StartNew();

            var results = _innerStrategy.Search(xmlStream, criteria);

            stopwatch.Stop();
            LastExecutionTime = stopwatch.Elapsed;

            return results;
        }
    }
}