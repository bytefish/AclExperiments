// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using AclExperiments.Database.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Data;

namespace AclExperiments.Database
{
    /// <summary>
    /// The <see cref="DbContext"/> to query for <see cref="SqlRelationTuple"/> and <see cref="SqlNamespaceConfiguration"/>.
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
        /// <param name="options">Options to configure the <see cref="DbContext"/></param>
        public ApplicationDbContext(ILogger<ApplicationDbContext> logger, DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
            Logger = logger;
        }

        /// <summary>
        /// Gets or sets the DbSet to query for <see cref="SqlRelationTuple"/>.
        /// </summary>
        public DbSet<SqlRelationTuple> SqlRelationTuples { get; set; }

        /// <summary>
        /// Gets or sets the DbSet to query for <see cref="SqlNamespaceConfiguration"/>.
        /// </summary>
        public DbSet<SqlNamespaceConfiguration> SqlNamespaceConfigurations { get; set; }

        /// <summary>
        /// Gets or sets the DbSet to query for <see cref="SqlUser"/>.
        /// </summary>
        public DbSet<SqlUser> SqlUsers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Now create the Tables
            modelBuilder.HasSequence<int>("sq_RelationTuple", schema: "Identity")
                .StartsAt(38187)
                .IncrementsBy(1);

            modelBuilder.Entity<SqlRelationTuple>(entity =>
            {
                entity.ToTable("RelationTuple", "Identity");

                entity.HasKey(e => e.Id);

                entity.Property(x => x.Id)
                    .HasColumnType("INT")
                    .HasColumnName("RelationTupleID")
                    .UseHiLo("sq_RelationTuple", "Identity")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Namespace)
                    .HasColumnType("NVARCHAR(255)")
                    .HasColumnName("Namespace")
                    .IsRequired(true)
                    .HasMaxLength(255);
                
                entity.Property(e => e.Object)
                    .HasColumnType("NVARCHAR(255)")
                    .HasColumnName("Object")
                    .IsRequired(true)
                    .HasMaxLength(255);

                entity.Property(e => e.Relation)
                    .HasColumnType("NVARCHAR(255)")
                    .HasColumnName("Relation")
                    .IsRequired(true)
                    .HasMaxLength(255);

                entity.Property(e => e.Subject)
                    .HasColumnType("NVARCHAR(255)")
                    .HasColumnName("Subject")
                    .IsRequired(true)
                    .HasMaxLength(255);

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


            modelBuilder.HasSequence<int>("sq_NamespaceConfiguration", schema: "Identity")
                .StartsAt(38187)
                .IncrementsBy(1);

            modelBuilder.Entity<SqlNamespaceConfiguration>(entity =>
            {
                entity.ToTable("NamespaceConfiguration", "Identity");

                entity.HasKey(e => e.Id);

                entity.Property(x => x.Id)
                    .HasColumnType("INT")
                    .HasColumnName("NamespaceConfigurationID")
                    .UseHiLo("sq_NamespaceConfiguration", "Identity")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Name)
                    .HasColumnType("NVARCHAR(255)")
                    .HasColumnName("Name")
                    .IsRequired(true)
                    .HasMaxLength(255);

                entity.Property(e => e.Version)
                    .HasColumnType("INT")
                    .HasColumnName("Version")
                    .IsRequired(true);

                entity.Property(e => e.Content)
                    .HasColumnType("NVARCHAR(MAX)")
                    .HasColumnName("Content")
                    .IsRequired(true)
                    .HasMaxLength(-1);

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

            modelBuilder.Entity<SqlUser>(entity =>
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

            base.OnModelCreating(modelBuilder);
        }
    }
}
