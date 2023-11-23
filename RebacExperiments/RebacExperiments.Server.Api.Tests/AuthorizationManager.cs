using System.Collections.Generic;

namespace RebacExperiments.Server.Api.Tests
{
    /// <summary>
    /// Everything in the Google Zanzibar configuration language can be expressed as a 
    /// Userset Expression. It also defines a visitor for visiting all Nodes of 
    /// a <see cref="UsersetExpression"/> tree.
    /// </summary>
    public abstract class UsersetExpression
    {
        public interface Visitor<T>
        {
            T VisitChildUsersetExpr(ChildUsersetExpression expr);

            T VisitComputedUsersetExpr(ComputedUsersetExpression expr);

            T VisitNamespaceUsersetExpr(NamespaceUsersetExpression expr);

            T VisitRelationUsersetExpr(RelationUsersetExpression expr);

            T VisitSetOperationExpr(SetOperationUsersetExpression expr);

            T VisitThisUsersetExpr(ThisUsersetExpression expr);

            T VisitTuplesetExpr(TuplesetExpression expr);

            T VisitTupleToUsersetExpr(TupleToUsersetExpression expr);
        }

        /// <summary>
        /// Visits the current tree node.
        /// </summary>
        /// <typeparam name="T">Type of the Visitor</typeparam>
        /// <param name="visitor">The Node Visitor</param>
        /// <returns>Visitor Type</returns>
        public abstract T Accept<T>(Visitor<T> visitor);
    }

    /// <summary>
    /// The root node of the Zanzibar Configuration language. It contains the 
    /// name of the configured subject and an optional list of relations, expressed 
    /// as <see cref="RelationUsersetExpression"/>.
    /// </summary>
    public class NamespaceUsersetExpression : UsersetExpression
    {
        /// <summary>
        /// Gets or sets the Namespace being configured.
        /// </summary>
        public required string Name { get; set; }

        /// <summary>
        /// Gets or sets the Relations expressed by the Namespace configuration.
        /// </summary>
        public Dictionary<string, RelationUsersetExpression> Relations { get; set; } = new();

        public override T Accept<T>(Visitor<T> visitor)
        {
            return visitor.VisitNamespaceUsersetExpr(this);
        }
    }

    /// <summary>
    /// A Relation is expressed by its name and an optional rewrite, which is expressed as a 
    /// <see cref="UsersetExpression"/>. 
    /// </summary>
    public class RelationUsersetExpression : UsersetExpression
    {
        public required string Name { get; set; }

        public UsersetExpression? Rewrite { get; set; }

        public override T Accept<T>(Visitor<T> visitor)
        {
            return visitor.VisitRelationUsersetExpr(this);
        }
    }

    /// <summary>
    /// The Set Operation to apply for a <see cref="UsersetExpression"/>.
    /// </summary>
    public enum SetOperationEnum
    {
        /// <summary>
        /// Unions together the relations/permissions referenced.
        /// </summary>
        Union = 1,

        /// <summary>
        /// Intersects the set of subjects found for the relations/permissions referenced.
        /// </summary>
        Intersection = 2,

        /// <summary>
        /// Intersects the set of subjects found for the relations/permissions referenced.
        /// </summary>
        Exclusion = 3,
    }

    /// <summary>
    /// Userset Expressions can be expressed as a union, intersection, ... and more 
    /// set operations, so we are able to define more complex authorization rules..
    /// </summary>
    public class SetOperationUsersetExpression : UsersetExpression
    {
        /// <summary>
        /// Gets or sets the Set Operation, such as a Union.
        /// </summary>
        public SetOperationEnum Operation { get; set; }

        /// <summary>
        /// Gets or sets the Children.
        /// </summary>
        public required List<UsersetExpression> Children { get; set; }

        public override T Accept<T>(Visitor<T> visitor)
        {
            return visitor.VisitSetOperationExpr(this);
        }
    }

    /// <summary>
    /// A Leaf Node of a <see cref="SetOperationUsersetExpression"/>.
    /// </summary>
    public class ChildUsersetExpression : UsersetExpression
    {
        /// <summary>
        /// Gets or sets the Userset Expression for this leaf node.
        /// </summary>
        public required UsersetExpression Userset { get; set; }

