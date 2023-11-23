// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.EntityFrameworkCore;
using RebacExperiments.Server.Api.Infrastructure.Constants;
using RebacExperiments.Server.Api.Infrastructure.Database;
using RebacExperiments.Server.Api.Infrastructure.Exceptions;
using RebacExperiments.Server.Api.Infrastructure.Logging;
using RebacExperiments.Server.Api.Models;

namespace RebacExperiments.Server.Api.Services
{
    public class UserTaskService : IUserTaskService
    {
        private readonly ILogger<UserTaskService> _logger;

        public UserTaskService(ILogger<UserTaskService> logger)
        {
            _logger = logger;
        }

        public async Task<UserTask> CreateUserTaskAsync(ApplicationDbContext context, UserTask userTask, int currentUserId, CancellationToken cancellationToken)
        {
            _logger.TraceMethodEntry();

            using (var transaction = await context.Database.BeginTransactionAsync(cancellationToken))
            {
                // Make sure the Current User is the last editor:
                userTask.LastEditedBy = currentUserId;

                // Add the new Task, the HiLo Pattern automatically assigns a new Id using the HiLo Pattern
                await context.AddAsync(userTask, cancellationToken);

                // The User is Viewer and Owner of the Task
                await context.AddRelationshipAsync<UserTask, User>(userTask.Id, Relations.Viewer, currentUserId, null, currentUserId, cancellationToken);
                await context.AddRelationshipAsync<UserTask, User>(userTask.Id, Relations.Owner, currentUserId, null, currentUserId, cancellationToken);

                // We want the created task to be visible by all members of the organization the user is in
                var organizations = await context
                    .ListUserObjects<Organization>(currentUserId, Relations.Member)
                    .AsNoTracking()
                    .ToListAsync(cancellationToken);

                foreach (var organization in organizations)
                {
                    await context.AddRelationshipAsync<UserTask, Organization>(userTask.Id, Relations.Viewer, organization.Id, Relations.Member, currentUserId, cancellationToken);
                }

                await context.SaveChangesAsync(cancellationToken);

                await transaction.CommitAsync(cancellationToken);
            }

            return userTask;
        }

        public async Task<UserTask> GetUserTaskByIdAsync(ApplicationDbContext context, int userTaskId, int currentUserId, CancellationToken cancellationToken)
        {
            _logger.TraceMethodEntry();

            var userTask = await context.UserTasks
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == userTaskId, cancellationToken);

            if (userTask == null)
            {
                throw new EntityNotFoundException()
                {
                    EntityName = nameof(UserTask),
                    EntityId = userTaskId,
                };
            }

            bool isAuthorized = await context.CheckUserObject(currentUserId, userTask, Relations.Viewer, cancellationToken);

            if (!isAuthorized)
            {
                throw new EntityUnauthorizedAccessException()
                {
                    EntityName = nameof(UserTask),
                    EntityId = userTaskId,
                    UserId = currentUserId,
                };
            }

            return userTask;
        }

        public async Task<List<UserTask>> GetUserTasksAsync(ApplicationDbContext context, int currentUserId, CancellationToken cancellationToken)
        {
            _logger.TraceMethodEntry();

            var userTasks = context
                .ListUserObjects<UserTask>(currentUserId, new[] { Relations.Viewer, Relations.Owner })
                .ToListAsync(cancellationToken);

            return await userTasks;
        }

        public async Task<UserTask> UpdateUserTaskAsync(ApplicationDbContext context, UserTask userTask, int currentUserId, CancellationToken cancellationToken)
        {
            _logger.TraceMethodEntry();

            bool isAuthorized = await context.CheckUserObject(currentUserId, userTask, Relations.Owner, cancellationToken);

            if (!isAuthorized)
            {
                throw new EntityUnauthorizedAccessException()
                {
                    EntityName = nameof(UserTask),
                    EntityId = userTask.Id,
                    UserId = currentUserId,
                };
            }

            int rowsAffected = await context.UserTasks
                .Where(t => t.Id == userTask.Id && t.RowVersion == userTask.RowVersion)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(x => x.Title, userTask.Title)
                    .SetProperty(x => x.Description, userTask.Description)
                    .SetProperty(x => x.DueDateTime, userTask.DueDateTime)
                    .SetProperty(x => x.CompletedDateTime, userTask.CompletedDateTime)
                    .SetProperty(x => x.ReminderDateTime, userTask.ReminderDateTime)
                    .SetProperty(x => x.AssignedTo, userTask.AssignedTo)
                    .SetProperty(x => x.UserTaskPriority, userTask.UserTaskPriority)
                    .SetProperty(x => x.UserTaskStatus, userTask.UserTaskStatus)
                    .SetProperty(x => x.LastEditedBy, currentUserId), cancellationToken);

            if (rowsAffected == 0)
            {
                throw new EntityConcurrencyException()
                {
                    EntityName = nameof(UserTask),
                    EntityId = userTask.Id,
                };
            }

            return userTask;
        }

        public async Task DeleteUserTaskAsync(ApplicationDbContext context, int userTaskId, int currentUserId, CancellationToken cancellationToken)
        {
            _logger.TraceMethodEntry();

            using (var transaction = await context.Database.BeginTransactionAsync(cancellationToken))
            {
                var userTask = await context.UserTasks
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Id == userTaskId);

                if (userTask == null)
                {
                    throw new EntityNotFoundException()
                    {
                        EntityName = nameof(UserTask),
                        EntityId = userTaskId,
                    };
                }

                bool isAuthorized = await context.CheckUserObject<UserTask>(currentUserId, userTaskId, Relations.Owner, cancellationToken);

                if (!isAuthorized)
                {
                    throw new EntityUnauthorizedAccessException()
                    {
                        EntityName = nameof(UserTask),
                        EntityId = userTaskId,
                        UserId = currentUserId,
                    };
                }

                // Start by deleting all Relationships, where UserTask is the Object ...
                {
                    int numRowsDeleted = await context
                        .RelationTuples.Where(x => x.ObjectNamespace == nameof(UserTask) && x.ObjectKey == userTask.Id)
                        .ExecuteDeleteAsync(cancellationToken);

                    if (_logger.IsDebugEnabled())
                    {
                        _logger.LogDebug("'{NumRowsDeleted}' Relations deleted for Object UserTask (Id = {UserTaskId})", numRowsDeleted, userTaskId);
                    }
                }

                // ... then delete all Relationships, where UserTask is the Subject ...
                {
                    int numRowsDeleted = await context
                        .RelationTuples.Where(x => x.SubjectNamespace == nameof(UserTask) && x.SubjectKey == userTask.Id)
                        .ExecuteDeleteAsync(cancellationToken);

                    if (_logger.IsDebugEnabled())
                    {
                        _logger.LogDebug("'{NumRowsDeleted}' Relations deleted for Subject UserTask (Id = {UserTaskId})", numRowsDeleted, userTaskId);
                    }
                }

                // After removing all possible references, delete the UserTask itself
                int rowsAffected = await context.UserTasks
                    .Where(t => t.Id == userTask.Id)
                    .ExecuteDeleteAsync(cancellationToken);

                // No Idea if this could happen, because we are in a Transaction and there
                // is a row, which should be locked. So this shouldn't happen at all...
                if (rowsAffected == 0)
                {
                    throw new EntityConcurrencyException()
                    {
                        EntityName = nameof(UserTask),
                        EntityId = userTaskId,
                    };
                }

                await transaction.CommitAsync(cancellationToken);
            }
        }
    }
}