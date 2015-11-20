namespace EntityTestFramework.Tests.TestHelpers
{
    internal class DbOperationsHelper
    {
        private readonly TestDbContext _context;

        public DbOperationsHelper(TestDbContext context)
        {
            _context = context;
        }

        public void CreateTestRecord(TestRecord record)
        {
            _context.TestRecords.Add(record);
            _context.SaveChanges();
        }

        public void CreateInvalidTestRecord(TestRecord record)
        {
            //some arbritary test that does nothing.
            if (record != null)
            {
                _context.SaveChanges();
            }
        }
    }
}
