// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace AclExperiments.Database.Model
{
    public abstract class SqlEntity
    {
        /// <summary>
        /// Gets or sets the Id.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the user the task row version.
        /// </summary>
        public byte[]? RowVersion { get; set; }

        /// <summary>
        /// Gets or sets the user, that made the latest modifications.
        /// </summary>
        public int LastEditedBy { get; set; }

        /// <summary>
        /// Gets or sets the Start Date for the row.
        /// </summary>
        public DateTime? ValidFrom { get; set; }

        /// <summary>
        /// Gets or sets the End Date for the row.
        /// </summary>
        public DateTime? ValidTo { get; set; }
    }
}
