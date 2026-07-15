using System;
using System.Collections.Generic;
using System.Text;

namespace OzzieAI.FaissSharp
{


    using global::OzzieAI.FaissSharp.Native;
    using OzzieAI.FaissSharp.Native;
    using System;

    /// <summary>
    /// Supported quantizer types for the Scalar Quantizer.
    /// </summary>
    public enum FaissQuantizerType
    {
        /// <summary>8-bit uniform quantization</summary>
        QT_8bit = 0,
        /// <summary>4-bit uniform quantization</summary>
        QT_4bit = 1,
        /// <summary>8-bit uniform quantization, sharing range across dimensions</summary>
        QT_8bit_uniform = 2,
        /// <summary>4-bit uniform quantization, sharing range across dimensions</summary>
        QT_4bit_uniform = 3,
        /// <summary>Float 16-bit (Half-precision)</summary>
        QT_fp16 = 4,
        /// <summary>8-bit direct quantization (no offset/scale)</summary>
        QT_8bit_direct = 5,
        /// <summary>6-bit uniform quantization</summary>
        QT_6bit = 6
    }

    /// <summary>
    /// An index that compresses floating-point vectors into much smaller scalar representations (e.g., 8-bit or 4-bit ints).
    /// Provides an excellent balance of memory reduction and search speed without the complexity of Product Quantization.
    /// </summary>
    public class IndexScalarQuantizer : FaissIndex
    {
        /// <summary>
        /// Initializes a new Scalar Quantizer index.
        /// </summary>
        /// <param name="dimensions">The dimensionality of the indexed vectors.</param>
        /// <param name="qtype">The level of compression to apply (e.g., 8-bit vs 4-bit vs fp16).</param>
        /// <param name="metric">The distance metric to use (L2 or InnerProduct).</param>
        public IndexScalarQuantizer(int dimensions, FaissQuantizerType qtype, FaissMetricType metric = FaissMetricType.L2)
            : base(Initialize(dimensions, qtype, metric), dimensions)
        {
        }

        private static IntPtr Initialize(int dimensions, FaissQuantizerType qtype, FaissMetricType metric)
        {
            int status = FaissNative.faiss_IndexScalarQuantizer_new(out IntPtr ptr, dimensions, (int)qtype, (int)metric);
            FaissException.ThrowIfError(status);
            return ptr;
        }
    }

    /// <summary>
    /// An index that utilizes Product Quantization (PQ) to heavily compress vectors into tiny byte codes.
    /// Best for massive datasets where RAM is strictly limited.
    /// </summary>
    public class IndexPQ : FaissIndex
    {
        /// <summary>
        /// Initializes a new Product Quantizer index.
        /// </summary>
        /// <param name="dimensions">The dimensionality of the indexed vectors.</param>
        /// <param name="m">The number of sub-vectors to split the original vector into (Dimensions must be a multiple of M).</param>
        /// <param name="nbits">The number of bits used per sub-vector (usually 8).</param>
        public IndexPQ(int dimensions, int m, int nbits = 8)
            : base(Initialize(dimensions, m, nbits), dimensions)
        {
        }

        private static IntPtr Initialize(int dimensions, int m, int nbits)
        {
            int status = FaissNative.faiss_IndexPQ_new(out IntPtr ptr, dimensions, m, nbits);
            FaissException.ThrowIfError(status);
            return ptr;
        }
    }

    /// <summary>
    /// An index utilizing Locality-Sensitive Hashing (LSH).
    /// Maps continuous vectors to binary hashes, where nearby vectors have identical or highly similar hashes.
    /// </summary>
    public class IndexLSH : FaissIndex
    {
        /// <summary>
        /// Initializes a new Locality-Sensitive Hashing index.
        /// </summary>
        /// <param name="dimensions">The dimensionality of the indexed vectors.</param>
        /// <param name="nbits">The size of the output hash signature in bits (more bits = higher accuracy but larger size).</param>
        public IndexLSH(int dimensions, int nbits)
            : base(Initialize(dimensions, nbits), dimensions)
        {
        }

        private static IntPtr Initialize(int dimensions, int nbits)
        {
            int status = FaissNative.faiss_IndexLSH_new(out IntPtr ptr, dimensions, nbits);
            FaissException.ThrowIfError(status);
            return ptr;
        }
    }
}