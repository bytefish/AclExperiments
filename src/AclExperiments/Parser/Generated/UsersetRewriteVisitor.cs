//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     ANTLR Version: 4.13.1
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from C:/Users/philipp/source/repos/bytefish/AclExperiments//src/AclExperiments/Parser/UsersetRewrite.g4 by ANTLR 4.13.1

// Unreachable code detected
#pragma warning disable 0162
// The variable '...' is assigned but its value is never used
#pragma warning disable 0219
// Missing XML comment for publicly visible type or member '...'
#pragma warning disable 1591
// Ambiguous reference in cref attribute
#pragma warning disable 419

namespace AclExperiments.Parser.Generated {
#pragma warning disable 3021
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using IToken = Antlr4.Runtime.IToken;

/// <summary>
/// This interface defines a complete generic visitor for a parse tree produced
/// by <see cref="UsersetRewriteParser"/>.
/// </summary>
/// <typeparam name="Result">The return type of the visit operation.</typeparam>
[System.CodeDom.Compiler.GeneratedCode("ANTLR", "4.13.1")]
[System.CLSCompliant(false)]
public interface IUsersetRewriteVisitor<Result> : IParseTreeVisitor<Result> {
	/// <summary>
	/// Visit a parse tree produced by <see cref="UsersetRewriteParser.namespace"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitNamespace([NotNull] UsersetRewriteParser.NamespaceContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="UsersetRewriteParser.relation"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitRelation([NotNull] UsersetRewriteParser.RelationContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="UsersetRewriteParser.metadata"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitMetadata([NotNull] UsersetRewriteParser.MetadataContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="UsersetRewriteParser.metadataRelation"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitMetadataRelation([NotNull] UsersetRewriteParser.MetadataRelationContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="UsersetRewriteParser.directlyRelatedType"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitDirectlyRelatedType([NotNull] UsersetRewriteParser.DirectlyRelatedTypeContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="UsersetRewriteParser.directlyRelatedTypeNamespaceRef"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitDirectlyRelatedTypeNamespaceRef([NotNull] UsersetRewriteParser.DirectlyRelatedTypeNamespaceRefContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="UsersetRewriteParser.directlyRelatedTypeRelationRef"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitDirectlyRelatedTypeRelationRef([NotNull] UsersetRewriteParser.DirectlyRelatedTypeRelationRefContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="UsersetRewriteParser.usersetRewrite"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitUsersetRewrite([NotNull] UsersetRewriteParser.UsersetRewriteContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="UsersetRewriteParser.userset"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitUserset([NotNull] UsersetRewriteParser.UsersetContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="UsersetRewriteParser.childUserset"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitChildUserset([NotNull] UsersetRewriteParser.ChildUsersetContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="UsersetRewriteParser.computedUserset"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitComputedUserset([NotNull] UsersetRewriteParser.ComputedUsersetContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="UsersetRewriteParser.usersetNamespaceRef"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitUsersetNamespaceRef([NotNull] UsersetRewriteParser.UsersetNamespaceRefContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="UsersetRewriteParser.usersetObjectRef"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitUsersetObjectRef([NotNull] UsersetRewriteParser.UsersetObjectRefContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="UsersetRewriteParser.usersetRelationRef"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitUsersetRelationRef([NotNull] UsersetRewriteParser.UsersetRelationRefContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="UsersetRewriteParser.thisUserset"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitThisUserset([NotNull] UsersetRewriteParser.ThisUsersetContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="UsersetRewriteParser.tupleToUserset"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitTupleToUserset([NotNull] UsersetRewriteParser.TupleToUsersetContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="UsersetRewriteParser.tupleset"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitTupleset([NotNull] UsersetRewriteParser.TuplesetContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="UsersetRewriteParser.namespaceRef"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitNamespaceRef([NotNull] UsersetRewriteParser.NamespaceRefContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="UsersetRewriteParser.objectRef"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitObjectRef([NotNull] UsersetRewriteParser.ObjectRefContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="UsersetRewriteParser.relationRef"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitRelationRef([NotNull] UsersetRewriteParser.RelationRefContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="UsersetRewriteParser.setOperationUserset"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitSetOperationUserset([NotNull] UsersetRewriteParser.SetOperationUsersetContext context);
}
} // namespace AclExperiments.Parser.Generated
