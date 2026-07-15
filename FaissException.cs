namespace OzzieAI.FaissSharp
{

    using FaissSharp.Native;
    using Microsoft.Win32.SafeHandles;
    using OzzieAI.FaissSharp.Native;
    using System;
    using System.Runtime.InteropServices;


    /// <summary>
    /// Custom exception that retrieves and formats the internal C++ error message from FAISS.
    /// </summary>
    public class FaissException : Exception
    {
        public FaissException(string message) : base(message) { }

        /// <summary>
        /// Checks the return code from a FAISS C API call. If it's non-zero, it fetches
        /// the last error from the native library and throws a FaissException.
        /// </summary>
        /// <param name="statusCode">The integer return code from the native function.</param>
        public static void ThrowIfError(int statusCode)
        {
            if (statusCode == 0) return; // 0 indicates success

            IntPtr errorPtr = FaissNative.faiss_get_last_error();
            string errorMessage = errorPtr != IntPtr.Zero
                ? Marshal.PtrToStringAnsi(errorPtr) ?? "Unknown FAISS error."
                : "Unknown FAISS error.";

            throw new FaissException($"FAISS Native Error: {errorMessage}");
        }
    }

    /// <summary>
    /// A SafeHandle wrapper for the FaissIndex native pointer. 
    /// Ensures that the C++ memory is freed even if the consumer forgets to call Dispose().
    /// </summary>
    internal class FaissIndexHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        public FaissIndexHandle() : base(true) { }
        public FaissIndexHandle(IntPtr preExistingHandle, bool ownsHandle) : base(ownsHandle)
        {
            SetHandle(preExistingHandle);
        }

        protected override bool ReleaseHandle()
        {
            if (handle != IntPtr.Zero)
            {
                FaissNative.faiss_Index_free(handle);
                handle = IntPtr.Zero;
            }
            return true;
        }
    }
}