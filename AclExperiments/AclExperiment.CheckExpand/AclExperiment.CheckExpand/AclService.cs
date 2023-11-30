using AclExperiment.CheckExpand.Stores;
using Microsoft.Extensions.Logging;

namespace AclExperiment.CheckExpand
{
    public class AclService
    {

        private readonly ILogger _logger;
        private readonly IRelationTupleStore _relationTupleStore;

        public AclService(ILogger<AclService> logger, IRelationTupleStore relationTupleStore) 
        { 
            _logger = logger;
            _relationTupleStore = relationTupleStore;
        }

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {
            Assert.Pass();
        }
    }
}