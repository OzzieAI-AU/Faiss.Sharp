namespace OzzieAI.FaissSharp
{


    using FaissSharp.Native;
    using OzzieAI.FaissSharp.Native;
    using System;


    /// <summary>
    /// Defines the metric types used by FAISS.
    /// </summary>
    public enum FaissMetricType
    {
        InnerProduct = 0,
        L2 = 1,
        L1 = 2,
        LInf = 3
    }

    /// <summary>
    /// Provides utility methods to build complex FAISS indexes using strings.
    /// </summary>
    public static class IndexFactory
    {
        /// <summary>
        /// Creates an index based on a standard FAISS description string.
        /// Example description: "IVF1024,Flat" (Inverted file with 1024 centroids, flat storage).
        /// </summary>
        public static FaissIndex Create(int dimensions, string description, FaissMetricType metric = FaissMetricType.L2)
        {
            int status = FaissNative.faiss_index_factory(out IntPtr ptr, dimensions, description, (int)metric);
            FaissException.ThrowIfError(status);
            return new GenericFaissIndex(ptr, dimensions);
        }
    }

    /// <summary>
    /// Handles reading and writing indexes to and from the disk.
    /// </summary>
    public static class FaissIO
    {
        /// <summary>
        /// Saves a trained/populated index to a file on disk.
        /// </summary>
        public static void WriteIndex(FaissIndex index, string filePath)
        {
            if (index == null || index.Handle.IsInvalid)
                throw new ArgumentNullException(nameof(index));

            int status = FaissNative.faiss_write_index_fname(index.Handle.DangerousGetHandle(), filePath);
            FaissException.ThrowIfError(status);
        }

        /// <summary>
        /// Loads an index from a file on disk. 
        /// Note: The dimensionality must be known or retrieved after loading, 
        /// but for generic wrappers we typically assume the caller knows the dimension.
        /// </summary>
        public static FaissIndex ReadIndex(string filePath, int dimensions)
        {
            // io_flags = 0 means default read mode
            int status = FaissNative.faiss_read_index_fname(filePath, 0, out IntPtr ptr);
            FaissException.ThrowIfError(status);
            return new GenericFaissIndex(ptr, dimensions);
        }
    }
}