// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace AclExperiments.Database.Model
{
    /// <summary>
    /// A Namespace Configuration in the Google Zanzibar language format.
    /// </summary>
    public class SqlAuthorizationModel : SqlEntity
    {
        /// <summary>
        /// Gets or sets a unique identifier for the model.
        /// </summary>
        public required string ModelKey { get; set; }

        /// <summary>
        /// Gets or sets the Name.
        /// </summary>
        public required string Name { get; set; }

        /// <summary>
        /// Gets or sets the Description.
        /// </summary>
        public required string Description { get; set; }

        /// <summary>
        /// Gets or sets the Content.
        /// </summary>
        public required string Content { get; set; }
    }
}
