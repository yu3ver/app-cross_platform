using System;
using System.Diagnostics;

namespace Integreat.Shared.Debugging
{
    public static class AsyncErrorHandler
    {
       public static void HandleException(Exception exception)
        {
            Debug.WriteLine(exception);
        }
    }
}
