// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.Mvc;
using RebacExperiments.Server.Api.Infrastructure.Resources;
using System.ComponentModel.DataAnnotations;

namespace RebacExperiments.Server.Api.Dto
{
    [ModelMetadataType(typeof(CredentialsDtoMetadata))]
    public class CredentialsDto
    {
        public required string Username { get; set; }

        public required string Password { get; set; }

        public bool RememberMe { get; set; }
    }

    public class CredentialsDtoMetadata
    {
        [Required(ErrorMessageResourceName = nameof(ErrorMessages.Validation_Required), ErrorMessageResourceType = typeof(ErrorMessages))]
        [StringLength(255, ErrorMessageResourceName = nameof(ErrorMessages.Validation_StringLength), ErrorMessageResourceType = typeof(ErrorMessages))]
        public required string Username { get; set; }

        [Required(ErrorMessageResourceName = nameof(ErrorMessages.Validation_Required), ErrorMessageResourceType = typeof(ErrorMessages))]
        [StringLength(255, ErrorMessageResourceName = nameof(ErrorMessages.Validation_StringLength), ErrorMessageResourceType = typeof(ErrorMessages))]
        public required string Password { get; set; }
    }
}