        public override T Accept<T>(Visitor<T> visitor)
        {
            return visitor.VisitChildUsersetExpr(this);
        }
    }

    /// <summary>
    /// Returns all users from stored relation tuples for the <code>object#relation</code> pair, including 
    /// indirect ACLs referenced by usersets from the tuples.This is the default behavior when no rewrite
    /// rule is specified.
    /// </summary>
    public class ThisUsersetExpression : UsersetExpression
    {
        public override T Accept<T>(Visitor<T> visitor)
        {
            return visitor.VisitThisUsersetExpr(this);
        }
    }

    /// <summary>
    /// Computes, for the input object, a new userset. For example, this allows the userset expression for 
    /// a viewer relation to refer to the editor userset on the same object, thus offering an ACL inheritance
    /// capability between relations.
    /// </summary>
    public class ComputedUsersetExpression : UsersetExpression
    {
        /// <summary>
        /// Gets or sets the Object,
        /// </summary>
        public string? Object { get; set; }

        /// <summary>
        /// Gets or sets the Relation.
        /// </summary>
        public required string Relation { get; set; }

        public override T Accept<T>(Visitor<T> visitor)
        {
            return visitor.VisitComputedUsersetExpr(this);
        }
    }

    /// <summary>
    /// Each tupleset specifies keys of a set of relation tuples. The set can include a single tuple key, or 
    /// all tuples with a given object ID or userset in a namespace, optionally constrained by a relation 
    /// name.
    /// </summary>
    public class TuplesetExpression : UsersetExpression
    {
        /// <summary>
        /// Gets or sets the Namespace.
        /// </summary>
        public string? Namespace;

        /// <summary>
        /// Gets or sets the Object.
        /// </summary>
        public string? Object { get; set; }

        /// <summary>
        /// Gets or sets the Relation.
        /// </summary>
        public required string Relation { get; set; }

        public override T Accept<T>(Visitor<T> visitor)
        {
            return visitor.VisitTuplesetExpr(this);
        }
    }

    /// <summary>
    ///  Computes a tupleset (§2.4.1) from the input object, fetches relation tuples matching the tupleset, and computes 
    ///  a userset from every fetched relation tuple.This flexible primitive allows our clients to express complex
    ///  policies such as "Look up the 'parent' Folder of the Document and inherit 
    ///  its 'viewers'".
    /// </summary>
    public class TupleToUsersetExpression : UsersetExpression
    {
        /// <summary>
        /// Gets or sets the Tupleset.
        /// </summary>
        public required TuplesetExpression TuplesetExpression { get; set; }

        /// <summary>
        /// Gets or sets the Computer Userset.
        /// </summary>
        public required ComputedUsersetExpression ComputedUsersetExpression { get; set; }

        public override T Accept<T>(Visitor<T> visitor)
        {
            return visitor.VisitTupleToUsersetExpr(this);
        }
    }

    public class GoogleZanzibarTests
    {
        public void NamespaceConfigurationExample()
        {
            var configuration = new NamespaceUsersetExpression
            {
                Name = "document",
                Relations =
                {
                    {
                        "owner",
                        new RelationUsersetExpression { Name = "owner" }
                    },
                    {
                        "editor",
                        new RelationUsersetExpression
                        {
                            Name = "editor",
                            Rewrite = new SetOperationUsersetExpression
                            {
                                Operation = SetOperationEnum.Union,
                                Children =
                                [
                                    new ThisUsersetExpression(),
                                    new ComputedUsersetExpression { Relation = "owner" }
                                ]
                            }
                        }
                    },
                    {
                        "viewer",
                        new RelationUsersetExpression
                        {
                            Name = "viewer",
                            Rewrite = new SetOperationUsersetExpression
                            {
                                Operation = SetOperationEnum.Union,
                                Children =
                                [
                                    new ThisUsersetExpression(),
                                    new ComputedUsersetExpression { Relation = "editor" },
                                    new TupleToUsersetExpression
                                    {
                                        TuplesetExpression = new TuplesetExpression { Relation = "parent" },
                                        ComputedUsersetExpression = new ComputedUsersetExpression
                                        {
                                            Object = " $TUPLE_USERSET_OBJECT",
                                            Relation = "viewer"
                                        }
                                    }
                                ]
                            }
                        }
                    }
                }
            };

            // Build a simple Visitor:

            
        }
    }
}