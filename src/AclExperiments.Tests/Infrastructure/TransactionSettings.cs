// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Transactions;

namespace LayeredArchitecture.Shared.Data.Transactions
{
    /// <summary>
    /// Settings used to create a new <see cref="TransactionScope"/>
    /// </summary>
    public record TransactionSettings
    {
        /// <summary>
        /// Gets or sets the TransactionScopeOption, defaults to <see cref="TransactionScopeOption.Required"/>.
        /// </summary>
        public TransactionScopeOption TransactionScopeOption { get; set; } = TransactionScopeOption.Required;

        /// <summary>
        /// Gets or sets the Isolation Level, defaults to <see cref="IsolationLevel.ReadCommitted"/>.
        /// </summary>
        public IsolationLevel IsolationLevel { get; set; } = IsolationLevel.ReadCommitted;

        /// <summary>
        /// Gets or sets the Async Flow Option for the Transaction, defaults to <see cref="TransactionScopeAsyncFlowOption.Enabled"/>.
        /// </summary>
        public TransactionScopeAsyncFlowOption AsyncFlowOption { get; set; } = TransactionScopeAsyncFlowOption.Enabled;

        /// <summary>
        /// Gets or sets the Transaction Timeout, defaults to <see cref="TransactionManager.DefaultTimeout"/>.
        /// </summary>
        public TimeSpan Timeout = TransactionManager.DefaultTimeout;
    }
}