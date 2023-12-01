// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace AclExperiments.Expressions
{
    /// <summary>
    /// Returns all users from stored relation tuples for the <code>object#relation</code> pair, including 
    /// indirect ACLs referenced by usersets from the tuples.This is the default behavior when no rewrite
    /// rule is specified.
    /// </summary>
    public record ThisUsersetExpression : UsersetExpression
    {
    }
}
