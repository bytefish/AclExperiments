using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AclExperiment.CheckExpand.Models
{
    /// <summary>
    /// Query.
    /// </summary>
    public record RelationTupleQuery
    {
        /// <summary>
        /// Gets or sets the Object.
        /// </summary>
        public required AclObject Object { get; set; }

        /// <summary>
        /// Gets or sets the Relations.
        /// </summary>
        public required string[] Relations { get; set; }

        /// <summary>
        /// Gets or sets the Subject.
        /// </summary>
        public AclSubject? Subject { get; set;; }
    }
}
