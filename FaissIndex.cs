namespace OzzieAI.FaissSharp
{

    using FaissSharp.Native;
    using OzzieAI.FaissSharp.Native;
    using System;


    /// <summary>
    /// The base class for all FAISS Indexes. It manages the native handle and provides
    /// the primary vector search and management capabilities.
    /// </summary>
    public abstract class FaissIndex : IDisposable
    {

        internal FaissIndexHandle Handle { get; }

        /// <summary>
        /// The dimensionality of the vectors this index was configured to accept.
        /// </summary>
        public int Dimensions { get; }

        /// <summary>
        /// Internal constructor used by derived classes to set the native handle.
        /// </summary>
        internal FaissIndex(IntPtr nativePointer, int dimensions)
        {
            if (nativePointer == IntPtr.Zero)
                throw new ArgumentNullException(nameof(nativePointer), "Failed to initialize native FAISS index.");

            Handle = new FaissIndexHandle(nativePointer, true);
            Dimensions = dimensions;
        }

        /// <summary>
        /// Gets the total number of vectors currently stored in the index.
        /// </summary>
        public long NTotal => FaissNative.faiss_Index_ntotal(Handle.DangerousGetHandle());

        /// <summary>
        /// Indicates whether the index requires training or has already been trained.
        /// Flat indexes are always trained. IVF indexes require training.
        /// </summary>
        public bool IsTrained => FaissNative.faiss_Index_is_trained(Handle.DangerousGetHandle()) != 0;

        /// <summary>
        /// Adds an array of vectors to the index. The array must be flattened (1D).
        /// </summary>
        /// <param name="vectors">A flat array representing n vectors of length 'Dimensions'.</param>
        public void Add(float[] vectors)
        {
            if (vectors.Length % Dimensions != 0)
                throw new ArgumentException("The length of the vectors array must be a multiple of the index dimensions.");

            long n = vectors.Length / Dimensions;
            int status = FaissNative.faiss_Index_add(Handle.DangerousGetHandle(), n, vectors);
            FaissException.ThrowIfError(status);
        }

        /// <summary>
        /// Trains the index using a set of representative vectors. 
        /// Only required for clustering-based indexes (like IVF or PQ).
        /// </summary>
        public void Train(float[] vectors)
        {
            if (vectors.Length % Dimensions != 0)
                throw new ArgumentException("The length of the vectors array must be a multiple of the index dimensions.");

            long n = vectors.Length / Dimensions;
            int status = FaissNative.faiss_Index_train(Handle.DangerousGetHandle(), n, vectors);
            FaissException.ThrowIfError(status);
        }

        /// <summary>
        /// Searches the index for the 'k' nearest neighbors of the provided query vectors.
        /// </summary>
        /// <param name="queries">A flat array of query vectors.</param>
        /// <param name="k">The number of nearest neighbors to retrieve per query.</param>
        /// <param name="distances">Output array containing the distances to the nearest neighbors.</param>
        /// <param name="labels">Output array containing the IDs (labels) of the nearest neighbors.</param>
        public void Search(float[] queries, int k, out float[] distances, out long[] labels)
        {
            if (queries.Length % Dimensions != 0)
                throw new ArgumentException("The length of the queries array must be a multiple of the index dimensions.");

            long n = queries.Length / Dimensions;

            // Allocate output arrays. Size is (number of queries) * (neighbors requested)
            distances = new float[n * k];
            labels = new long[n * k];

            int status = FaissNative.faiss_Index_search(Handle.DangerousGetHandle(), n, queries, k, distances, labels);
            FaissException.ThrowIfError(status);
        }

        /// <summary>
        /// Removes all vectors from the index.
        /// </summary>
        public void Reset()
        {
            int status = FaissNative.faiss_Index_reset(Handle.DangerousGetHandle());
            FaissException.ThrowIfError(status);
        }

        /// <summary>
        /// Retrieves the metric type utilized by this index.
        /// </summary>
        public FaissMetricType MetricType => (FaissMetricType)FaissNative.faiss_Index_metric_type(Handle.DangerousGetHandle());

        /// <summary>
        /// Adds an array of vectors to the index and assigns them specific IDs.
        /// Note: This is only natively supported by certain index types (like IDMap or IVF).
        /// </summary>
        public void AddWithIds(float[] vectors, long[] ids)
        {
            if (vectors.Length % Dimensions != 0)
                throw new ArgumentException("Vectors array length must be a multiple of Dimensions.");

            long n = vectors.Length / Dimensions;

            if (ids.Length != n)
                throw new ArgumentException("The number of IDs must match the number of vectors.");

            int status = FaissNative.faiss_Index_add_with_ids(Handle.DangerousGetHandle(), n, vectors, ids);
            FaissException.ThrowIfError(status);
        }

        /// <summary>
        /// Reconstructs a single vector based on its internal ID or mapped ID.
        /// </summary>
        public float[] Reconstruct(long key)
        {
            float[] vector = new float[Dimensions];
            int status = FaissNative.faiss_Index_reconstruct(Handle.DangerousGetHandle(), key, vector);
            FaissException.ThrowIfError(status);
            return vector;
        }

        /// <summary>
        /// Reconstructs a sequence of vectors based on their sequential index IDs.
        /// </summary>
        public float[] ReconstructN(long startKey, long count)
        {
            float[] vectors = new float[Dimensions * count];
            int status = FaissNative.faiss_Index_reconstruct_n(Handle.DangerousGetHandle(), startKey, count, vectors);
            FaissException.ThrowIfError(status);
            return vectors;
        }

        /// <summary>
        /// Removes vectors based on an ID Selector.
        /// </summary>
        public long RemoveIds(IDSelector selector)
        {
            if (selector == null || selector.Handle.IsInvalid)
                throw new ArgumentNullException(nameof(selector));

            int status = FaissNative.faiss_Index_remove_ids(Handle.DangerousGetHandle(), selector.Handle.DangerousGetHandle(), out long removedCount);
            FaissException.ThrowIfError(status);
            return removedCount;
        }

        /// <summary>
        /// Creates a deep copy (clone) of the current index.
        /// </summary>
        public FaissIndex Clone()
        {
            int status = FaissNative.faiss_clone_index(Handle.DangerousGetHandle(), out IntPtr clonePtr);
            FaissException.ThrowIfError(status);
            return new GenericFaissIndex(clonePtr, Dimensions);
        }

        /// <summary>
        /// Searches the index and returns the reconstructed nearest neighbor vectors.
        /// Useful when you don't store vectors locally and need the approximate vectors back.
        /// </summary>
        public void SearchAndReconstruct(float[] queries, int k, out float[] distances, out long[] labels, out float[] reconstructedVectors)
        {
            if (queries.Length % Dimensions != 0)
                throw new ArgumentException("Queries array length must be a multiple of Dimensions.");

            long n = queries.Length / Dimensions;
            distances = new float[n * k];
            labels = new long[n * k];
            reconstructedVectors = new float[n * k * Dimensions];

            int status = FaissNative.faiss_Index_search_and_reconstruct(Handle.DangerousGetHandle(), n, queries, k, distances, labels, reconstructedVectors);
            FaissException.ThrowIfError(status);
        }

        /// <summary>
        /// Empties the provided 'otherIndex' and moves all its data into this index.
        /// </summary>
        public void MergeFrom(FaissIndex otherIndex, long addIdOffset = 0)
        {
            if (otherIndex == null || otherIndex.Handle.IsInvalid)
                throw new ArgumentNullException(nameof(otherIndex));

            int status = FaissNative.faiss_Index_merge_from(Handle.DangerousGetHandle(), otherIndex.Handle.DangerousGetHandle(), addIdOffset);
            FaissException.ThrowIfError(status);
        }

        /// <summary>
        /// Safely sets a dynamic parameter (like "nprobe" for IVF) using the Faiss ParameterSpace.
        /// </summary>
        public void SetParameter(string parameterName, double value)
        {
            FaissNative.faiss_ParameterSpace_new(out IntPtr pSpace);
            try
            {
                int status = FaissNative.faiss_ParameterSpace_set_index_parameter(pSpace, Handle.DangerousGetHandle(), parameterName, value);
                FaissException.ThrowIfError(status);
            }
            finally
            {
                if (pSpace != IntPtr.Zero)
                    FaissNative.faiss_ParameterSpace_free(pSpace);
            }
        }

        /// <summary>
        /// Searches the index for all vectors that fall within a specific distance (radius) of the query vectors.
        /// Note: Range search is not supported natively by all index types (e.g., HNSW).
        /// </summary>
        /// <param name="queries">A flat array of query vectors.</param>
        /// <param name="radius">The search radius. In L2 distance, smaller is closer. In InnerProduct, larger is closer.</param>
        /// <returns>A structured result mapping matches to their respective queries.</returns>
        public RangeSearchResult RangeSearch(float[] queries, float radius)
        {
            if (queries.Length % Dimensions != 0)
                throw new ArgumentException("Queries array length must be a multiple of Dimensions.");

            long n = queries.Length / Dimensions;
            var result = new RangeSearchResult(n);

            int status = FaissNative.faiss_Index_range_search(
                Handle.DangerousGetHandle(),
                n,
                queries,
                radius,
                result.Handle.DangerousGetHandle()
            );

            FaissException.ThrowIfError(status);
            return result;
        }

        /// <summary>
        /// Cleans up the native resources.
        /// </summary>
        public void Dispose()
        {
            Handle?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}