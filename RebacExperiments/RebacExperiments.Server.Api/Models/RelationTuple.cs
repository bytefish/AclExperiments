// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace RebacExperiments.Server.Api.Models
{
    public class RelationTuple : Entity
    {
        /// <summary>
        /// Gets or sets the ObjectKey.
        /// </summary>
        public int ObjectKey { get; set; }

        /// <summary>
        /// Gets or sets the ObjectNamespace.
        /// </summary>
        public required string ObjectNamespace { get; set; }

        /// <summary>
        /// Gets or sets the ObjectRelation.
        /// </summary>
        public required string ObjectRelation { get; set; }

        /// <summary>
        /// Gets or sets the SubjectKey.
        /// </summary>
        public required int SubjectKey { get; set; }

        /// <summary>
        /// Gets or sets the SubjectNamespace.
        /// </summary>
        public string? SubjectNamespace { get; set; }

        /// <summary>
        /// Gets or sets the SubjectRelation.
        /// </summary>
        public string? SubjectRelation { get; set; }
    }
}
