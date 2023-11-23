// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.EntityFrameworkCore;
using RebacExperiments.Server.Api.Models;

namespace RebacExperiments.Server.Api.Infrastructure.Database
{
    /// <summary>
    /// A <see cref="DbContext"/> to query the database.
    /// </summary>
    public class ApplicationDbContext : DbContext
    {
        /// <summary>
        /// Logger.
        /// </summary>
        internal ILogger<ApplicationDbContext> Logger { get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="options">Options to configure the base <see cref="DbContext"/></param>
        public ApplicationDbContext(ILogger<ApplicationDbContext> logger, DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
            Logger = logger;
        }

        /// <summary>
        /// Gets or sets the Users.
        /// </summary>
        public DbSet<User> Users { get; set; } = null!;

        /// <summary>
        /// Gets or sets the Roles.
        /// </summary>
        public DbSet<Role> Roles { get; set; } = null!;

        /// <summary>
        /// Gets or sets the UserTasks.
        /// </summary>
        public DbSet<UserTask> UserTasks { get; set; } = null!;

        /// <summary>
        /// Gets or sets the UserTasks.
        /// </summary>
        public DbSet<Team> Teams { get; set; } = null!;

        /// <summary>
        /// Gets or sets the UserTasks.
        /// </summary>
        public DbSet<Organization> Organizations { get; set; } = null!;

        /// <summary>
        /// Gets or sets the UserTasks.
        /// </summary>
        public DbSet<RelationTuple> RelationTuples { get; set; } = null!;

        /// <summary>
        /// List Objects.
        /// </summary>
        /// <param name="objectNamespace">Object Namespace</param>
        /// <param name="objectRelation">Object Relation</param>
        /// <param name="subjectNamespace">Subject Namespace</param>
        /// <param name="subjectKey">Subject Key</param>
        /// <returns></returns>
        public IQueryable<RelationTuple> ListObjects(string objectNamespace, string objectRelation, string subjectNamespace, int subjectKey)
            => FromExpression(() => ListObjects(objectNamespace, objectRelation, subjectNamespace, subjectKey));

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Add ListObjects Function, so we can use it in LINQ:
            modelBuilder
                .HasDbFunction(
                    methodInfo: typeof(ApplicationDbContext).GetMethod(nameof(ListObjects), new[] { typeof(string), typeof(string), typeof(string), typeof(int) })!,
                    builderAction: builder => builder
                        .HasSchema("Identity")
                        .HasName("tvf_RelationTuples_ListObjects"));

            // Now create the Tables
            modelBuilder.HasSequence<int>("sq_UserTask", schema: "Application")
                .StartsAt(38187)
                .IncrementsBy(1);

            modelBuilder.Entity<UserTask>(entity =>
            {
                entity.ToTable("UserTask", "Application");

                entity.HasKey(e => e.Id);

                entity.Property(x => x.Id)
                    .HasColumnType("INT")
                    .HasColumnName("UserTaskID")
                    .UseHiLo("sq_UserTask", "Application")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Title)
                    .HasColumnType("NVARCHAR(50)")
                    .HasColumnName("Title")
                    .IsRequired(true)
                    .HasMaxLength(50);

                entity.Property(e => e.Description)
                    .HasColumnType("NVARCHAR(2000)")
                    .HasColumnName("Description")
                    .IsRequired(true)
                    .HasMaxLength(2000);

                entity.Property(e => e.DueDateTime)
                    .HasColumnType("DATETIME2(7)")
                    .HasColumnName("DueDateTime")
                    .IsRequired(false);

                entity.Property(e => e.ReminderDateTime)
                    .HasColumnType("DATETIME2(7)")
                    .HasColumnName("ReminderDateTime")
                    .IsRequired(false);

                entity.Property(e => e.CompletedDateTime)
                    .HasColumnType("DATETIME2(7)")
                    .HasColumnName("CompletedDateTime")
                    .IsRequired(false);

                entity.Property(e => e.AssignedTo)
                    .HasColumnType("INT")
                    .HasColumnName("AssignedTo")
                    .IsRequired(false);

                entity.Property(e => e.UserTaskStatus)
                    .HasColumnType("INT")
                    .HasColumnName("UserTaskStatusID")
                    .HasConversion(v => (int)v, v => (UserTaskStatusEnum)v)
                    .IsRequired(true);

                entity.Property(e => e.UserTaskPriority)
                    .HasColumnType("INT")
                    .HasColumnName("UserTaskPriorityID")
                    .HasConversion(v => (int)v, v => (UserTaskPriorityEnum)v)
                    .IsRequired(true);

                entity.Property(e => e.RowVersion)
                    .HasColumnType("ROWVERSION")
                    .HasColumnName("RowVersion")
                    .IsConcurrencyToken()
                    .IsRequired(false)
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.ValidFrom)
                    .HasColumnType("DATETIME2(7)")
                    .HasColumnName("ValidFrom")
                    .IsRequired(false)
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.ValidTo)
                    .HasColumnType("DATETIME2(7)")
                    .HasColumnName("ValidTo")
                    .IsRequired(false)
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.LastEditedBy)
                    .HasColumnType("INT")
                    .HasColumnName("LastEditedBy")
                    .IsRequired(true);
            });

            modelBuilder.HasSequence<int>("sq_Organization", schema: "Application")
                .StartsAt(38187)
                .IncrementsBy(1);

            modelBuilder.Entity<Organization>(entity =>
            {
                entity.ToTable("Organization", "Application");

                entity.HasKey(e => e.Id);

                entity.Property(x => x.Id)
                    .HasColumnType("INT")
                    .HasColumnName("OrganizationID")
                    .UseHiLo("sq_Organization", "Application")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Name)
                    .HasColumnType("NVARCHAR(255)")
                    .HasColumnName("Name")
                    .IsRequired(true)
                    .HasMaxLength(255);

                entity.Property(e => e.Description)
                    .HasColumnType("NVARCHAR(2000)")
                    .HasColumnName("Description")
                    .IsRequired(true)
                    .HasMaxLength(2000);

                entity.Property(e => e.RowVersion)
                    .HasColumnType("ROWVERSION")
                    .HasColumnName("RowVersion")
                    .IsConcurrencyToken()
                    .IsRequired(false)
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.ValidFrom)
                    .HasColumnType("DATETIME2(7)")
                    .HasColumnName("ValidFrom")
                    .IsRequired(false)
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.ValidTo)
                    .HasColumnType("DATETIME2(7)")
                    .HasColumnName("ValidTo")
                    .IsRequired(false)
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.LastEditedBy)
                    .HasColumnType("INT")
                    .HasColumnName("LastEditedBy")
                    .IsRequired(true);
            });

            modelBuilder.HasSequence<int>("sq_Team", schema: "Application")
                .StartsAt(38187)
                .IncrementsBy(1);

            modelBuilder.Entity<Team>(entity =>
            {
                entity.ToTable("Team", "Application");

                entity.HasKey(e => e.Id);

                entity.Property(x => x.Id)
                    .HasColumnType("INT")
                    .HasColumnName("TeamID")
                    .UseHiLo("sq_Team", "Application")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Name)
                    .HasColumnType("NVARCHAR(255)")
                    .HasColumnName("Name")
                    .IsRequired(true)
                    .HasMaxLength(255);

                entity.Property(e => e.Description)
                    .HasColumnType("NVARCHAR(2000)")
                    .HasColumnName("Description")
                    .IsRequired(true)
                    .HasMaxLength(2000);

                entity.Property(e => e.RowVersion)
                    .HasColumnType("ROWVERSION")
                    .HasColumnName("RowVersion")
                    .IsConcurrencyToken()
                    .IsRequired(false)
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.ValidFrom)
                    .HasColumnType("DATETIME2(7)")
                    .HasColumnName("ValidFrom")
                    .IsRequired(false)
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.ValidTo)
                    .HasColumnType("DATETIME2(7)")
                    .HasColumnName("ValidTo")
                    .IsRequired(false)
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.LastEditedBy)
                    .HasColumnType("INT")
                    .HasColumnName("LastEditedBy")
                    .IsRequired(true);
            });

            modelBuilder.HasSequence<int>("sq_RelationTuple", schema: "Identity")
                .StartsAt(38187)
                .IncrementsBy(1);

            modelBuilder.Entity<RelationTuple>(entity =>
            {
                entity.ToTable("RelationTuple", "Identity");

                entity.HasKey(e => e.Id);

                entity.Property(x => x.Id)
                    .HasColumnType("INT")
                    .HasColumnName("RelationTupleID")
                    .UseHiLo("sq_RelationTuple", "Identity")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.ObjectKey)
                    .HasColumnType("INT")
                    .HasColumnName("ObjectKey")
                    .IsRequired(true);

                entity.Property(e => e.ObjectNamespace)
                    .HasColumnType("NVARCHAR(50)")
                    .HasColumnName("ObjectNamespace")
                    .IsRequired(true)
                    .HasMaxLength(50);

                entity.Property(e => e.ObjectRelation)
                    .HasColumnType("NVARCHAR(50)")
                    .HasColumnName("ObjectRelation")
                    .IsRequired(true)
                    .HasMaxLength(50);

                entity.Property(e => e.SubjectKey)
                    .HasColumnType("INT")
                    .HasColumnName("SubjectKey")
                    .IsRequired(true);

                entity.Property(e => e.SubjectNamespace)
                    .HasColumnType("NVARCHAR(50)")
                    .HasColumnName("SubjectNamespace")
                    .IsRequired(true)
                    .HasMaxLength(50);

                entity.Property(e => e.SubjectRelation)
                    .HasColumnType("NVARCHAR(50)")
                    .HasColumnName("SubjectRelation")
                    .IsRequired(false)
                    .HasMaxLength(50);

                entity.Property(e => e.RowVersion)
                    .HasColumnType("ROWVERSION")
                    .HasColumnName("RowVersion")
                    .IsConcurrencyToken()
                    .IsRequired(false)
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.ValidFrom)
                    .HasColumnType("DATETIME2(7)")
                    .HasColumnName("ValidFrom")
                    .IsRequired(false)
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.ValidTo)
                    .HasColumnType("DATETIME2(7)")
                    .HasColumnName("ValidTo")
                    .IsRequired(false)
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.LastEditedBy)
                    .HasColumnType("INT")
                    .HasColumnName("LastEditedBy")
                    .IsRequired(true);
            });

            modelBuilder.HasSequence<int>("sq_User", schema: "Identity")
                .StartsAt(38187)
                .IncrementsBy(1);

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("User", "Identity");

                entity.HasKey(e => e.Id);

                entity.Property(x => x.Id)
                    .HasColumnType("INT")
                    .HasColumnName("UserID")
                    .UseHiLo("sq_User", "Identity")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.FullName)
                    .HasColumnType("NVARCHAR(50)")
                    .HasColumnName("FullName")
                    .IsRequired(true)
                    .HasMaxLength(50);

                entity.Property(e => e.PreferredName)
                    .HasColumnType("NVARCHAR(50)")
                    .HasColumnName("PreferredName")
                    .IsRequired(true)
                    .HasMaxLength(50);

                entity.Property(e => e.IsPermittedToLogon)
                    .HasColumnType("BIT")
                    .HasColumnName("IsPermittedToLogon")
                    .IsRequired(true)
                    .HasMaxLength(50);

                entity.Property(e => e.LogonName)
                    .HasColumnType("NVARCHAR(256)")
                    .HasColumnName("LogonName")
                    .IsRequired(false)
                    .HasMaxLength(50);

                entity.Property(e => e.HashedPassword)
                    .HasColumnType("NVARCHAR(MAX)")
                    .HasColumnName("HashedPassword")
                    .IsRequired(false);

                entity.Property(e => e.RowVersion)
                    .HasColumnType("ROWVERSION")
                    .HasColumnName("RowVersion")
                    .IsRequired(false)
                    .IsConcurrencyToken()
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.ValidFrom)
                    .HasColumnType("DATETIME2(7)")
                    .HasColumnName("ValidFrom")
                    .IsRequired(false)
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.ValidTo)
                    .HasColumnType("DATETIME2(7)")
                    .HasColumnName("ValidTo")
                    .IsRequired(false)
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.LastEditedBy)
                    .HasColumnType("INT")
                    .HasColumnName("LastEditedBy")
                    .IsRequired(true);
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("Role", "Identity");

                entity.HasKey(e => e.Id);

                entity.Property(x => x.Id)
                    .HasColumnType("INT")
                    .HasColumnName("RoleID")
                    .UseHiLo("[Application].[sq_Role]")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Name)
                    .HasColumnType("NVARCHAR(255)")
                    .HasColumnName("Name")
                    .IsRequired(true)
                    .HasMaxLength(255);

                entity.Property(e => e.Description)
                    .HasColumnType("NVARCHAR(2000)")
                    .HasColumnName("Description")
                    .IsRequired(true)
                    .HasMaxLength(2000);

                entity.Property(e => e.RowVersion)
                    .HasColumnType("ROWVERSION")
                    .HasColumnName("RowVersion")
                    .IsConcurrencyToken()
                    .IsRequired(false)
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.ValidFrom)
                    .HasColumnType("DATETIME2(7)")
                    .HasColumnName("ValidFrom")
                    .IsRequired(false)
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.ValidTo)
                    .HasColumnType("DATETIME2(7)")
                    .HasColumnName("ValidTo")
                    .IsRequired(false)
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.LastEditedBy)
                    .HasColumnType("INT")
                    .HasColumnName("LastEditedBy")
                    .IsRequired(true);
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
