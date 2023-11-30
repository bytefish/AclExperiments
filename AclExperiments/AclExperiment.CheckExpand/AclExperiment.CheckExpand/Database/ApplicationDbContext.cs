using AclExperiment.CheckExpand.Database.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AclExperiment.CheckExpand.Database
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<SqlRelationTuple> SqlRelationTuples { get; set; }
    }
}
