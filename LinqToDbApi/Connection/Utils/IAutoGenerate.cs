using System;
using System.Collections.Generic;

namespace LinqToDbApi.Connection.Utils
{
    /// <summary>
    /// Interface of auto generation tables that don't exist
    /// </summary>
    public interface IAutoGenerate
    {
        /// <summary>
        /// Enumerable of ITable Types
        /// </summary>
        IEnumerable<Type> SelfInitialize { get; }

        /// <summary>
        /// Function of generating tables
        /// </summary>
        void AutoGenerate();
    }
}