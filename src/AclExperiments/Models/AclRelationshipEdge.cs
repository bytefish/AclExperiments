using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AclExperiments.Models
{
    /// <summary>
    /// 
    /// </summary>
    public record RelationshipEdge
    {
        /// <summary>
        /// 
        /// </summary>
        public required RelationReference TargetReference { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? TuplesetRelation { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public required bool TargetIsIntersectionOrExclusion { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public required RelationEdgeTypeEnum RelationEdgeType { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public record RelationReference
    {
        /// <summary>
        /// 
        /// </summary>
        public required string Namespace { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public required string Relation { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public enum RelationEdgeTypeEnum
    {
        /// <summary>
        /// Direct.
        /// </summary>
        DirectEdge = 0,

        /// <summary>
        /// Computed Userset.
        /// </summary>
        ComputedUserset = 1,

        /// <summary>
        /// Tuple To Userset.
        /// </summary>
        TupleToUserset = 2
    }

}
