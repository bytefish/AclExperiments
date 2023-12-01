// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Transactions;

namespace LayeredArchitecture.Shared.Data.Transactions
{
    /// <summary>
    /// Simplifies working with Transactions.
    /// </summary>
    public static class TransactionScopeManager
    {
        /// <summary>
        /// Creates a TransactionScope with sane default values.
        /// </summary>
        /// <param name="transactionScopeSettings">Settings for the <see cref="TransactionScope"/> to build</param>
        /// <returns>A <see cref="TransactionScope"/></returns>
        public static TransactionScope CreateTransactionScope(TransactionSettings transactionScopeSettings)
        {
            var transactionOptions = new TransactionOptions
            {
                IsolationLevel = transactionScopeSettings.IsolationLevel,
                Timeout = transactionScopeSettings.Timeout
            };

            return new TransactionScope(transactionScopeSettings.TransactionScopeOption, transactionOptions, transactionScopeSettings.AsyncFlowOption);
        }
    }
}