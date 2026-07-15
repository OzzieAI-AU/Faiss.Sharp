namespace OzzieAI.FaissSharp
{

    using global::OzzieAI.FaissSharp.Native;
    using Microsoft.Win32.SafeHandles;
    using OzzieAI.FaissSharp.Native;
    using System;


    /// <summary>
    /// A safe handle for managing the unmanaged memory lifecycle of a native FAISS StandardGpuResources object.
    /// Inherits from <see cref="SafeHandleZeroOrMinusOneIsInvalid"/> to guarantee that GPU allocations 
    /// and streams are safely released, preventing VRAM leaks during hardware-accelerated operations.
    /// </summary>
    internal class GpuResourcesHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GpuResourcesHandle"/> class, 
        /// specifying that the underlying GPU resource memory is reliably owned by this handle.
        /// </summary>
        public GpuResourcesHandle() : base(true) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="GpuResourcesHandle"/> class wrapping an existing native pointer.
        /// </summary>
        /// <param name="handle">The pre-existing native IntPtr to the FAISS GPU resources object.</param>
        /// <param name="ownsHandle">true to reliably release the handle during the finalization phase; otherwise, false.</param>
        public GpuResourcesHandle(IntPtr handle, bool ownsHandle) : base(ownsHandle)
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
                // Call into the native C-API to safely free the StandardGpuResources allocated in C++
                FaissNative.faiss_StandardGpuResources_free(handle);
                handle = IntPtr.Zero;
            }
            return true;
        }
    }

    /// <summary>
    /// Manages the GPU memory allocation and streams required for executing FAISS operations on Nvidia hardware.
    /// </summary>
    public class StandardGpuResources : IDisposable
    {
        internal GpuResourcesHandle Handle { get; }

        /// <summary>
        /// Initializes a new GPU resource manager.
        /// </summary>
        public StandardGpuResources()
        {
            int status = FaissNative.faiss_StandardGpuResources_new(out IntPtr ptr);
            FaissException.ThrowIfError(status);
            Handle = new GpuResourcesHandle(ptr, true);
        }

        /// <summary>
        /// Limits or expands the temporary scratch memory used on the GPU during searches.
        /// </summary>
        /// <param name="bytes">The total size in bytes allowed for temporary GPU allocations.</param>
        public void SetTempMemory(ulong bytes)
        {
            int status = FaissNative.faiss_StandardGpuResources_setTempMemory(Handle.DangerousGetHandle(), bytes);
            FaissException.ThrowIfError(status);
        }

        public void Dispose()
        {
            Handle?.Dispose();
            GC.SuppressFinalize(this);
        }
    }

    /// <summary>
    /// Static utility for migrating indexes between the CPU and GPU.
    /// </summary>
    public static class FaissGpu
    {
        /// <summary>
        /// Clones a CPU index onto the specified GPU, enabling extreme search acceleration.
        /// </summary>
        /// <param name="gpuResources">The GPU resource manager.</param>
        /// <param name="deviceId">The ID of the GPU to use (e.g., 0 for the primary GPU).</param>
        /// <param name="cpuIndex">The populated CPU index to clone.</param>
        /// <returns>A new FaissIndex residing entirely in GPU VRAM.</returns>
        public static FaissIndex CloneToGpu(StandardGpuResources gpuResources, int deviceId, FaissIndex cpuIndex)
        {
            if (gpuResources == null || gpuResources.Handle.IsInvalid) throw new ArgumentNullException(nameof(gpuResources));
            if (cpuIndex == null || cpuIndex.Handle.IsInvalid) throw new ArgumentNullException(nameof(cpuIndex));

            int status = FaissNative.faiss_index_cpu_to_gpu(
                gpuResources.Handle.DangerousGetHandle(),
                deviceId,
                cpuIndex.Handle.DangerousGetHandle(),
                out IntPtr gpuPtr
            );

            FaissException.ThrowIfError(status);
            return new GenericFaissIndex(gpuPtr, cpuIndex.Dimensions);
        }

        /// <summary>
        /// Pulls a GPU-based index back down into system RAM.
        /// </summary>
        public static FaissIndex CloneToCpu(FaissIndex gpuIndex)
        {
            if (gpuIndex == null || gpuIndex.Handle.IsInvalid) throw new ArgumentNullException(nameof(gpuIndex));

            int status = FaissNative.faiss_index_gpu_to_cpu(gpuIndex.Handle.DangerousGetHandle(), out IntPtr cpuPtr);
            FaissException.ThrowIfError(status);

            return new GenericFaissIndex(cpuPtr, gpuIndex.Dimensions);
        }
    }
}