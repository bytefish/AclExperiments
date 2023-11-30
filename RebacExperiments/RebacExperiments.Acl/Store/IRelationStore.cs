using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RebacExperiments.Acl.Store
{
    public interface IRelationStore
    {
        /// <summary>
        /// Returns all Relations for a given Object.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<AclRelation> GetRelationTuplesAsync(AclRelation key);
    }
}
