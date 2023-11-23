// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;

namespace RebacExperiments.Server.Api.Models
{
    public class User : Entity
    {
        /// <summary>
        /// Gets or sets the FullName.
        /// </summary>
        public required string FullName { get; set; }

        /// <summary>
        /// Gets or sets the PreferredName.
        /// </summary>
        public required string PreferredName { get; set; }

        /// <summary>
        /// Gets or sets the IsPermittedToLogon.
        /// </summary>
        [MemberNotNullWhen(returnValue: true, member: nameof(LogonName))]
        [MemberNotNullWhen(returnValue: true, member: nameof(HashedPassword))]
        public bool IsPermittedToLogon { get; set; }

        /// <summary>
        /// Gets or sets the LogonName.
        /// </summary>
        public string? LogonName { get; set; }

        /// <summary>
        /// Gets or sets the HashedPassword.
        /// </summary>
        public string? HashedPassword { get; set; }
    }
}
