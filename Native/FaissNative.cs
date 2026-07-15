namespace OzzieAI.FaissSharp.Native
{


    using System;
    using System.Runtime.InteropServices;


    /// <summary>
    /// Contains all the P/Invoke declarations for the FAISS C API.
    /// Maps to the faiss_c dynamic library (libfaiss_c.so, faiss_c.dll, or libfaiss_c.dylib).
    /// </summary>
    internal static class FaissNative
    {

        private const string DllName = "dll/faiss.dll";

        // ==========================================
        // Error Handling
        // ==========================================

        /// <summary>
        /// Retrieves the last error message raised by the FAISS C API.
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr faiss_get_last_error();

        // ==========================================
        // Index Base Operations (Index_c.h)
        // ==========================================

        /// <summary>
        /// Frees the memory associated with a FaissIndex.
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void faiss_Index_free(IntPtr index);

        /// <summary>
        /// Gets the total number of vectors currently indexed.
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern long faiss_Index_ntotal(IntPtr index);

        /// <summary>
        /// Returns whether the index is trained.
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int faiss_Index_is_trained(IntPtr index);

        /// <summary>
        /// Adds n vectors of dimension d to the index.
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int faiss_Index_add(IntPtr index, long n, [In] float[] x);

        /// <summary>
        /// Searches for the k nearest neighbors of the n query vectors.
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int faiss_Index_search(IntPtr index, long n, [In] float[] x, long k, [Out] float[] distances, [Out] long[] labels);

        /// <summary>
        /// Trains the index on a representative set of vectors (required for IVF, PQ, etc.).
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int faiss_Index_train(IntPtr index, long n, [In] float[] x);

        /// <summary>
        /// Removes all vectors from the index.
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int faiss_Index_reset(IntPtr index);

        // ==========================================
        // Flat Indexes (IndexFlat_c.h)
        // ==========================================

        /// <summary>
        /// Creates a new Flat index for L2 (Euclidean) distance.
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int faiss_IndexFlatL2_new(out IntPtr index, int d);

        /// <summary>
        /// Creates a new Flat index for Inner Product (Cosine similarity if normalized).
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int faiss_IndexFlatIP_new(out IntPtr index, int d);

        // ==========================================
        // Index Factory (AutoTune_c.h)
        // ==========================================

        /// <summary>
        /// Creates a complex index using a description string (e.g., "IVF1024,Flat").
        /// metric is usually 1 (L2) or 2 (Inner Product).
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern int faiss_index_factory(out IntPtr index, int d, string description, int metric);

        // ==========================================
        // I/O Operations (index_io_c.h)
        // ==========================================

        /// <summary>
        /// Writes an index to a file on disk.
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern int faiss_write_index_fname(IntPtr index, string fname);

        /// <summary>
        /// Reads an index from a file on disk.
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern int faiss_read_index_fname(string fname, int io_flags, out IntPtr index);

        // ==========================================
        // Index Properties (Index_c.h)
        // ==========================================

        /// <summary>
        /// Gets the dimensionality of the index.
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int faiss_Index_d(IntPtr index);

        /// <summary>
        /// Gets the metric type of the index (L2, InnerProduct, etc.).
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int faiss_Index_metric_type(IntPtr index);

        // ==========================================
        // Advanced Index Operations (Index_c.h)
        // ==========================================

        /// <summary>
        /// Adds n vectors with specific IDs to the index (not supported by all indexes natively).
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int faiss_Index_add_with_ids(IntPtr index, long n, [In] float[] x, [In] long[] xids);

        /// <summary>
        /// Removes vectors from the index based on an IDSelector.
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int faiss_Index_remove_ids(IntPtr index, IntPtr sel, out long n_removed);

        /// <summary>
        /// Reconstructs a vector from the index given its ID.
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int faiss_Index_reconstruct(IntPtr index, long key, [Out] float[] recons);

        /// <summary>
        /// Reconstructs multiple vectors from the index.
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int faiss_Index_reconstruct_n(IntPtr index, long i0, long ni, [Out] float[] recons);

        /// <summary>
        /// Computes the distances to the nearest centroids and their labels.
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int faiss_Index_assign(IntPtr index, long n, [In] float[] x, [Out] long[] labels, long k);

        /// <summary>
        /// Clones an index completely into a new pointer in memory.
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int faiss_clone_index(IntPtr index, out IntPtr p_out);

        // ==========================================
        // ID Selectors (AuxIndexStructures_c.h)
        // ==========================================

        /// <summary>
        /// Frees the memory associated with an IDSelector.
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void faiss_IDSelector_free(IntPtr sel);

        /// <summary>
        /// Creates an ID selector that captures a range of IDs (imin <= ID < imax).
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int faiss_IDSelectorRange_new(out IntPtr p_sel, long imin, long imax);

        /// <summary>
        /// Creates an ID selector that captures specific batched IDs.
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int faiss_IDSelectorBatch_new(out IntPtr p_sel, long n, [In] long[] indices);

        // ==========================================
        // ID Maps and Complex Indexes (IndexIDMap_c.h, IndexIVF_c.h, etc.)
        // ==========================================

        /// <summary>
        /// Wraps an existing index inside an IDMap, allowing it to support AddWithIds.
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int faiss_IndexIDMap_new(out IntPtr p_index, IntPtr index);

        /// <summary>
        /// Creates a direct map for an IVF index (necessary for reconstruction in IVF indexes).
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int faiss_IndexIVF_make_direct_map(IntPtr index, int new_maintain_direct_map);

        /// <summary>
        /// Creates an IVF Flat index.
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int faiss_IndexIVFFlat_new(out IntPtr p_index, IntPtr quantizer, long d, long nlist);

        /// <summary>
        /// Creates an IVF Flat index with a specific metric.
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int faiss_IndexIVFFlat_new_with_metric(out IntPtr p_index, IntPtr quantizer, long d, long nlist, int metric);

        /// <summary>
        /// Creates an HNSW Flat index.
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int faiss_IndexHNSWFlat_new(out IntPtr p_index, int d, int M);

        /// <summary>
        /// Creates a Locality Sensitive Hashing (LSH) index.
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int faiss_IndexLSH_new(out IntPtr p_index, int d, int nbits);

        /// <summary>
        /// Creates a Scalar Quantizer index.
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int faiss_IndexScalarQuantizer_new(out IntPtr p_index, int d, int qtype, int metric);

        /// <summary>
        /// Creates a Product Quantizer index.
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int faiss_IndexPQ_new(out IntPtr p_index, int d, int M, int nbits);

        // ==========================================
        // Index Parameters & AutoTuning (AutoTune_c.h)
        // ==========================================

        /// <summary>
        /// Creates a new ParameterSpace object, used to dynamically tune index parameters.
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int faiss_ParameterSpace_new(out IntPtr p_space);

        /// <summary>
        /// Frees a ParameterSpace object.
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void faiss_ParameterSpace_free(IntPtr space);

        /// <summary>
        /// Dynamically sets a specific configuration parameter on an index (e.g., "nprobe=10" for IVF).
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern int faiss_ParameterSpace_set_index_parameter(IntPtr space, IntPtr index, string name, double val);

        // ==========================================
        // Advanced Search (Index_c.h)
        // ==========================================

        /// <summary>
        /// Searches for the k nearest neighbors and also returns the reconstructed vectors.
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int faiss_Index_search_and_reconstruct(IntPtr index, long n, [In] float[] x, long k, [Out] float[] distances, [Out] long[] labels, [Out] float[] recons);

        /// <summary>
        /// Computes the residual vector (difference between vector and its reconstruction).
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int faiss_Index_compute_residual(IntPtr index, [In] float[] x, [Out] float[] residual, long key);

        /// <summary>
        /// Merges the contents of another index into this one, consuming the other index in the process.
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int faiss_Index_merge_from(IntPtr index, IntPtr otherIndex, long add_id);

        // ==========================================
        // K-Means Clustering (Clustering_c.h)
        // ==========================================

        [StructLayout(LayoutKind.Sequential)]
        public struct FaissClusteringParameters
        {
            public int niter;
            public int nredo;
            public int spherical;
            public int int_centroids;
            public int update_index;
            public int frozen_centroids;
            public int min_points_per_centroid;
            public int max_points_per_centroid;
            public int seed;
            public ulong decode_block_size;
        }

        /// <summary>
        /// Initializes ClusteringParameters with default values.
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void faiss_ClusteringParameters_init(out FaissClusteringParameters p);

        /// <summary>
        /// Creates a new clustering object.
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int faiss_Clustering_new(out IntPtr p_clustering, int d, int k);

        /// <summary>
        /// Creates a new clustering object with specific parameters.
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int faiss_Clustering_new_with_params(out IntPtr p_clustering, int d, int k, [In] ref FaissClusteringParameters cp);

        /// <summary>
        /// Trains the clustering object using the provided dataset and index.
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int faiss_Clustering_train(IntPtr clustering, long n, [In] float[] x, IntPtr index);

        /// <summary>
        /// Frees the memory associated with a clustering object.
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void faiss_Clustering_free(IntPtr clustering);

        // ==========================================
        // Index Shards (IndexShards_c.h)
        // ==========================================

        /// <summary>
        /// Creates a meta-index that shards queries across multiple sub-indexes.
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int faiss_IndexShards_new(out IntPtr p_index, int d);

        /// <summary>
        /// Creates an IndexShards object utilizing a specific threading model.
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int faiss_IndexShards_new_with_options(out IntPtr p_index, int d, int threaded, int successive_ids);

        /// <summary>
        /// Adds a sub-index (shard) to the IndexShards collection.
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int faiss_IndexShards_add_shard(IntPtr index, IntPtr shard);

        /// <summary>
        /// Gets a pointer to a specific shard within the IndexShards collection.
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int faiss_IndexShards_at(IntPtr index, int i, out IntPtr shard);

        // ==========================================
        // Standalone Codec (Index_c.h)
        // ==========================================

        /// <summary>
        /// Encodes vectors into compact codes (used for Quantizers).
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int faiss_Index_sa_encode(IntPtr index, long n, [In] float[] x, [Out] byte[] bytes);

        /// <summary>
        /// Decodes compact codes back into approximate float vectors.
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int faiss_Index_sa_decode(IntPtr index, long n, [In] byte[] bytes, [Out] float[] x);

        // ==========================================
        // Extended File IO (index_io_c.h)
        // ==========================================

        /// <summary>
        /// Casts a base index to an IVF index safely, returning null if it is not an IVF index.
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr faiss_IndexIVF_cast(IntPtr index);

        /// <summary>
        /// Gets the nprobe setting from an IVF index pointer.
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int faiss_IndexIVF_get_nprobe(IntPtr index, out long nprobe);

        /// <summary>
        /// Sets the nprobe setting on an IVF index pointer.
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int faiss_IndexIVF_set_nprobe(IntPtr index, long nprobe);

        // ==========================================
        // Range Search (AuxIndexStructures_c.h & Index_c.h)
        // ==========================================

        /// <summary>
        /// Creates a new RangeSearchResult object to hold the dynamic results of a range search.
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int faiss_RangeSearchResult_new(out IntPtr p_rsr, long nq);

        /// <summary>
        /// Frees the memory associated with a RangeSearchResult object.
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void faiss_RangeSearchResult_free(IntPtr rsr);

        /// <summary>
        /// Retrieves the number of queries (nq) executed in the range search result.
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern long faiss_RangeSearchResult_nq(IntPtr rsr);

        /// <summary>
        /// Retrieves a pointer to the limits array (size nq + 1) which defines the slice of results for each query.
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void faiss_RangeSearchResult_lims(IntPtr rsr, out IntPtr lims);

        /// <summary>
        /// Retrieves a pointer to the labels array containing the matched vector IDs.
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void faiss_RangeSearchResult_labels(IntPtr rsr, out IntPtr labels);

        /// <summary>
        /// Retrieves a pointer to the distances array containing the distances of the matched vectors.
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void faiss_RangeSearchResult_distances(IntPtr rsr, out IntPtr distances);

        /// <summary>
        /// Performs a range search, finding all vectors within a specified radius of the query vectors.
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int faiss_Index_range_search(IntPtr index, long n, [In] float[] x, float radius, IntPtr result);

        // ==========================================
        // GPU Resources & Interop (StandardGpuResources_c.h & gpu_index_c.h)
        // ==========================================

        /// <summary>
        /// Allocates a new StandardGpuResources object, required for any GPU FAISS operations.
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int faiss_StandardGpuResources_new(out IntPtr p_res);

        /// <summary>
        /// Frees the StandardGpuResources object.
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void faiss_StandardGpuResources_free(IntPtr res);

        /// <summary>
        /// Configures the temporary memory size allocated on the GPU for FAISS operations.
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int faiss_StandardGpuResources_setTempMemory(IntPtr res, ulong size);

        /// <summary>
        /// Clones a CPU index to a single GPU device.
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int faiss_index_cpu_to_gpu(IntPtr res, int device, IntPtr index, out IntPtr p_out);

        /// <summary>
        /// Clones a GPU index back to system RAM as a CPU index.
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int faiss_index_gpu_to_cpu(IntPtr index_gpu, out IntPtr p_out);

        /// <summary>
        /// Shards or replicates a CPU index across multiple GPUs.
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int faiss_index_cpu_to_gpu_multiple(IntPtr[] res, int[] devices, int num_devices, IntPtr index, out IntPtr p_out);

        // ==========================================
        // Index Refinement & Pre-Transforms (IndexRefine_c.h & VectorTransform_c.h)
        // ==========================================

        /// <summary>
        /// Creates a RefineFlat index that reranks results from a base index using exact distances.
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int faiss_IndexRefineFlat_new(out IntPtr p_index, IntPtr base_index);

        /// <summary>
        /// Creates a new IndexPreTransform, which applies transformations (like PCA) before indexing.
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int faiss_IndexPreTransform_new(out IntPtr p_index, IntPtr underlying_index);

        /// <summary>
        /// Prepends a vector transformation (like PCA or OPQ) to an IndexPreTransform.
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int faiss_IndexPreTransform_prepend_transform(IntPtr index, IntPtr transform);

        /// <summary>
        /// Creates a Principal Component Analysis (PCA) matrix for dimensionality reduction.
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int faiss_PCAMatrix_new(out IntPtr p_transform, int d_in, int d_out, int eigen_power, int random_rotation);

        /// <summary>
        /// Frees a VectorTransform object (such as a PCA Matrix).
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void faiss_VectorTransform_free(IntPtr transform);

        // ==========================================
        // Vector Transformations (VectorTransform_c.h)
        // ==========================================

        /// <summary>
        /// Creates a normalization transform that scales vectors to a specific L2 norm (default is 1.0, creating Cosine similarity on L2 indexes).
        /// </summary>
        /// <param name="p_transform">Outputs the pointer to the newly allocated Normalization transform.</param>
        /// <param name="d">The dimensionality of the vectors.</param>
        /// <param name="norm">The target L2 norm (typically 1.0).</param>
        /// <returns>0 on success, or a non-zero error code.</returns>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int faiss_NormalizationTransform_new(out IntPtr p_transform, int d, float norm);

        // ==========================================
        // Binary Indexes (IndexBinary_c.h & IndexBinaryFlat_c.h)
        // ==========================================

        /// <summary>
        /// Frees the memory associated with a binary Faiss index.
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void faiss_IndexBinary_free(IntPtr index);

        /// <summary>
        /// Gets the total number of binary vectors currently indexed.
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern long faiss_IndexBinary_ntotal(IntPtr index);

        /// <summary>
        /// Returns whether the binary index is trained (1 = true, 0 = false).
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int faiss_IndexBinary_is_trained(IntPtr index);

        /// <summary>
        /// Adds n binary vectors of dimension d to the index. Note: x is a byte array.
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int faiss_IndexBinary_add(IntPtr index, long n, [In] byte[] x);

        /// <summary>
        /// Searches for the k nearest neighbors of the n query vectors using Hamming distance.
        /// Distances are returned as integers (number of differing bits).
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int faiss_IndexBinary_search(IntPtr index, long n, [In] byte[] x, long k, [Out] int[] distances, [Out] long[] labels);

        /// <summary>
        /// Creates a new exact Flat binary index (brute-force Hamming distance search).
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int faiss_IndexBinaryFlat_new(out IntPtr p_index, int d);

        /// <summary>
        /// Creates a new Inverted File (IVF) binary index for approximate Hamming search.
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int faiss_IndexBinaryIVF_new(out IntPtr p_index, IntPtr quantizer, int d, int nlist);
    }
}