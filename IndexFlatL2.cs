namespace OzzieAI.FaissSharp
{

    using FaissSharp.Native;
    using Microsoft.Win32.SafeHandles;

    using System;
    using System.Runtime.InteropServices;


    /// <summary>
    /// An exact search index that uses L2 (Euclidean) distance.
    /// It performs brute-force search. It does not require training.
    /// </summary>
    public class IndexFlatL2 : FaissIndex
    {
        public IndexFlatL2(int dimensions) : base(Initialize(dimensions), dimensions)
        {
        }

        private static IntPtr Initialize(int dimensions)
        {
            int status = FaissNative.faiss_IndexFlatL2_new(out IntPtr ptr, dimensions);
            FaissException.ThrowIfError(status);
            return ptr;
        }
    }

    /// <summary>
    /// An exact search index that uses Inner Product (dot product).
    /// If the vectors are normalized, this is equivalent to Cosine Similarity.
    /// </summary>
    public class IndexFlatIP : FaissIndex
    {
        public IndexFlatIP(int dimensions) : base(Initialize(dimensions), dimensions)
        {
        }

        private static IntPtr Initialize(int dimensions)
        {
            int status = FaissNative.faiss_IndexFlatIP_new(out IntPtr ptr, dimensions);
            FaissException.ThrowIfError(status);
            return ptr;
        }
    }

    /// <summary>
    /// Represents a generic index created from a factory string or loaded from disk.
    /// </summary>
    public class GenericFaissIndex : FaissIndex
    {
        internal GenericFaissIndex(IntPtr nativePointer, int dimensions) : base(nativePointer, dimensions)
        {
        }
    }

    /// <summary>
    /// A safe handle for managing the unmanaged memory lifecycle of a native FAISS ID selector[cite: 10].
    /// Inherits from <see cref="SafeHandleZeroOrMinusOneIsInvalid"/> to guarantee that native 
    /// resources are reliably freed, preventing memory leaks even if the consumer forgets to call Dispose()[cite: 10].
    /// </summary>
    internal class IDSelectorHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IDSelectorHandle"/> class, specifying that the underlying native memory is reliably owned by this handle[cite: 10].
        /// </summary>
        public IDSelectorHandle() : base(true) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="IDSelectorHandle"/> class wrapping an existing native pointer[cite: 10].
        /// </summary>
        /// <param name="handle">The pre-existing native IntPtr to the FAISS ID selector object[cite: 10].</param>
        /// <param name="ownsHandle">true to reliably release the handle during the finalization phase; otherwise, false[cite: 10].</param>
        public IDSelectorHandle(IntPtr handle, bool ownsHandle) : base(ownsHandle)
        {
            SetHandle(handle);
        }

        /// <summary>
        /// Executes the native FAISS memory release function when the handle is disposed or finalized[cite: 10].
        /// </summary>
        /// <returns>true if the handle was released successfully; otherwise, in the event of a catastrophic failure, false[cite: 10].</returns>
        protected override bool ReleaseHandle()
        {
            if (handle != IntPtr.Zero)
            {
                // Call into the native C-API to safely free the IDSelector resources allocated in C++[cite: 10]
                FaissNative.faiss_IDSelector_free(handle);
                handle = IntPtr.Zero;
            }
            return true;
        }
    }

    /// <summary>
    /// Base class for selecting IDs in operations like RemoveIds.
    /// </summary>
    public abstract class IDSelector : IDisposable
    {
        internal IDSelectorHandle Handle { get; }

        internal IDSelector(IntPtr ptr)
        {
            if (ptr == IntPtr.Zero)
                throw new ArgumentNullException(nameof(ptr), "Failed to initialize native ID Selector.");
            Handle = new IDSelectorHandle(ptr, true);
        }

        public void Dispose()
        {
            Handle?.Dispose();
            GC.SuppressFinalize(this);
        }
    }

    /// <summary>
    /// Selects IDs that fall within a specific range.
    /// </summary>
    public class IDSelectorRange : IDSelector
    {
        public IDSelectorRange(long minId, long maxId) : base(Initialize(minId, maxId)) { }

        private static IntPtr Initialize(long minId, long maxId)
        {
            FaissException.ThrowIfError(FaissNative.faiss_IDSelectorRange_new(out IntPtr ptr, minId, maxId));
            return ptr;
        }
    }

    /// <summary>
    /// Selects specific individual IDs provided in a batch.
    /// </summary>
    public class IDSelectorBatch : IDSelector
    {
        public IDSelectorBatch(long[] ids) : base(Initialize(ids)) { }

        private static IntPtr Initialize(long[] ids)
        {
            FaissException.ThrowIfError(FaissNative.faiss_IDSelectorBatch_new(out IntPtr ptr, ids.Length, ids));
            return ptr;
        }
    }

    /// <summary>
    /// Wraps a standard Flat index to allow adding vectors with custom arbitrary IDs.
    /// By default, standard Flat indexes assign sequential IDs (0, 1, 2...). 
    /// </summary>
    public class IndexIDMap : FaissIndex
    {
        // Maintains a reference to prevent the garbage collector from finalizing the wrapped index
        private readonly FaissIndex _wrappedIndex;

        public IndexIDMap(FaissIndex indexToWrap) : base(Initialize(indexToWrap), indexToWrap.Dimensions)
        {
            _wrappedIndex = indexToWrap;
        }

        private static IntPtr Initialize(FaissIndex indexToWrap)
        {
            if (indexToWrap == null || indexToWrap.Handle.IsInvalid)
                throw new ArgumentNullException(nameof(indexToWrap));

            FaissException.ThrowIfError(FaissNative.faiss_IndexIDMap_new(out IntPtr ptr, indexToWrap.Handle.DangerousGetHandle()));
            return ptr;
        }
    }

    /// <summary>
    /// A safe handle for managing the unmanaged memory lifecycle of a native FAISS clustering object.
    /// Inherits from <see cref="SafeHandleZeroOrMinusOneIsInvalid"/> to guarantee that native 
    /// resources are freed reliably, even if the consumer forgets to call Dispose().
    /// </summary>
    internal class ClusteringHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ClusteringHandle"/> class, specifying that the handle is reliably owned.
        /// </summary>
        public ClusteringHandle() : base(true) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClusteringHandle"/> class wrapping an existing native pointer.
        /// </summary>
        /// <param name="handle">The pre-existing native IntPtr to the FAISS clustering object.</param>
        /// <param name="ownsHandle">true to reliably release the handle during the finalization phase; otherwise, false.</param>
        public ClusteringHandle(IntPtr handle, bool ownsHandle) : base(ownsHandle)
        {
            SetHandle(handle);
        }

        /// <summary>
        /// Executes the native FAISS memory release function when the handle is disposed or finalized.
        /// </summary>
        /// <returns>true if the handle was released successfully; otherwise, in the event of a catastrophic failure, false.</returns>
        protected override bool ReleaseHandle()
        {
            if (handle != IntPtr.Zero)
            {
                // Call into the native C-API to free the clustering resources
                FaissNative.faiss_Clustering_free(handle);
                handle = IntPtr.Zero;
            }
            return true;
        }
    }

    /// <summary>
    /// Implements the K-Means clustering algorithm by leveraging FAISS's highly optimized native C++ core.
    /// Clustering is commonly used as a foundational step for building quantizers (e.g., Product Quantization).
    /// </summary>
    public class KMeansClustering : IDisposable
    {
        /// <summary>
        /// The safe handle managing the unmanaged FAISS clustering instance.
        /// </summary>
        internal ClusteringHandle Handle { get; }

        /// <summary>
        /// Gets the dimensionality of the vectors being clustered.
        /// </summary>
        public int Dimensions { get; }

        /// <summary>
        /// Gets the target number of clusters (centroids) to be generated.
        /// </summary>
        public int K { get; }

        /// <summary>
        /// Initializes a new K-Means clustering algorithm configuration.
        /// </summary>
        /// <param name="dimensions">The dimensionality of the vector space.</param>
        /// <param name="k">The desired number of clusters (centroids) to form.</param>
        /// <exception cref="FaissException">Thrown if the native FAISS library fails to initialize the clustering object.</exception>
        public KMeansClustering(int dimensions, int k)
        {
            Dimensions = dimensions;
            K = k;

            // Allocate the clustering object in native memory and check for initialization errors
            int status = FaissNative.faiss_Clustering_new(out IntPtr ptr, dimensions, k);
            FaissException.ThrowIfError(status);

            Handle = new ClusteringHandle(ptr, true);
        }

        /// <summary>
        /// Trains the clustering model using the provided dataset. It calculates the centroids 
        /// utilizing the provided FAISS index for accelerated distance computations.
        /// </summary>
        /// <param name="dataset">A flattened, 1D array representing the vectors to cluster (e.g., [v1_1, v1_2, v2_1, v2_2...]).</param>
        /// <param name="indexToUse">A <see cref="FaissIndex"/> (typically an exact search index like <see cref="IndexFlatL2"/>) used to assign points to the nearest centroids during training.</param>
        /// <exception cref="ArgumentException">Thrown if the dataset array length is not a perfect multiple of the configured <see cref="Dimensions"/>.</exception>
        /// <exception cref="FaissException">Thrown if the native FAISS library encounters an error during the training process.</exception>
        public void Train(float[] dataset, FaissIndex indexToUse)
        {
            // Ensure the flat array maps perfectly to the expected vector dimensionality
            if (dataset.Length % Dimensions != 0)
            {
                throw new ArgumentException($"Dataset array length ({dataset.Length}) must be a multiple of Dimensions ({Dimensions}).");
            }

            // Calculate the exact number of vectors present in the flattened array
            long numVectors = dataset.Length / Dimensions;

            // Execute the native K-Means training routine
            int status = FaissNative.faiss_Clustering_train(
                Handle.DangerousGetHandle(),
                numVectors,
                dataset,
                indexToUse.Handle.DangerousGetHandle()
            );

            FaissException.ThrowIfError(status);
        }

        /// <summary>
        /// Safely cleans up and releases the native unmanaged resources associated with the clustering object.
        /// </summary>
        public void Dispose()
        {
            Handle?.Dispose();
            // Instruct the Garbage Collector not to call the finalizer, as resources are already freed
            GC.SuppressFinalize(this);
        }
    }

    /// <summary>
    /// A safe handle for managing the unmanaged memory lifecycle of a native FAISS range search result object.
    /// Inherits from <see cref="SafeHandleZeroOrMinusOneIsInvalid"/> to guarantee that native 
    /// resources are reliably freed, preventing memory leaks when performing radius-based searches.
    /// </summary>
    internal class RangeSearchResultHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RangeSearchResultHandle"/> class, 
        /// specifying that the underlying native memory is reliably owned by this handle.
        /// </summary>
        public RangeSearchResultHandle() : base(true) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="RangeSearchResultHandle"/> class wrapping an existing native pointer.
        /// </summary>
        /// <param name="handle">The pre-existing native IntPtr to the FAISS range search result object.</param>
        /// <param name="ownsHandle">true to reliably release the handle during the finalization phase; otherwise, false.</param>
        public RangeSearchResultHandle(IntPtr handle, bool ownsHandle) : base(ownsHandle)
        {
            SetHandle(handle);
        }

        /// <summary>
        /// Executes the native FAISS memory release function when the handle is disposed or finalized.
        /// </summary>
        /// <returns>true if the handle was released successfully; otherwise, in the event of a catastrophic failure, false.</returns>
        protected override bool ReleaseHandle()
        {
            if (handle != IntPtr.Zero)
            {
                // Call into the native C-API to safely free the RangeSearchResult resources allocated in C++
                FaissNative.faiss_RangeSearchResult_free(handle);
                handle = IntPtr.Zero;
            }
            return true;
        }
    }

    /// <summary>
    /// Represents the dynamic results of a radius-based range search.
    /// </summary>
    public class RangeSearchResult : IDisposable
    {
        internal RangeSearchResultHandle Handle { get; }

        /// <summary>
        /// The number of queries executed.
        /// </summary>
        public long QueryCount { get; }

        /// <summary>
        /// Initializes a new RangeSearchResult to hold the outputs of a range search.
        /// </summary>
        /// <param name="queryCount">The number of individual query vectors.</param>
        public RangeSearchResult(long queryCount)
        {
            QueryCount = queryCount;
            int status = FaissNative.faiss_RangeSearchResult_new(out IntPtr ptr, queryCount);
            FaissException.ThrowIfError(status);
            Handle = new RangeSearchResultHandle(ptr, true);
        }

        /// <summary>
        /// Extracts the flattened labels (IDs) and distances, and returns an array of limits mapping them to each query.
        /// </summary>
        /// <param name="limits">Output array of size (QueryCount + 1). Query 'i' results start at limits[i] and end at limits[i+1].</param>
        /// <param name="distances">Output flat array containing the distances of all matches across all queries.</param>
        /// <param name="labels">Output flat array containing the IDs of all matches across all queries.</param>
        public void ExtractResults(out long[] limits, out float[] distances, out long[] labels)
        {
            FaissNative.faiss_RangeSearchResult_lims(Handle.DangerousGetHandle(), out IntPtr limsPtr);

            // The limits array defines how many results exist per query. It is always size (QueryCount + 1).
            limits = new long[QueryCount + 1];
            Marshal.Copy(limsPtr, limits, 0, limits.Length);

            // The last element in the limits array dictates the total number of matched neighbors across ALL queries.
            long totalMatches = limits[QueryCount];

            distances = new float[totalMatches];
            labels = new long[totalMatches];

            if (totalMatches > 0)
            {
                FaissNative.faiss_RangeSearchResult_distances(Handle.DangerousGetHandle(), out IntPtr distPtr);
                FaissNative.faiss_RangeSearchResult_labels(Handle.DangerousGetHandle(), out IntPtr labelsPtr);

                Marshal.Copy(distPtr, distances, 0, (int)totalMatches);
                Marshal.Copy(labelsPtr, labels, 0, (int)totalMatches);
            }
        }

        public void Dispose()
        {
            Handle?.Dispose();
            GC.SuppressFinalize(this);
        }
    }

    /// <summary>
    /// A safe handle for managing the unmanaged memory lifecycle of a native FAISS VectorTransform.
    /// Inherits from <see cref="SafeHandleZeroOrMinusOneIsInvalid"/> to ensure that native C++ memory 
    /// is reliably freed during garbage collection or explicit disposal.
    /// </summary>
    internal class VectorTransformHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        /// <summary>
        /// Initializes a new, reliably owned instance of the <see cref="VectorTransformHandle"/> class.
        /// </summary>
        public VectorTransformHandle() : base(true) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="VectorTransformHandle"/> wrapping a pre-existing native pointer.
        /// </summary>
        /// <param name="handle">The pre-existing native IntPtr to the FAISS VectorTransform.</param>
        /// <param name="ownsHandle">true if this instance should free the memory during finalization; otherwise, false.</param>
        public VectorTransformHandle(IntPtr handle, bool ownsHandle) : base(ownsHandle)
        {
            SetHandle(handle);
        }

        /// <summary>
        /// Safely releases the unmanaged C++ memory associated with the vector transform.
        /// </summary>
        /// <returns>true if the resource was successfully released.</returns>
        protected override bool ReleaseHandle()
        {
            if (handle != IntPtr.Zero)
            {
                FaissNative.faiss_VectorTransform_free(handle);
                handle = IntPtr.Zero;
            }
            return true;
        }
    }

    /// <summary>
    /// The abstract base class representing a mathematical transformation applied to vectors 
    /// (e.g., Dimensionality Reduction, Normalization).
    /// </summary>
    public abstract class VectorTransform : IDisposable
    {
        /// <summary>
        /// Exposes the secure memory handle wrapping the native C++ transform pointer.
        /// </summary>
        internal VectorTransformHandle Handle { get; }

        /// <summary>
        /// Initializes the base VectorTransform with a provided native pointer.
        /// </summary>
        /// <param name="ptr">The native pointer acquired from the FAISS C API.</param>
        internal VectorTransform(IntPtr ptr)
        {
            if (ptr == IntPtr.Zero) throw new ArgumentNullException(nameof(ptr), "Failed to initialize native VectorTransform.");
            Handle = new VectorTransformHandle(ptr, true);
        }

        /// <summary>
        /// Safely cleans up the unmanaged transform resources.
        /// </summary>
        public void Dispose()
        {
            Handle?.Dispose();
            GC.SuppressFinalize(this);
        }
    }

    /// <summary>
    /// A vector transformation that performs Principal Component Analysis (PCA) to reduce 
    /// vector dimensionality while preserving the maximum possible variance.
    /// </summary>
    public class PCAMatrix : VectorTransform
    {
        /// <summary>
        /// Initializes a new PCA transformation matrix.
        /// </summary>
        /// <param name="inputDimensions">The initial dimensionality of the vectors before transformation.</param>
        /// <param name="outputDimensions">The desired dimensionality of the vectors after transformation.</param>
        public PCAMatrix(int inputDimensions, int outputDimensions)
            : base(Initialize(inputDimensions, outputDimensions)) { }

        /// <summary>
        /// Invokes the native C API to allocate the PCA Matrix.
        /// </summary>
        private static IntPtr Initialize(int dIn, int dOut)
        {
            int status = FaissNative.faiss_PCAMatrix_new(out IntPtr ptr, dIn, dOut, 0, 0);
            FaissException.ThrowIfError(status); //[cite: 17]
            return ptr;
        }
    }

    /// <summary>
    /// A vector transformation that scales vectors to a specific L2 norm.
    /// Applying L2 normalization to vectors allows Euclidean (L2) indexes to compute Cosine Similarity.
    /// </summary>
    public class NormalizationTransform : VectorTransform
    {
        /// <summary>
        /// Initializes a new L2 normalization transformation.
        /// </summary>
        /// <param name="dimensions">The dimensionality of the vectors being normalized.</param>
        /// <param name="norm">The target L2 norm to scale to. Defaults to 1.0f (Unit Vector).</param>
        public NormalizationTransform(int dimensions, float norm = 1.0f)
            : base(Initialize(dimensions, norm)) { }

        /// <summary>
        /// Invokes the native C API to allocate the Normalization Transform.
        /// </summary>
        private static IntPtr Initialize(int d, float norm)
        {
            int status = FaissNative.faiss_NormalizationTransform_new(out IntPtr ptr, d, norm);
            FaissException.ThrowIfError(status); //[cite: 17]
            return ptr;
        }
    }

    /// <summary>
    /// A meta-index that applies one or more <see cref="VectorTransform"/> operations to incoming 
    /// vectors before storing them or searching against a base <see cref="FaissIndex"/>[cite: 18].
    /// </summary>
    public class IndexPreTransform : FaissIndex
    {
        // Maintains a reference to prevent the Garbage Collector from prematurely finalizing the base index.
        private readonly FaissIndex _baseIndex;

        /// <summary>
        /// Initializes a new Pre-Transform pipeline wrapping a target base index.
        /// </summary>
        /// <param name="baseIndex">The underlying index that will store the transformed vectors.</param>
        public IndexPreTransform(FaissIndex baseIndex) : base(Initialize(baseIndex), baseIndex.Dimensions)
        {
            _baseIndex = baseIndex;
        }

        private static IntPtr Initialize(FaissIndex baseIndex)
        {
            if (baseIndex == null || baseIndex.Handle.IsInvalid) throw new ArgumentNullException(nameof(baseIndex));

            int status = FaissNative.faiss_IndexPreTransform_new(out IntPtr ptr, baseIndex.Handle.DangerousGetHandle());
            FaissException.ThrowIfError(status); //[cite: 17]
            return ptr;
        }

        /// <summary>
        /// Adds a vector transformation (e.g., PCA or Normalization) to the pipeline. 
        /// Transformations are applied in the order they are prepended.
        /// </summary>
        /// <param name="transform">The transform matrix/operation to apply.</param>
        public void PrependTransform(VectorTransform transform)
        {
            if (transform == null || transform.Handle.IsInvalid) throw new ArgumentNullException(nameof(transform));

            int status = FaissNative.faiss_IndexPreTransform_prepend_transform(
                Handle.DangerousGetHandle(),
                transform.Handle.DangerousGetHandle()
            );

            FaissException.ThrowIfError(status); //[cite: 17]
        }
    }

    /// <summary>
    /// An Inverted File (IVF) index that partitions vectors into Voronoi cells based on a coarse quantizer.
    /// This drastically reduces search scope, providing fast, approximate nearest neighbor retrieval.
    /// </summary>
    public class IndexIVFFlat : FaissIndex
    {
        // Prevents the GC from destroying the quantizer while the IVF index still relies on it.
        private readonly FaissIndex _quantizer;

        /// <summary>
        /// Initializes a new IVF Flat index.
        /// </summary>
        /// <param name="quantizer">A coarse exact index (like <see cref="IndexFlatL2"/>) used to define cluster centroids.</param>
        /// <param name="dimensions">The dimensionality of the indexed vectors.</param>
        /// <param name="nlist">The number of clusters (inverted lists) to create.</param>
        public IndexIVFFlat(FaissIndex quantizer, int dimensions, long nlist)
            : base(Initialize(quantizer, dimensions, nlist), dimensions)
        {
            _quantizer = quantizer;
        }

        private static IntPtr Initialize(FaissIndex quantizer, int dimensions, long nlist)
        {
            if (quantizer == null || quantizer.Handle.IsInvalid) throw new ArgumentNullException(nameof(quantizer));

            int status = FaissNative.faiss_IndexIVFFlat_new(
                out IntPtr ptr,
                quantizer.Handle.DangerousGetHandle(),
                dimensions,
                nlist
            );

            FaissException.ThrowIfError(status); //[cite: 17]
            return ptr;
        }
    }

    /// <summary>
    /// A Hierarchical Navigable Small World (HNSW) index.
    /// Represents vectors as nodes in a graph with multi-layered links. It provides incredibly fast, highly accurate approximate searches without requiring training.
    /// </summary>
    public class IndexHNSWFlat : FaissIndex
    {
        /// <summary>
        /// Initializes a new HNSW Flat index.
        /// </summary>
        /// <param name="dimensions">The dimensionality of the indexed vectors.</param>
        /// <param name="m">The number of bi-directional graph links per node. Default is usually 32.</param>
        public IndexHNSWFlat(int dimensions, int m = 32) : base(Initialize(dimensions, m), dimensions)
        {
        }

        private static IntPtr Initialize(int dimensions, int m)
        {
            int status = FaissNative.faiss_IndexHNSWFlat_new(out IntPtr ptr, dimensions, m);
            FaissException.ThrowIfError(status); //[cite: 17]
            return ptr;
        }
    }

    /// <summary>
    /// A meta-index that shards (distributes) data and queries across multiple independent sub-indexes.
    /// Excellent for multi-threaded performance and handling datasets larger than a single index can comfortably manage.
    /// </summary>
    public class IndexShards : FaissIndex
    {
        /// <summary>
        /// Initializes a new empty IndexShards meta-index.
        /// </summary>
        /// <param name="dimensions">The dimensionality of the vectors that will be added to the shards.</param>
        public IndexShards(int dimensions) : base(Initialize(dimensions), dimensions)
        {
        }

        private static IntPtr Initialize(int dimensions)
        {
            int status = FaissNative.faiss_IndexShards_new(out IntPtr ptr, dimensions);
            FaissException.ThrowIfError(status); //[cite: 17]
            return ptr;
        }

        /// <summary>
        /// Attaches a sub-index to the collection of shards.
        /// </summary>
        /// <param name="shard">The FaissIndex to append as a shard.</param>
        public void AddShard(FaissIndex shard)
        {
            if (shard == null || shard.Handle.IsInvalid) throw new ArgumentNullException(nameof(shard));

            int status = FaissNative.faiss_IndexShards_add_shard(
                Handle.DangerousGetHandle(),
                shard.Handle.DangerousGetHandle()
            );

            FaissException.ThrowIfError(status); //[cite: 17]
        }
    }

    /// <summary>
    /// A safe handle for managing the unmanaged memory lifecycle of a native FAISS Binary Index.
    /// </summary>
    internal class FaissIndexBinaryHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FaissIndexBinaryHandle"/> class.
        /// </summary>
        public FaissIndexBinaryHandle() : base(true) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="FaissIndexBinaryHandle"/> class wrapping an existing native pointer.
        /// </summary>
        public FaissIndexBinaryHandle(IntPtr preExistingHandle, bool ownsHandle) : base(ownsHandle)
        {
            SetHandle(preExistingHandle);
        }

        /// <summary>
        /// Executes the native FAISS memory release function for binary indexes.
        /// </summary>
        protected override bool ReleaseHandle()
        {
            if (handle != IntPtr.Zero)
            {
                FaissNative.faiss_IndexBinary_free(handle);
                handle = IntPtr.Zero;
            }
            return true;
        }
    }

    /// <summary>
    /// The abstract base class for all FAISS Binary Indexes. These indexes operate on byte streams 
    /// and calculate proximity using Hamming distance.
    /// </summary>
    public abstract class FaissIndexBinary : IDisposable
    {
        internal FaissIndexBinaryHandle Handle { get; }

        /// <summary>
        /// The dimensionality of the vectors (in bits). Note that input byte arrays must be of length (Dimensions / 8).
        /// </summary>
        public int Dimensions { get; }

        /// <summary>
        /// Internal constructor used by derived binary classes.
        /// </summary>
        internal FaissIndexBinary(IntPtr nativePointer, int dimensions)
        {
            if (nativePointer == IntPtr.Zero)
                throw new ArgumentNullException(nameof(nativePointer), "Failed to initialize native FAISS binary index.");

            Handle = new FaissIndexBinaryHandle(nativePointer, true);
            Dimensions = dimensions;
        }

        /// <summary>
        /// Gets the total number of binary vectors currently stored in the index.
        /// </summary>
        public long NTotal => FaissNative.faiss_IndexBinary_ntotal(Handle.DangerousGetHandle());

        /// <summary>
        /// Indicates whether the binary index requires training.
        /// </summary>
        public bool IsTrained => FaissNative.faiss_IndexBinary_is_trained(Handle.DangerousGetHandle()) != 0;

        /// <summary>
        /// Adds an array of binary vectors to the index.
        /// </summary>
        /// <param name="vectors">A flat byte array. Each vector consumes (Dimensions / 8) bytes.</param>
        public void Add(byte[] vectors)
        {
            int bytesPerVector = Dimensions / 8;
            if (vectors.Length % bytesPerVector != 0)
                throw new ArgumentException("The length of the byte array must be a multiple of (Dimensions / 8).");

            long n = vectors.Length / bytesPerVector;
            int status = FaissNative.faiss_IndexBinary_add(Handle.DangerousGetHandle(), n, vectors);
            FaissException.ThrowIfError(status);
        }

        /// <summary>
        /// Searches the index for the 'k' nearest neighbors using Hamming distance.
        /// </summary>
        /// <param name="queries">A flat byte array of query vectors.</param>
        /// <param name="k">The number of nearest neighbors to retrieve per query.</param>
        /// <param name="distances">Output array containing the Hamming distances (number of differing bits).</param>
        /// <param name="labels">Output array containing the IDs of the nearest neighbors.</param>
        public void Search(byte[] queries, int k, out int[] distances, out long[] labels)
        {
            int bytesPerVector = Dimensions / 8;
            if (queries.Length % bytesPerVector != 0)
                throw new ArgumentException("The length of the queries array must be a multiple of (Dimensions / 8).");

            long n = queries.Length / bytesPerVector;
            distances = new int[n * k];
            labels = new long[n * k];

            int status = FaissNative.faiss_IndexBinary_search(Handle.DangerousGetHandle(), n, queries, k, distances, labels);
            FaissException.ThrowIfError(status);
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

    /// <summary>
    /// An exact search index that computes brute-force Hamming distances on binary vectors.
    /// Highly useful for hashing or binary fingerprint matching.
    /// </summary>
    public class IndexBinaryFlat : FaissIndexBinary
    {
        /// <summary>
        /// Initializes a new brute-force binary index.
        /// </summary>
        /// <param name="dimensions">The number of bits per vector (must be a multiple of 8).</param>
        public IndexBinaryFlat(int dimensions) : base(Initialize(dimensions), dimensions)
        {
        }

        private static IntPtr Initialize(int dimensions)
        {
            int status = FaissNative.faiss_IndexBinaryFlat_new(out IntPtr ptr, dimensions);
            FaissException.ThrowIfError(status);
            return ptr;
        }
    }
}