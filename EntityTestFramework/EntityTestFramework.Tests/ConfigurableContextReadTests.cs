using System.Collections.Generic;
using System.Linq;
using EntityTestFramework.Tests.TestHelpers;
using Xunit;
using FluentAssertions;

namespace EntityTestFramework.Tests
{
    public class ConfigurableContextReadTests
    {
        [Fact]
        public void QueryTest()
        {
            var configContext = new ConfigurableContext<TestDbContext>(ctx =>
            {
                ctx.Setup(x => x.TestRecords, new TestRecord { Id = 1 }, new TestRecord { Id = 2 });
            });

            var results = DoSomethingWithContext(configContext);

            results.Should().NotBeNull();
        }

        private IEnumerable<TestRecord> DoSomethingWithContext(TestDbContext ctx)
        {
            return ctx.TestRecords.Where(x => x.Id == 1);
        }
    }
}
