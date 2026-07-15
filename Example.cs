namespace OzzieAI.FaissSharp
{


    using System;
    using FaissSharp;


    /// <summary>
    /// Provides comprehensive, real-world examples demonstrating the capabilities of the FaissSharp library.
    /// </summary>
    public static class FaissExamples
    {
        // ==========================================
        // Utility Methods
        // ==========================================

        /// <summary>
        /// Generates a flat array of random floats to simulate vector embeddings.
        /// </summary>
        private static float[] GenerateRandomVectors(int count, int dimensions, int seed = 42)
        {
            float[] vectors = new float[count * dimensions];
            Random rand = new Random(seed);
            for (int i = 0; i < vectors.Length; i++)
            {
                vectors[i] = (float)rand.NextDouble();
            }
            return vectors;
        }

        // ==========================================
        // Example 1: Exact Brute-Force Search
        // ==========================================

        /// <summary>
        /// Demonstrates a standard exact nearest-neighbor search using Euclidean (L2) distance[cite: 20].
        /// </summary>
        public static void ExactSearchExample()
        {
            Console.WriteLine("\n--- Running Exact Search (IndexFlatL2) Example ---");
            int dimensions = 128;
            int numVectors = 10000;
            int k = 5;

            float[] database = GenerateRandomVectors(numVectors, dimensions);
            float[] query = GenerateRandomVectors(1, dimensions, seed: 99);

            // Create the brute-force index[cite: 20]
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
        }

        // ==========================================
        // Example 2: Approximate Search with IVF
        // ==========================================

        /// <summary>
        /// Demonstrates clustering vectors into Voronoi cells (Inverted File) for highly accelerated approximate searches.
        /// </summary>
        public static void IvfApproximateSearchExample()
        {
            Console.WriteLine("\n--- Running IVF Approximate Search Example ---");
            int dimensions = 128;
            int numVectors = 50000;
            long numCentroids = 100; // Partition the data into 100 clusters
            int k = 5;

            float[] database = GenerateRandomVectors(numVectors, dimensions);
            float[] query = GenerateRandomVectors(1, dimensions, seed: 101);

            // 1. Create a coarse quantizer to manage the cluster centroids[cite: 20]
            using (var quantizer = new IndexFlatL2(dimensions))
            // 2. Create the IVF index using the quantizer
            using (var ivfIndex = new IndexIVFFlat(quantizer, dimensions, numCentroids))
            {
                Console.WriteLine($"Is trained before? {ivfIndex.IsTrained}");

                // 3. Train the IVF index on the dataset to find the centroids[cite: 18]
                Console.WriteLine("Training the index (running K-Means under the hood)...");
                ivfIndex.Train(database);
                Console.WriteLine($"Is trained after? {ivfIndex.IsTrained}");

                // 4. Add the vectors
                ivfIndex.Add(database);
                Console.WriteLine($"Added {ivfIndex.NTotal} vectors to IVF Index.");

                // 5. Dynamic tuning: search more clusters to increase accuracy at the cost of speed[cite: 18]
                ivfIndex.SetParameter("nprobe", 10);

                ivfIndex.Search(query, k, out float[] distances, out long[] labels);

                for (int i = 0; i < k; i++)
                {
                    Console.WriteLine($"Rank {i + 1}: ID {labels[i]}, Distance: {distances[i]:F4}");
                }
            }
        }

        // ==========================================
        // Example 3: Graph-Based Search (HNSW)
        // ==========================================

        /// <summary>
        /// Demonstrates the HNSW index, which provides incredible speed and accuracy without requiring a training phase.
        /// </summary>
        public static void HnswSearchExample()
        {
            Console.WriteLine("\n--- Running HNSW Graph Search Example ---");
            int dimensions = 128;
            int numVectors = 20000;
            int k = 5;

            float[] database = GenerateRandomVectors(numVectors, dimensions);
            float[] query = GenerateRandomVectors(1, dimensions, seed: 202);

            // Create HNSW index with 32 bidirectional links per node
            using (var hnswIndex = new IndexHNSWFlat(dimensions, 32))
            {
                // HNSW does not require training. We can add directly.
                Console.WriteLine("Building graph...");
                hnswIndex.Add(database);
                Console.WriteLine($"Added {hnswIndex.NTotal} vectors to HNSW Graph.");

                hnswIndex.Search(query, k, out float[] distances, out long[] labels);

                for (int i = 0; i < k; i++)
                {
                    Console.WriteLine($"Rank {i + 1}: ID {labels[i]}, Distance: {distances[i]:F4}");
                }
            }
        }

        // ==========================================
        // Example 4: Custom ID Mapping
        // ==========================================

        /// <summary>
        /// Demonstrates wrapping a standard index in an IDMap to assign arbitrary database IDs to vectors[cite: 20].
        /// </summary>
        public static void CustomIdMappingExample()
        {
            Console.WriteLine("\n--- Running Custom ID Mapping Example ---");
            int dimensions = 64;
            int numVectors = 5;

            float[] database = GenerateRandomVectors(numVectors, dimensions);

            // Generate custom sparse IDs (e.g., matching a SQL database primary key)
            long[] customIds = new long[] { 1005, 2042, 3301, 8099, 9999 };

            using (var flatIndex = new IndexFlatL2(dimensions))
            using (var idMapIndex = new IndexIDMap(flatIndex)) // Wrap the flat index[cite: 20]
            {
                // Add using the specific IDs[cite: 18]
                idMapIndex.AddWithIds(database, customIds);
                Console.WriteLine("Added vectors with custom IDs: " + string.Join(", ", customIds));

                // Reconstruct a specific vector by its custom ID[cite: 18]
                float[] reconstructed = idMapIndex.Reconstruct(3301);
                Console.WriteLine($"Reconstructed vector 3301 successfully. First element: {reconstructed[0]:F4}");
            }
        }

        // ==========================================
        // Example 5: Radius Range Search
        // ==========================================

        /// <summary>
        /// Demonstrates searching for all vectors that fall within a specific distance radius[cite: 18].
        /// </summary>
        public static void RangeSearchExample()
        {
            Console.WriteLine("\n--- Running Range Search Example ---");
            int dimensions = 32;
            int numVectors = 5000;
            float radius = 4.5f; // Search radius

            float[] database = GenerateRandomVectors(numVectors, dimensions);

            // Create two queries
            float[] queries = GenerateRandomVectors(2, dimensions, seed: 77);

            using (var index = new IndexFlatL2(dimensions))
            {
                index.Add(database);

                // Execute the range search[cite: 18]
                using (RangeSearchResult result = index.RangeSearch(queries, radius)) //[cite: 20]
                {
                    result.ExtractResults(out long[] limits, out float[] distances, out long[] labels); //[cite: 20]

                    for (int q = 0; q < result.QueryCount; q++) //[cite: 20]
                    {
                        long start = limits[q];
                        long end = limits[q + 1];
                        long matchCount = end - start;

                        Console.WriteLine($"Query {q + 1} found {matchCount} vectors within radius {radius}.");
                    }
                }
            }
        }

        // ==========================================
        // Example 6: Pre-Transform Pipeline (PCA)
        // ==========================================

        /// <summary>
        /// Demonstrates reducing vector dimensionality automatically before inserting them into an index.
        /// </summary>
        public static void PreTransformPcaExample()
        {
            Console.WriteLine("\n--- Running Pre-Transform (PCA) Example ---");
            int originalDimensions = 256;
            int reducedDimensions = 64; // Reduce 256D to 64D
            int numVectors = 10000;

            float[] database = GenerateRandomVectors(numVectors, originalDimensions);

            using (var baseIndex = new IndexFlatL2(reducedDimensions))
            using (var pcaTransform = new PCAMatrix(originalDimensions, reducedDimensions))
            using (var pipelineIndex = new IndexPreTransform(baseIndex))
            {
                // Prepend the PCA transformation to the pipeline
                pipelineIndex.PrependTransform(pcaTransform);

                // The pipeline must be trained so the PCA matrix can learn the variances[cite: 18]
                Console.WriteLine($"Training PCA matrix from {originalDimensions}D down to {reducedDimensions}D...");
                pipelineIndex.Train(database);

                // Add the vectors. They are automatically transformed and reduced!
                pipelineIndex.Add(database);
                Console.WriteLine($"Successfully added {pipelineIndex.NTotal} vectors into the transformed index.");
            }
        }
    }
}