// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using RebacExperiments.Server.Api.Infrastructure.Database;
using RebacExperiments.Server.Api.Models;

namespace RebacExperiments.Server.Api.Services
{
    /// <summary>
    /// An <see cref="IUserTaskService"/> is responsible for authorized access to a <see cref="UserTask"/>.
    /// </summary>
    public interface IUserTaskService
    {
        /// <summary>
        /// Creates a new <see cref="UserTask"/> and assigns default relationships.
        /// </summary>
        /// <param name="context"><see cref="ApplicationDbContext"/> to use</param>
        /// <param name="userTask"><see cref="UserTask"/> with values</param>
        /// <param name="currentUserId"><see cref="User"/> ID</param>
        /// <param name="cancellationToken">CancellationToken to cancel asynchronous processing</param>
        /// <returns>The created <see cref="UserTask"/></returns>
        Task<UserTask> CreateUserTaskAsync(ApplicationDbContext context, UserTask userTask, int currentUserId, CancellationToken cancellationToken);

        /// <summary>
        /// Gets a <see cref="UserTask"/> by id for the current user.
        /// </summary>
        /// <param name="context"><see cref="ApplicationDbContext"/> to use</param>
        /// <param name="userTaskId"><see cref="UserTask"/> ID</param>
        /// <param name="currentUserId"><see cref="User"/> ID</param>
        /// <param name="cancellationToken">CancellationToken to cancel asynchronous processing</param>
        /// <returns>The <see cref="UserTask"/> for the given ID</returns>
        Task<UserTask> GetUserTaskByIdAsync(ApplicationDbContext context, int userTaskId, int currentUserId, CancellationToken cancellationToken);

        /// <summary>
        /// Gets all <see cref="UserTask"/> the given User has Viewer or Owner access to.
        /// </summary>
        /// <param name="context"><see cref="ApplicationDbContext"/> to use</param>
        /// <param name="currentUserId"><see cref="User"/> ID</param>
        /// <param name="cancellationToken">CancellationToken to cancel asynchronous processing</param>
        /// <returns>The <see cref="UserTask"/> for the given ID</returns>
        Task<List<UserTask>> GetUserTasksAsync(ApplicationDbContext context, int currentUserId, CancellationToken cancellationToken);

        /// <summary>
        /// Updates a <see cref="UserTask"/> for the current user.
        /// </summary>
        /// <param name="context"><see cref="ApplicationDbContext"/> to use</param>
        /// <param name="userTask"><see cref="UserTask"/> with values</param>
        /// <param name="currentUserId"><see cref="User"/> ID</param>
        /// <param name="cancellationToken">CancellationToken to cancel asynchronous processing</param>
        /// <returns>The Updated <see cref="UserTask"/></returns>
        Task<UserTask> UpdateUserTaskAsync(ApplicationDbContext context, UserTask userTask, int currentUserId, CancellationToken cancellationToken);

        /// <summary>
        /// Deletes a <see cref="UserTask"/> and all of its relationships.
        /// </summary>
        /// <param name="context"><see cref="ApplicationDbContext"/> to use</param>
        /// <param name="userTaskId"><see cref="UserTask"/> to delete</param>
        /// <param name="currentUserId"><see cref="User"/> ID</param>
        /// <param name="cancellationToken">CancellationToken to cancel asynchronous processing</param>
        /// <returns>The Updated <see cref="UserTask"/></returns>
        Task DeleteUserTaskAsync(ApplicationDbContext context, int userTaskId, int currentUserId, CancellationToken cancellationToken);
    }
}
