// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using AclExperiments.Tests.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace RebacExperiments.Server.Api.Tests
{
    public class AclServiceTests : IntegrationTestBase
    {


        ///// <summary>
        ///// In this test we create a <see cref="SqlUser"/> (user) and a <see cref="UserTask"/> (task). The 'user' is member of 
        ///// a <see cref="Team"/> (team). The 'user' is also a member of an <see cref="Organization"/> (oganization). Members 
        ///// of the 'organization' are viewers of the 'task' and members of the 'team' are owners of the 'task'.
        ///// 
        ///// The Relationship-Table is given below.
        ///// 
        ///// ObjectKey           |  ObjectNamespace  |   ObjectRelation  |   SubjectKey          |   SubjectNamespace    |   SubjectRelation
        ///// --------------------|-------------------|-------------------|-----------------------|-----------------------|-------------------
        ///// :team.id:           |   Team            |       member      |   :user.id:           |       User            |   NULL
        ///// :organization.id:   |   Organization    |       member      |   :user.id:           |       User            |   NULL
        ///// :task.id:           |   UserTask        |       viewer      |   :organization.id:   |       Organization    |   member
        ///// :task.id:           |   UserTask        |       owner       |   :team.id:           |       Team            |   member
        ///// </summary>
        //[Test]
        //public async Task CheckUserObject_OneUserTaskAssignedThroughOrganizationAndTeam()
        //{
        //    // Arrange
        //    var user = new SqlUser
        //    {
        //        FullName = "Test-User",
        //        PreferredName = "Test-User",
        //        IsPermittedToLogon = false,
        //        LastEditedBy = 1,
        //        LogonName = "test-user@test-user.localhost"
        //    };

        //    await _applicationDbContext.AddAsync(user);
        //    await _applicationDbContext.SaveChangesAsync();

        //    var organization = new Organization
        //    {
        //        Name = "Test-Organization",
        //        Description = "Organization for Unit Test",
        //        LastEditedBy = user.Id
        //    };

        //    await _applicationDbContext.AddAsync(organization);
        //    await _applicationDbContext.SaveChangesAsync();

        //    var team = new Team
        //    {
        //        Name = "Test-Team",
        //        Description = "Team for Unit Test",
        //        LastEditedBy = user.Id
        //    };

        //    await _applicationDbContext.AddAsync(team);
        //    await _applicationDbContext.SaveChangesAsync();

        //    var task = new UserTask
        //    {
        //        Title = "Test-Task",
        //        Description = "My Test-Task",
        //        LastEditedBy = user.Id,
        //        UserTaskPriority = UserTaskPriorityEnum.High,
        //        UserTaskStatus = UserTaskStatusEnum.InProgress
        //    };

        //    await _applicationDbContext.AddAsync(task);
        //    await _applicationDbContext.SaveChangesAsync();

        //    await _applicationDbContext.AddRelationshipAsync(team, Relations.Member, user, null, user.Id);
        //    await _applicationDbContext.AddRelationshipAsync(organization, Relations.Member, user, null, user.Id);
        //    await _applicationDbContext.AddRelationshipAsync(task, Relations.Viewer, organization, Relations.Member, user.Id);
        //    await _applicationDbContext.AddRelationshipAsync(task, Relations.Owner, team, Relations.Member, user.Id);
        //    await _applicationDbContext.SaveChangesAsync();

        //    // Act
        //    var isOwnerOfTask = await _applicationDbContext.CheckUserObject(user.Id, task, Relations.Owner, default);
        //    var isViewerOfTask = await _applicationDbContext.CheckUserObject(user.Id, task, Relations.Viewer, default);

        //    // Assert
        //    Assert.AreEqual(true, isOwnerOfTask);
        //    Assert.AreEqual(true, isViewerOfTask);
        //}

        ///// <summary>
        ///// In this test we create a <see cref="SqlUser"/> (user) and assign two <see cref="UserTask"/> (tas1, task2). The 'user' 
        ///// is 'viewer' for 'task1' and an 'owner' for 'task2'.
        ///// 
        ///// The Relationship-Table is given below.
        ///// 
        ///// ObjectKey           |  ObjectNamespace  |   ObjectRelation  |   SubjectKey          |   SubjectNamespace    |   SubjectRelation
        ///// --------------------|-------------------|-------------------|-----------------------|-----------------------|-------------------
        ///// :task1.id:          |   UserTask        |       viewer      |   :user.id:           |       User            |   NULL
        ///// :task2.id:          |   UserTask        |       owner       |   :user.id:           |       User            |   NULL
        ///// </summary>
        //[Test]
        //public async Task CheckUserObject_TwoUserTasksAssignedToUser()
        //{
        //    // Arrange
        //    var user = new SqlUser
        //    {
        //        FullName = "Test-User",
        //        PreferredName = "Test-User",
        //        IsPermittedToLogon = false,
        //        LastEditedBy = 1,
        //        LogonName = "test-user@test-user.localhost"
        //    };

        //    await _applicationDbContext.AddAsync(user);
        //    await _applicationDbContext.SaveChangesAsync();

        //    var task1 = new UserTask
        //    {
        //        Title = "Task 1",
        //        Description = "Task 1",
        //        LastEditedBy = user.Id,
        //        UserTaskPriority = UserTaskPriorityEnum.High,
        //        UserTaskStatus = UserTaskStatusEnum.InProgress
        //    };

        //    var task2 = new UserTask
        //    {
        //        Title = "Task2",
        //        Description = "Task2",
        //        LastEditedBy = user.Id,
        //        UserTaskPriority = UserTaskPriorityEnum.High,
        //        UserTaskStatus = UserTaskStatusEnum.InProgress
        //    };

        //    await _applicationDbContext.AddRangeAsync(new[] { task1, task2 });
        //    await _applicationDbContext.SaveChangesAsync();

        //    await _applicationDbContext.AddRelationshipAsync(task1, Relations.Viewer, user, null, user.Id);
        //    await _applicationDbContext.AddRelationshipAsync(task2, Relations.Owner, user, null, user.Id);
        //    await _applicationDbContext.SaveChangesAsync();

        //    // Act
        //    var isOwnerOfTask1 = await _applicationDbContext.CheckUserObject(user.Id, task1, Relations.Owner, default);
        //    var isViewerOfTask1 = await _applicationDbContext.CheckUserObject(user.Id, task1, Relations.Viewer, default);

        //    var isOwnerOfTask2 = await _applicationDbContext.CheckUserObject(user.Id, task2, Relations.Owner, default);
        //    var isViewerOfTask2 = await _applicationDbContext.CheckUserObject(user.Id, task2, Relations.Viewer, default);

        //    // Assert
        //    Assert.AreEqual(false, isOwnerOfTask1);
        //    Assert.AreEqual(true, isViewerOfTask1);  

        //    Assert.AreEqual(true, isOwnerOfTask2);
        //    Assert.AreEqual(false, isViewerOfTask2);           
        //}
        public override void RegisterServices(IServiceCollection services)
        {
            throw new NotImplementedException();
        }
    }
}