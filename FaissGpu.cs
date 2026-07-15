namespace OzzieAI.FaissSharp
{

    using global::OzzieAI.FaissSharp.Native;
    using OzzieAI.FaissSharp.Native;
    using System;

    public static partial class FaissGpu
    {
        /// <summary>
        /// Clones and distributes a CPU index across multiple GPUs simultaneously. 
        /// This provides horizontal scaling for enormous datasets, sharding the search workload across hardware.
        /// </summary>
        /// <param name="gpuResourcesList">An array of initialized StandardGpuResources, one for each GPU device.</param>
        /// <param name="deviceIds">An array of physical GPU device IDs (e.g., [0, 1]). Must match the length of gpuResourcesList.</param>
        /// <param name="cpuIndex">The populated CPU index to clone and shard.</param>
        /// <returns>A new FaissIndex that routes queries to the underlying GPU cluster.</returns>
        public static FaissIndex CloneToGpuMultiple(StandardGpuResources[] gpuResourcesList, int[] deviceIds, FaissIndex cpuIndex)
        {
            if (gpuResourcesList == null || gpuResourcesList.Length == 0) throw new ArgumentNullException(nameof(gpuResourcesList));
            if (deviceIds == null || deviceIds.Length != gpuResourcesList.Length) throw new ArgumentException("Device IDs array must exactly match the length of the GPU Resources list.");
            if (cpuIndex == null || cpuIndex.Handle.IsInvalid) throw new ArgumentNullException(nameof(cpuIndex));

            // Extract the raw pointers from the safe handles
            IntPtr[] resPointers = new IntPtr[gpuResourcesList.Length];
            for (int i = 0; i < gpuResourcesList.Length; i++)
            {
                resPointers[i] = gpuResourcesList[i].Handle.DangerousGetHandle();
            }

            int status = FaissNative.faiss_index_cpu_to_gpu_multiple(
                resPointers,
                deviceIds,
                deviceIds.Length,
                cpuIndex.Handle.DangerousGetHandle(),
                out IntPtr multiGpuPtr
            );

            FaissException.ThrowIfError(status);
            return new GenericFaissIndex(multiGpuPtr, cpuIndex.Dimensions);
        }
    }
}