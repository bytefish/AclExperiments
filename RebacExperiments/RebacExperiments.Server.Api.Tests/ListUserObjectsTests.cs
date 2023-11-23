// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using RebacExperiments.Server.Api.Infrastructure.Constants;
using RebacExperiments.Server.Api.Infrastructure.Database;
using RebacExperiments.Server.Api.Models;
using System.Linq;
using System.Threading.Tasks;

namespace RebacExperiments.Server.Api.Tests
{
    public class ListUserObjectsTests : TransactionalTestBase
    {
        /// <summary>
        /// In this test we create a <see cref="User"/> (user) and a <see cref="UserTask"/> (task). The 'user' is member of 
        /// a <see cref="Team"/> (team). The 'user' is also a member of an <see cref="Organization"/> (oganization). Members 
        /// of the 'organization' are viewers of the 'task' and members of the 'team' are owners of the 'task'.
        /// 
        /// The Relationship-Table is given below.
        /// 
        /// ObjectKey           |  ObjectNamespace  |   ObjectRelation  |   SubjectKey          |   SubjectNamespace    |   SubjectRelation
        /// --------------------|-------------------|-------------------|-----------------------|-----------------------|-------------------
        /// :team.id:           |   Team            |       member      |   :user.id:           |       User            |   NULL
        /// :organization.id:   |   Organization    |       member      |   :user.id:           |       User            |   NULL
        /// :task.id:           |   UserTask        |       viewer      |   :organization.id:   |       Organization    |   member
        /// :task.id:           |   UserTask        |       owner       |   :team.id:           |       Team            |   member
        /// </summary>
        [Test]
        public async Task ListUserObjects_OneUserTaskAssignedThroughOrganizationAndTeam()
        {
            // Arrange
            var user = new User
            {
                FullName = "Test-User",
                PreferredName = "Test-User",
                IsPermittedToLogon = false,
                LastEditedBy = 1,
                LogonName = "test-user@test-user.localhost"
            };

            await _applicationDbContext.AddAsync(user);
            await _applicationDbContext.SaveChangesAsync();

            var organization = new Organization
            {
                Name = "Test-Organization",
                Description = "Organization for Unit Test",
                LastEditedBy = user.Id
            };

            await _applicationDbContext.AddAsync(organization);
            await _applicationDbContext.SaveChangesAsync();

            var team = new Team
            {
                Name = "Test-Team",
                Description = "Team for Unit Test",
                LastEditedBy = user.Id
            };

            await _applicationDbContext.AddAsync(team);
            await _applicationDbContext.SaveChangesAsync();

            var task = new UserTask
            {
                Title = "Test-Task",
                Description = "My Test-Task",
                LastEditedBy = user.Id,
                UserTaskPriority = UserTaskPriorityEnum.High,
                UserTaskStatus = UserTaskStatusEnum.InProgress
            };

            await _applicationDbContext.AddAsync(task);
            await _applicationDbContext.SaveChangesAsync();

            await _applicationDbContext.AddRelationshipAsync(team, Relations.Member, user, null, user.Id);
            await _applicationDbContext.AddRelationshipAsync(organization, Relations.Member, user, null, user.Id);
            await _applicationDbContext.AddRelationshipAsync(task, Relations.Viewer, organization, Relations.Member, user.Id);
            await _applicationDbContext.AddRelationshipAsync(task, Relations.Owner, team, Relations.Member, user.Id);
            await _applicationDbContext.SaveChangesAsync();

            // Act
            var userTasks_Owner = _applicationDbContext
                .ListUserObjects<UserTask>(user.Id, Relations.Owner)
                .AsNoTracking()
                .ToList();

            var userTasks_Viewer = _applicationDbContext
                .ListUserObjects<UserTask>(user.Id, Relations.Viewer)
                .AsNoTracking()
                .ToList();

            var team_Member = _applicationDbContext
                .ListUserObjects<Team>(user.Id, Relations.Member)
                .AsNoTracking()
                .ToList();

            var organization_Member = _applicationDbContext
                .ListUserObjects<Organization>(user.Id, Relations.Member)
                .AsNoTracking()
                .ToList();

            // Assert
            Assert.AreEqual(1, userTasks_Owner.Count);
            Assert.AreEqual(task.Id, userTasks_Owner[0].Id);

            Assert.AreEqual(1, userTasks_Viewer.Count);
            Assert.AreEqual(task.Id, userTasks_Viewer[0].Id);

            Assert.AreEqual(1, team_Member.Count);
            Assert.AreEqual(team.Id, team_Member[0].Id);

            Assert.AreEqual(1, organization_Member.Count);
            Assert.AreEqual(organization.Id, organization_Member[0].Id);
        }

        /// <summary>
        /// In this test we create a <see cref="User"/> (user) and assign two <see cref="UserTask"/> (tas1, task2). The 'user' 
        /// is 'viewer' for 'task1' and an 'owner' for 'task2'.
        /// 
        /// The Relationship-Table is given below.
        /// 
        /// ObjectKey           |  ObjectNamespace  |   ObjectRelation  |   SubjectKey          |   SubjectNamespace    |   SubjectRelation
        /// --------------------|-------------------|-------------------|-----------------------|-----------------------|-------------------
        /// :task1.id:          |   UserTask        |       viewer      |   :user.id:           |       User            |   NULL
        /// :task2.id:          |   UserTask        |       owner       |   :user.id:           |       User            |   NULL
        /// </summary>
        [Test]
        public async Task ListUserObjects_TwoUserTasksAssignedToUser()
        {
            // Arrange
            var user = new User
            {
                FullName = "Test-User",
                PreferredName = "Test-User",
                IsPermittedToLogon = false,
                LastEditedBy = 1,
                LogonName = "test-user@test-user.localhost"
            };

            await _applicationDbContext.AddAsync(user);
            await _applicationDbContext.SaveChangesAsync();

            var task1 = new UserTask
            {
                Title = "Task 1",
                Description = "Task 1",
                LastEditedBy = user.Id,
                UserTaskPriority = UserTaskPriorityEnum.High,
                UserTaskStatus = UserTaskStatusEnum.InProgress
            };
            
            var task2 = new UserTask
            {
                Title = "Task2",
                Description = "Task2",
                LastEditedBy = user.Id,
                UserTaskPriority = UserTaskPriorityEnum.High,
                UserTaskStatus = UserTaskStatusEnum.InProgress
            };

            await _applicationDbContext.AddRangeAsync(new[] { task1, task2 });
            await _applicationDbContext.SaveChangesAsync();

            await _applicationDbContext.AddRelationshipAsync(task1, Relations.Viewer, user, null, user.Id);
            await _applicationDbContext.AddRelationshipAsync(task2, Relations.Owner, user, null, user.Id);
            await _applicationDbContext.SaveChangesAsync();

            // Act
            var userTasks = _applicationDbContext
                .ListUserObjects<UserTask>(user.Id, new[] { Relations.Viewer, Relations.Owner })
                .AsNoTracking()
                .ToList();

            // Assert
            Assert.AreEqual(2, userTasks.Count);
            Assert.IsTrue(userTasks.Any(x => x.Id == task1.Id));
            Assert.IsTrue(userTasks.Any(x => x.Id == task2.Id));
        }
    }
}