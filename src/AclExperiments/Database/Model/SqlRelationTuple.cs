using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AclExperiment.CheckExpand.Database.Model
{
    public record SqlRelationTuple
    {
        /// <summary>
        /// Gets or sets the Object Namespace.
        /// </summary>
        public required string Namespace { get; set; }

        /// <summary>
        /// Gets or sets the Object ID.
        /// </summary>
        public required string Object { get; set; }

        /// <summary>
        /// Gets or sets the Object Relation to a Subject.
        /// </summary>
        public required string Relation { get; set; }

        /// <summary>
        /// Gets or sets Subject Namespace.
        /// </summary>
        public required string Subject { get; set; }
    }
}
