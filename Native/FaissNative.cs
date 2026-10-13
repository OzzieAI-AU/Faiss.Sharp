namespace OzzieAI.FaissSharp.Native
{
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Direct P/Invoke bindings for the Meta Faiss C-API (faiss_c).
    /// </summary>
    internal static class FaissNative
    {
        private const string DllName = "dll/faiss.dll";

        // ==========================================
        // Error Handling (error_c.h)
        // ==========================================

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr faiss_get_last_error();

        public static string? GetLastError()
        {
            IntPtr ptr = faiss_get_last_error();
            return ptr == IntPtr.Zero ? null : Marshal.PtrToStringUTF8(ptr);
        }

        // ==========================================
        // Index Base Operations (Index_c.h)
        // ==========================================

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void faiss_Index_free(IntPtr index);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern long faiss_Index_ntotal(IntPtr index);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int faiss_Index_is_trained(IntPtr index);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int faiss_Index_add(IntPtr index, long n, [In] float[] x);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int faiss_Index_search(IntPtr index, long n, [In] float[] x, long k, [Out] float[] distances, [Out] long[] labels);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int faiss_Index_train(IntPtr index, long n, [In] float[] x);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int faiss_Index_reset(IntPtr index);

        // ==========================================
        // Flat Indexes (IndexFlat_c.h)
        // ==========================================

        [DllImport(DllName, EntryPoint = "faiss_IndexFlatL2_new_with", CallingConvention = CallingConvention.Cdecl)]
        public static extern int faiss_IndexFlatL2_new(out IntPtr index, long d);

        [DllImport(DllName, EntryPoint = "faiss_IndexFlatIP_new_with", CallingConvention = CallingConvention.Cdecl)]
        public static extern int faiss_IndexFlatIP_new(out IntPtr index, long d);

        // ==========================================
        // Index Factory (AutoTune_c.h)
        // ==========================================

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int faiss_index_factory(out IntPtr index, int d, [MarshalAs(UnmanagedType.LPUTF8Str)] string description, int metric);

        // ==========================================
        // I/O Operations (index_io_c.h)
        // ==========================================

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int faiss_write_index_fname(IntPtr index, [MarshalAs(UnmanagedType.LPUTF8Str)] string fname);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int faiss_read_index_fname([MarshalAs(UnmanagedType.LPUTF8Str)] string fname, int io_flags, out IntPtr index);

        // ==========================================
        // Index Properties (Index_c.h)
        // ==========================================

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int faiss_Index_d(IntPtr index);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int faiss_Index_metric_type(IntPtr index);

        // ==========================================
        // Advanced Index Operations (Index_c.h & clone_index_c.h)
        // ==========================================

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int faiss_Index_add_with_ids(IntPtr index, long n, [In] float[] x, [In] long[] xids);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int faiss_Index_remove_ids(IntPtr index, IntPtr sel, out long n_removed);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int faiss_Index_reconstruct(IntPtr index, long key, [Out] float[] recons);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int faiss_Index_reconstruct_n(IntPtr index, long i0, long ni, [Out] float[] recons);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int faiss_Index_assign(IntPtr index, long n, [In] float[] x, [Out] long[] labels, long k);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int faiss_clone_index(IntPtr index, out IntPtr p_out);

        // ==========================================
        // ID Selectors (AuxIndexStructures_c.h)
        // ==========================================

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void faiss_IDSelector_free(IntPtr sel);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int faiss_IDSelectorRange_new(out IntPtr p_sel, long imin, long imax);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int faiss_IDSelectorBatch_new(out IntPtr p_sel, long n, [In] long[] indices);

        // ==========================================
        // ID Maps and Complex Indexes (MetaIndexes_c.h, IndexIVF_c.h, etc.)
        // ==========================================

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int faiss_IndexIDMap_new(out IntPtr p_index, IntPtr index);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int faiss_IndexIVF_make_direct_map(IntPtr index, int new_maintain_direct_map);

        [DllImport(DllName, EntryPoint = "faiss_IndexIVFFlat_new_with", CallingConvention = CallingConvention.Cdecl)]
        public static extern int faiss_IndexIVFFlat_new(out IntPtr p_index, IntPtr quantizer, long d, long nlist);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int faiss_IndexIVFFlat_new_with_metric(out IntPtr p_index, IntPtr quantizer, long d, long nlist, int metric);

        [DllImport(DllName, EntryPoint = "faiss_IndexHNSWFlat_new_with", CallingConvention = CallingConvention.Cdecl)]
        public static extern int faiss_IndexHNSWFlat_new(out IntPtr p_index, int d, int M);

        [DllImport(DllName, EntryPoint = "faiss_IndexLSH_new_with", CallingConvention = CallingConvention.Cdecl)]
        public static extern int faiss_IndexLSH_new(out IntPtr p_index, long d, int nbits);

        [DllImport(DllName, EntryPoint = "faiss_IndexScalarQuantizer_new_with", CallingConvention = CallingConvention.Cdecl)]
        public static extern int faiss_IndexScalarQuantizer_new(out IntPtr p_index, long d, int qtype, int metric);

        // ==========================================
        // Index Parameters & AutoTuning (AutoTune_c.h)
        // ==========================================

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int faiss_ParameterSpace_new(out IntPtr p_space);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void faiss_ParameterSpace_free(IntPtr space);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int faiss_ParameterSpace_set_index_parameter(IntPtr space, IntPtr index, [MarshalAs(UnmanagedType.LPUTF8Str)] string name, double val);

        // ==========================================
        // Advanced Search (Index_c.h)
        // ==========================================

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int faiss_Index_search_and_reconstruct(IntPtr index, long n, [In] float[] x, long k, [Out] float[] distances, [Out] long[] labels, [Out] float[] recons);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int faiss_Index_compute_residual(IntPtr index, [In] float[] x, [Out] float[] residual, long key);

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
            public int verbose;
            public int spherical;
            public int int_centroids;
            public int update_index;
            public int frozen_centroids;
            public int min_points_per_centroid;
            public int max_points_per_centroid;
            public int seed;
            public UIntPtr decode_block_size;
        }

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void faiss_ClusteringParameters_init(out FaissClusteringParameters p);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int faiss_Clustering_new(out IntPtr p_clustering, int d, int k);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int faiss_Clustering_new_with_params(out IntPtr p_clustering, int d, int k, [In] ref FaissClusteringParameters cp);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int faiss_Clustering_train(IntPtr clustering, long n, [In] float[] x, IntPtr index);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void faiss_Clustering_free(IntPtr clustering);

        // ==========================================
        // Index Shards (IndexShards_c.h)
        // ==========================================

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int faiss_IndexShards_new(out IntPtr p_index, long d);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int faiss_IndexShards_new_with_options(out IntPtr p_index, long d, int threaded, int successive_ids);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int faiss_IndexShards_add_shard(IntPtr index, IntPtr shard);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int faiss_IndexShards_at(IntPtr index, int i, out IntPtr shard);

        // ==========================================
        // Standalone Codec (Index_c.h)
        // ==========================================

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int faiss_Index_sa_encode(IntPtr index, long n, [In] float[] x, [Out] byte[] bytes);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int faiss_Index_sa_decode(IntPtr index, long n, [In] byte[] bytes, [Out] float[] x);

        // ==========================================
        // Extended File IO / IVF Cast & Nprobe (IndexIVF_c.h)
        // ==========================================

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr faiss_IndexIVF_cast(IntPtr index);

        [DllImport(DllName, EntryPoint = "faiss_IndexIVF_nprobe", CallingConvention = CallingConvention.Cdecl)]
        public static extern ulong faiss_IndexIVF_get_nprobe(IntPtr index);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void faiss_IndexIVF_set_nprobe(IntPtr index, ulong nprobe);

        // ==========================================
        // Range Search (AuxIndexStructures_c.h & Index_c.h)
        // ==========================================

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int faiss_RangeSearchResult_new(out IntPtr p_rsr, long nq);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void faiss_RangeSearchResult_free(IntPtr rsr);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern ulong faiss_RangeSearchResult_nq(IntPtr rsr);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void faiss_RangeSearchResult_lims(IntPtr rsr, out IntPtr lims);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void faiss_RangeSearchResult_labels(IntPtr rsr, out IntPtr labels, out IntPtr distances);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int faiss_Index_range_search(IntPtr index, long n, [In] float[] x, float radius, IntPtr result);

        // ==========================================
        // Pre-Transforms & Vector Transforms (IndexPreTransform_c.h & VectorTransform_c.h)
        // ==========================================

        [DllImport(DllName, EntryPoint = "faiss_IndexPreTransform_new_with", CallingConvention = CallingConvention.Cdecl)]
        public static extern int faiss_IndexPreTransform_new(out IntPtr p_index, IntPtr underlying_index);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int faiss_IndexPreTransform_prepend_transform(IntPtr index, IntPtr transform);

        [DllImport(DllName, EntryPoint = "faiss_PCAMatrix_new_with", CallingConvention = CallingConvention.Cdecl)]
        public static extern int faiss_PCAMatrix_new(out IntPtr p_transform, int d_in, int d_out, float eigen_power, int random_rotation);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void faiss_VectorTransform_free(IntPtr transform);

        [DllImport(DllName, EntryPoint = "faiss_NormalizationTransform_new_with", CallingConvention = CallingConvention.Cdecl)]
        public static extern int faiss_NormalizationTransform_new(out IntPtr p_transform, int d, float norm);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void faiss_NormalizationTransform_free(IntPtr transform);

        // ==========================================
        // Binary Indexes (IndexBinary_c.h, IndexBinaryFlat_c.h, IndexBinaryIVF_c.h)
        // ==========================================

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void faiss_IndexBinary_free(IntPtr index);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern long faiss_IndexBinary_ntotal(IntPtr index);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int faiss_IndexBinary_is_trained(IntPtr index);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int faiss_IndexBinary_add(IntPtr index, long n, [In] byte[] x);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int faiss_IndexBinary_search(IntPtr index, long n, [In] byte[] x, long k, [Out] int[] distances, [Out] long[] labels);

        [DllImport(DllName, EntryPoint = "faiss_IndexBinaryFlat_new_with", CallingConvention = CallingConvention.Cdecl)]
        public static extern int faiss_IndexBinaryFlat_new(out IntPtr p_index, long d);

        [DllImport(DllName, EntryPoint = "faiss_IndexBinaryIVF_new_with", CallingConvention = CallingConvention.Cdecl)]
        public static extern int faiss_IndexBinaryIVF_new(out IntPtr p_index, IntPtr quantizer, long d, long nlist);

        // ==========================================
        // GPU Resources & Interop (StandardGpuResources_c.h & gpu_index_c.h)
        // ==========================================

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int faiss_StandardGpuResources_new(out IntPtr p_res);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void faiss_StandardGpuResources_free(IntPtr res);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int faiss_StandardGpuResources_setTempMemory(IntPtr res, ulong size);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int faiss_index_cpu_to_gpu(IntPtr res, int device, IntPtr index, out IntPtr p_out);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int faiss_index_gpu_to_cpu(IntPtr index_gpu, out IntPtr p_out);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int faiss_index_cpu_to_gpu_multiple(IntPtr[] res, int[] devices, int num_devices, IntPtr index, out IntPtr p_out);
    }
}