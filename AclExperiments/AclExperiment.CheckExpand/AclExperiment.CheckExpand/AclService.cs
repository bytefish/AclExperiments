using AclExperiment.CheckExpand.Expressions;
using AclExperiment.CheckExpand.Stores;
using Microsoft.Extensions.Logging;

namespace AclExperiment.CheckExpand
{
    /// <summary>
    /// The <see cref="AclService"/> implements the Expand API, Check API and ListObjects API of 
    /// the Google Zanzibar Paper.
    /// </summary>
    public class AclService
    {
        private readonly ILogger _logger;
        private readonly IRelationTupleStore _relationTupleStore;
        private readonly INamespaceConfigurationStore _namespaceConfigurationStore;


        public AclService(ILogger<AclService> logger, IRelationTupleStore relationTupleStore, INamespaceConfigurationStore namespaceConfigurationStore) 
        { 
            _logger = logger;
            _relationTupleStore = relationTupleStore;
            _namespaceConfigurationStore = namespaceConfigurationStore;
        }

        public bool CheckAsync()
        {

        }


    }
}