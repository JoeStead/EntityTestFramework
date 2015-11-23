using System;
using Microsoft.Data.Entity;

namespace EntityTestFramework.Tests.TestHelpers
{
    public class TestDbContext : DbContext
    {
        public DbSet<TestRecord> TestRecords { get; set; }

    }

    public class TestRecord
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
