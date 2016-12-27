using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NaughtyKidMVVM.Helpers
{
    public interface IExecuteWithObjectAndResult
    {
        /// <summary>
        /// Executes a Func and returns the result.
        /// </summary>
        /// <param name="parameter">A parameter passed as an object,
        /// to be casted to the appropriate type.</param>
        /// <returns>The result of the operation.</returns>
        object ExecuteWithObject(object parameter);
    }
}
