// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace AclExperiments.Database.Model
{
    /// <summary>
    /// A Namespace Configuration in the Google Zanzibar language format.
    /// </summary>
    public class SqlNamespaceConfiguration : SqlEntity
    {
        /// <summary>
        /// Gets or sets the Name.
        /// </summary>
        public required string Name { get; set; }

        /// <summary>
        /// Gets or sets the Version.
        /// </summary>
        public required int Version { get; set; }

        /// <summary>
        /// Gets or sets the Content.
        /// </summary>
        public required string Content { get; set; }
    }
}
