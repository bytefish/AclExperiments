using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AclExperiments.Expressions
{
    public record MetadataExpression : UsersetExpression
    {
        public required List<MetadataRelationExpression> Relations { get; set; }
    }

    public record MetadataRelationExpression : UsersetExpression
    {
        public required string Name { get; set; }

        public List<DirectlyRelatedUserType> DirectlyRelatedUserTypes { get; set; } = new();
    }

    public record DirectlyRelatedUserType : UsersetExpression
    {
        public required string Namespace { get; set; }

        public required string? Relation { get; set; }
    }
}
