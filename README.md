# Experimenting with Relationship-based Access Control #

[written an article about the Google Zanzibar Data Model]: https://www.bytefish.de/blog/relationship_based_acl_with_google_zanzibar.html

The Google Drive app starts and a moment later *your files* appear. It's magic. But have you 
ever wondered what's *your files* actually? How do these services actually know, which files 
*you are allowed* to see?

Are you part of an *Organization* and you are allowed to *view* all their files? Have you been 
assigned to a *Team*, that's allowed to *view* or *edit* files? Has someone shared *their files* 
with *you* as a *User*?

So in 2019 Google has lifted the curtain and has published a paper on *Google Zanzibar*, which 
is Google's central solution for providing authorization among its many services:

* [https://research.google/pubs/pub48190/](https://research.google/pubs/pub48190/)

The keyword here is *Relationship-based Access Control*, which is ...

> [...] an authorization paradigm where a subject's permission to access a resource is defined by the 
> presence of relationships between those subjects and resources.

I have previously [written an article about the Google Zanzibar Data Model], and also wrote some 
pretty nice SQL statements to make sense of the it. This repository implements Relationship-based 
Access Control using ASP.NET Core, EntityFramework Core and Microsoft SQL Server.

The blog article for this repository can be found at:

* [https://www.bytefish.de/blog/aspnetcore_rebac.html](https://www.bytefish.de/blog/aspnetcore_rebac.html)

## Running the Example ##

We got everything in place. We can now start the application and use Swagger to query it. But Visual Studio 2022 
now comes with the "Endpoints Explorer" to execute HTTP Requests against HTTP endpoints. Though it's not fully-fledged 
yet, I think it'll improve with time and it already covers a lot of use cases.

You can find the Endpoints Explorer at:

* `View -> Other Windows -> Endpoints Explorer`

By clicking on `RebacExperiments.Server.Api.http` the HTTP script with the sample requests comes up.

### The Example Setup ###

We have got 2 Tasks:

* `task_152`: "Sign Document"
* `task 323`: "Call Back Philipp Wagner"

And we have got two users: 

* `user_philipp`: "Philipp Wagner"
* `user_max`: "Max Mustermann"

Both users are permitted to login, so they are allowed to query for data, given a permitted role and permissions.

There are two Organizations:

* Organization 1: "Organization #1"
* Organization 2: "Organization #2"

And 2 Roles:

* `role_user`: "User" (Allowed to Query for UserTasks)
* `role_admin`: "Administrator" (Allowed to Delete a UserTask)

The Relationships between the entities are the following:

```
The Relationship-Table is given below.

ObjectKey           |  ObjectNamespace  |   ObjectRelation  |   SubjectKey          |   SubjectNamespace    |   SubjectRelation
--------------------|-------------------|-------------------|-----------------------|-----------------------|-------------------
:task_323  :        |   UserTask        |       viewer      |   :organization_1:    |       Organization    |   member
:task_152  :        |   UserTask        |       viewer      |   :organization_1:    |       Organization    |   member
:task_152  :        |   UserTask        |       viewer      |   :organization_2:    |       Organization    |   member
:organization_1:    |   Organization    |       member      |   :user_philipp:      |       User            |   NULL
:organization_2:    |   Organization    |       member      |   :user_max:          |       User            |   NULL
:role_user:         |   Role            |       member      |   :user_philipp:      |       User            |   NULL
:role_admin:        |   Role            |       member      |   :user_philipp:      |       User            |   NULL
:role_user:         |   Role            |       member      |   :user_max:          |       User            |   NULL
:task_323:          |   UserTask        |       owner       |   :user_2:            |       User            |   member
```

We can draw the following conclusions here: A `member` of `organization_1` is `viewer` of `task_152` and `task_323`. A `member` 
of `organization_2` is a `viewer` of `task_152` only. `user_philipp` is member of `organization_1`, so the user is able to see 
both tasks as `viewer`. `user_max` is member of `organization_2`, so he is a `viewer` of `task_152` only. `user_philipp` has the 
`User` and `Administrator` roles assigned, so he can create, query and delete a `UserTask`. `user_max` only has the `User` role 
assigned, so he is not authorized to delete a `UserTask`. Finally `user_philipp` is also the `owner` of `task_323` so he is 
permitted to update the data of the `UserTask`.

### HTTP Endpoints Explorer Script ###

We start by signing in `philipp@bytefish.de`:

```
### Sign In "philipp@bytefish.de"

POST {{RebacExperiments.Server.Api_HostAddress}}/Authentication/sign-in
Content-Type: application/json

{
  "username": "philipp@bytefish.de",
  "password": "5!F25GbKwU3P",
  "rememberMe": true
}
```

Then we get all Tasks by querying `/UserTasks` endpoint:

```
### Get all UserTasks

GET {{RebacExperiments.Server.Api_HostAddress}}/UserTasks
```

As expected by the example setup, the result has the `UserTask` with ID `152` and ID `323` as body:

```
[
  {
    "title": "Call Back",
    "description": "Call Back Philipp Wagner",
    "dueDateTime": null,
    "reminderDateTime": null,
    "completedDateTime": null,
    "assignedTo": null,
    "userTaskPriority": 1,
    "userTaskStatus": 1,
    "id": 152,
    "rowVersion": "AAAAAAAAB\u002Bw=",
    "lastEditedBy": 1,
    "validFrom": "2013-01-01T00:00:00",
    "validTo": "9999-12-31T23:59:59.9999999"
  },
  {
    "title": "Sign Document",
    "description": "You need to Sign a Document",
    "dueDateTime": null,
    "reminderDateTime": null,
    "completedDateTime": null,
    "assignedTo": null,
    "userTaskPriority": 2,
    "userTaskStatus": 2,
    "id": 323,
    "rowVersion": "AAAAAAAAB\u002B0=",
    "lastEditedBy": 1,
    "validFrom": "2013-01-01T00:00:00",
    "validTo": "9999-12-31T23:59:59.9999999"
  }
]
```

We sign out the user `philipp@bytefish.de`:

```
### Sign Out "philipp@bytefish.de"

POST {{RebacExperiments.Server.Api_HostAddress}}/Authentication/sign-out
```

And we query the `/UserTasks` endpoint to get the list of `UserTask` entities: 

```
### Check for 401 Unauthorized when not Authenticated

GET {{RebacExperiments.Server.Api_HostAddress}}/UserTasks
```

The Backend correctly returns a `401` Status Code, because the user isn't authenticated:

```
Status: 401 UnauthorizedTime: 7,48 msSize: 0 bytes
```

Then we sign in the user `max@mustermann.local`:

```
### Sign In as "max@mustermann.local"

POST {{RebacExperiments.Server.Api_HostAddress}}/Authentication/sign-in
Content-Type: application/json

{
  "username": "max@mustermann.local",
  "password": "5!F25GbKwU3P",
  "rememberMe": true
}
```

And querying the `/UserTasks` endpoint:

```
### Get all UserTasks for "max@mustermann.local"

GET {{RebacExperiments.Server.Api_HostAddress}}/UserTasks
```

Returns only `UserTask` with ID `152`, as expected:

```
[
  {
    "title": "Call Back",
    "description": "Call Back Philipp Wagner",
    "dueDateTime": null,
    "reminderDateTime": null,
    "completedDateTime": null,
    "assignedTo": null,
    "userTaskPriority": 1,
    "userTaskStatus": 1,
    "id": 152,
    "rowVersion": "AAAAAAAAB\u002Bw=",
    "lastEditedBy": 1,
    "validFrom": "2013-01-01T00:00:00",
    "validTo": "9999-12-31T23:59:59.9999999"
  }
]
```

Since we are not the `owner` of the Task `152`, we shouldn't be able to delete it:

```
### Delete UserTask 152 as "max@mustermann.local" (he is not the owner)
DELETE {{RebacExperiments.Server.Api_HostAddress}}/UserTasks/152
```

And as expected, we are not permitted to delete the task:

```
Status: 403 ForbiddenTime: 190,91 msSize: 1154 bytes

application/problem+json; charset=utf-8, 1154 bytes

{
  "type": "EntityUnauthorizedAccessException",
  "title": "EntityUnauthorizedAccess (User = 7, Entity = UserTask, EntityID = 152)",
  "status": 403,
  "instance": "/UserTasks/152",
  "error-code": "Entity:000002",
  "trace-id": "0HMUJJ9QPSEE0:00000001",
  "exception": "RebacExperiments.Server.Api.Infrastructure.Exceptions.EntityUnauthorizedAccessException: Exception of type \u0027RebacExperiments.Server.Api.Infrastructure.Exceptions.EntityUnauthorizedAccessException\u0027 was thrown.\r\n   at RebacExperiments.Server.Api.Services.UserTaskService.DeleteUserTaskAsync(ApplicationDbContext context, Int32 userTaskId, Int32 currentUserId, CancellationToken cancellationToken) in C:\\Users\\philipp\\source\\repos\\bytefish\\RebacExperiments\\RebacExperiments\\RebacExperiments.Server.Api\\Services\\UserTaskService.cs:line 148\r\n   at RebacExperiments.Server.Api.Controllers.UserTasksController.DeleteUserTask(ApplicationDbContext context, IUserTaskService userTaskService, Int32 userTaskId, CancellationToken cancellationToken) in C:\\Users\\philipp\\source\\repos\\bytefish\\RebacExperiments\\RebacExperiments\\RebacExperiments.Server.Api\\Controllers\\UserTasksController.cs:line 141"
}
```

The user `max@mustermann.local` is allowed to create a `UserTask` though. We have seen, that the person 
creating a `UserTask` is automatically the `owner` of the task and the entire organization of the user 
can view it.

```
### Create a new UserTask "API HTTP File Example" as "max@mustermann.local"

POST {{RebacExperiments.Server.Api_HostAddress}}/UserTasks
Content-Type: application/json

{
    "title": "API HTTP File Example",
    "description": "API HTTP File Example",
    "dueDateTime": null,
    "reminderDateTime": null,
    "completedDateTime": null,
    "assignedTo": null,
    "userTaskPriority": 2,
    "userTaskStatus": 2
}
```

We get a successful response with the created task as the response payload:

```
Status: 200 OKTime: 264,41 msSize: 335 bytes

{
  "title": "API HTTP File Example",
  "description": "API HTTP File Example",
  "dueDateTime": null,
  "reminderDateTime": null,
  "completedDateTime": null,
  "assignedTo": null,
  "userTaskPriority": 2,
  "userTaskStatus": 2,
  "id": 38188,
  "rowVersion": "AAAAAAAAB/k=",
  "lastEditedBy": 7,
  "validFrom": "2023-10-23T08:02:41.8051703",
  "validTo": "9999-12-31T23:59:59.9999999"
}
```

If we now sign-in "philipp@bytefish.de":

```
### Sign In "philipp@bytefish.de"

POST {{RebacExperiments.Server.Api_HostAddress}}/Authentication/sign-in
Content-Type: application/json

{
  "username": "philipp@bytefish.de",
  "password": "5!F25GbKwU3P",
  "rememberMe": true
}
```

And query for the `UserTasks`:

```
### Get all UserTasks for "philipp@bytefish.de"

GET {{RebacExperiments.Server.Api_HostAddress}}/UserTasks
```

We cannot see the "API HTTP File Example" Task created by the other user, because 
`philipp@bytefish.de` isn't part of the Organization and has no relationship to the 
task.

And this is where our example ends. 

Feel free to play around with the example as much as you like!



## Further Reading ##

* [Exploring Relationship-based Access Control (ReBAC) with Google Zanzibar](https://www.bytefish.de/blog/relationship_based_acl_with_google_zanzibar.html)