using System.Text;
using StudentPerformanceAnalyzer.Models;
using StudentPerformanceAnalyzer.Strategies;
using Xunit;

namespace StudentPerformanceAnalyzer.Tests
{
    public class SearchStrategiesTests
    {
        private const string SampleXml = """
            <?xml version="1.0" encoding="utf-8" ?>
            <Students>
              <Student FullName="Зюзін Євген" Faculty="ФКНК" Department="Інформаційних систем" Course="2" Group="К16">
                <Disciplines>
                  <Discipline Name="ООП" Grade="98" Status="Passed" />
                  <Discipline Name="Алгоритми" Grade="95" Status="Passed" />
                </Disciplines>
              </Student>
              <Student FullName="Шевченко Олександр" Faculty="ФКНК" Course="2" Group="К16">
                <Disciplines>
                  <Discipline Name="ООП" Grade="75" Status="Passed" />
                </Disciplines>
              </Student>
              <Student FullName="Коваленко Марія" Faculty="Економічний" Department="Економічної кібернетики" Course="3" Group="ЕК-31">
                <Disciplines>
                  <Discipline Name="Макроекономіка" Grade="88" Status="Passed" />
                </Disciplines>
              </Student>
            </Students>
            """;

        private static MemoryStream CreateXmlStream()
        {
            return new MemoryStream(Encoding.UTF8.GetBytes(SampleXml));
        }

        [Fact]
        public void AllStrategies_ShouldReturnIdenticalResults_ForSameCriteria()
        {
            var criteria = new SearchCriteria
            {
                Faculty = "ФКНК",
                Course = "2"
            };

            var saxStrategy = new SaxSearchStrategy();
            var domStrategy = new DomSearchStrategy();
            var linqStrategy = new LinqSearchStrategy();

            using var saxStream = CreateXmlStream();
            using var domStream = CreateXmlStream();
            using var linqStream = CreateXmlStream();

            var saxResults = saxStrategy.Search(saxStream, criteria);
            var domResults = domStrategy.Search(domStream, criteria);
            var linqResults = linqStrategy.Search(linqStream, criteria);

            Assert.Equal(2, saxResults.Count);
            Assert.Equal(saxResults.Count, domResults.Count);
            Assert.Equal(saxResults.Count, linqResults.Count);

            for (int i = 0; i < saxResults.Count; i++)
            {
                Assert.Equal(saxResults[i].FullName, domResults[i].FullName);
                Assert.Equal(saxResults[i].FullName, linqResults[i].FullName);
                Assert.Equal(saxResults[i].Department, domResults[i].Department);
                Assert.Equal(saxResults[i].Department, linqResults[i].Department);
                Assert.Equal(saxResults[i].Disciplines.Count, domResults[i].Disciplines.Count);
                Assert.Equal(saxResults[i].Disciplines.Count, linqResults[i].Disciplines.Count);
            }
        }

        [Fact]
        public void AllStrategies_ShouldFilterByOptionalAttribute_WhenSomeNodesLackIt()
        {
            var criteria = new SearchCriteria
            {
                Department = "Інформаційних систем"
            };

            var strategies = new IXmlSearchStrategy[]
            {
                new SaxSearchStrategy(),
                new DomSearchStrategy(),
                new LinqSearchStrategy()
            };

            foreach (var strategy in strategies)
            {
                using var stream = CreateXmlStream();
                var results = strategy.Search(stream, criteria);

                Assert.Single(results);
                Assert.Equal("Зюзін Євген", results[0].FullName);
            }
        }

        [Fact]
        public void TimingDecorator_ShouldMeasureExecutionTimeAndPreserveResults()
        {
            var criteria = new SearchCriteria { Faculty = "Економічний" };
            var decorator = new SearchTimingDecorator(new LinqSearchStrategy());

            using var stream = CreateXmlStream();
            var results = decorator.Search(stream, criteria);

            Assert.Single(results);
            Assert.Equal("Коваленко Марія", results[0].FullName);
            Assert.True(decorator.LastExecutionTime >= TimeSpan.Zero);
        }
    }
}