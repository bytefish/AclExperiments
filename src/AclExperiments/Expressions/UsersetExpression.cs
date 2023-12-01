﻿// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace AclExperiments.Expressions
{
    /// <summary>
    /// Everything in the Google Zanzibar configuration language can be expressed as a 
    /// Userset Expression. It also defines a visitor for visiting all Nodes of 
    /// a <see cref="UsersetExpression"/> tree.
    /// </summary>
    public abstract record UsersetExpression
    {
    }
}