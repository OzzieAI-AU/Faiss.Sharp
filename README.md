# FAISS.Sharp

![FAISS.Sharp Overview](Images/img1.png)

**FAISS.Sharp** is a comprehensive C# .NET 10 wrapper for FAISS (Facebook AI Similarity Search), built and maintained by **OzzieAI**. It enables C# developers to harness the power of FAISS for highly efficient similarity search and clustering of dense vectors natively within the .NET ecosystem.

## Key Features

* **Exact Search**: Brute-force nearest-neighbor search using Euclidean (`IndexFlatL2`) and Inner Product (`IndexFlatIP`) distances.
* **Approximate Nearest Neighbor (ANN)**: Highly accelerated approximate searches using Inverted File Indexes (`IndexIVFFlat`) and Graph-based indexes (`IndexHNSWFlat`).
* **Quantization & Compression**: Memory-efficient indexing via Scalar Quantizers (`IndexScalarQuantizer`) and Product Quantization (`IndexPQ`).
* **GPU Acceleration**: Multi-GPU support for horizontal scaling and sharding search workloads (`FaissGpu.CloneToGpuMultiple`).
* **Vector Transformations**: Integrated pre-transform pipelines, including PCA (`PCAMatrix`) and L2 Normalization (`NormalizationTransform`).
* **Binary Indexes**: Support for Hamming distance search using binary vectors (`IndexBinaryFlat`).
* **Advanced Functionality**: Radius range search (`RangeSearch`), custom ID mapping (`IndexIDMap`), and K-Means clustering (`KMeansClustering`).

![Architecture Diagram](Images/img2.png)

## Quick Start Examples

### 1. Exact Brute-Force Search

Demonstrates a standard exact nearest-neighbor search using Euclidean (L2) distance.

```csharp
using OzzieAI.FaissSharp;
using System;

int dimensions = 128;
int numVectors = 10000;
int k = 5;

// Populate database and query arrays with your vector embeddings
float[] database = GenerateRandomVectors(numVectors, dimensions);
float[] query = GenerateRandomVectors(1, dimensions);

// Create the brute-force index
using (var index = new IndexFlatL2(dimensions))
{
    index.Add(database);
    Console.WriteLine($"Added {index.NTotal} vectors to FlatL2 Index.");

    index.Search(query, k, out float[] distances, out long[] labels);

    for (int i = 0; i < k; i++)
    {
        Console.WriteLine($"Rank {i + 1}: ID {labels[i]}, Distance: {distances[i]:F4}");
    }
}
```

### 2. Graph-Based Search (HNSW)

The HNSW index provides incredible speed and accuracy without requiring a training phase.

```csharp
using OzzieAI.FaissSharp;

int dimensions = 128;
int numVectors = 20000;
int k = 5;

float[] database = GenerateRandomVectors(numVectors, dimensions);
float[] query = GenerateRandomVectors(1, dimensions);

// Create HNSW index with 32 bidirectional links per node
using (var hnswIndex = new IndexHNSWFlat(dimensions, 32))
{
    hnswIndex.Add(database);
    Console.WriteLine($"Added {hnswIndex.NTotal} vectors to HNSW Graph.");

    hnswIndex.Search(query, k, out float[] distances, out long[] labels);
}
```

![Performance Graph](Images/img3.png)

## Support & Community

For documentation, support, and community discussions, please visit our official channels:

* **Website**: [https://www.ozzieai.com/](https://www.ozzieai.com/)
* **Support Forum**: [https://forum.ozzieai.com/](https://forum.ozzieai.com/)

## License

FAISS.Sharp is distributed freely under the **Apache License 2.0**. 
See the accompanying `LICENSE` file or visit [http://www.apache.org/licenses/LICENSE-2.0](http://www.apache.org/licenses/LICENSE-2.0) for more details.
