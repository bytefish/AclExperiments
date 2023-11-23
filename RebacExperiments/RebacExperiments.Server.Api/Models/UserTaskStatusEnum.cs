// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace RebacExperiments.Server.Api.Models
{
    /// <summary>
    /// An Enumeration of Status for Tasks.
    /// </summary>
    public enum UserTaskStatusEnum
    {
        /// <summary>
        /// None.
        /// </summary>
        None = 0,

        /// <summary>
        /// Not Started.
        /// </summary>
        NotStarted = 1,

        /// <summary>
        /// In Progress.
        /// </summary>
        InProgress = 2,

        /// <summary>
        /// Completed.
        /// </summary>
        Completed = 3,

        /// <summary>
        /// Waiting For Others.
        /// </summary>
        WaitingForOthers = 3,

        /// <summary>
        /// Deferred.
        /// </summary>
        Deferred = 4
    }
}
