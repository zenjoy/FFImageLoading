﻿using System;
using System.Diagnostics;

namespace FFImageLoading.Helpers
{
    internal class MiniLogger : IMiniLogger
    {
        public void Debug(string message)
        {
            DebugInternal(message);
        }

        public void Error(string errorMessage, Exception ex)
        {
            // TODO WinRT doesn't have Console!
            // Console.WriteLine(errorMessage + Environment.NewLine + ex.ToString());
            System.Diagnostics.Debug.WriteLine(errorMessage + Environment.NewLine + ex.ToString());
        }

        [Conditional("DEBUG")]
        private void DebugInternal(string message)
        {
            System.Diagnostics.Debug.WriteLine(message);
        }
    }
}

