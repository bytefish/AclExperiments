// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace AclExperiments.Expressions
{
    /// <summary>
    /// Base class for all Userset Expressions.
    /// </summary>
    [JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
    [JsonDerivedType(typeof(SetOperationUsersetExpression), typeDiscriminator: "set_operation")]
    [JsonDerivedType(typeof(ThisUsersetExpression), typeDiscriminator: "_this")]
    [JsonDerivedType(typeof(ComputedUsersetExpression), typeDiscriminator: "computed_userset")]
    [JsonDerivedType(typeof(TupleToUsersetExpression), typeDiscriminator: "tuple_to_userset")]
    public abstract record UsersetExpression
    {
    }
}
