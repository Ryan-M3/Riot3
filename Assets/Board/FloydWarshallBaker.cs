using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class FloydWarshallBaker {
    public int[,] dists;
    public Board board;
    public int maxDist = 8;
    public ArraySaveLoad savef;

    protected int width;
    protected int height;
    protected int area;
    protected const int bigNum = 256;

    public FloydWarshallBaker(Board board, ArraySaveLoad savef) {
        this.board = board;
        this.savef = savef;
        width  = board.width;
        height = board.height;
        area   = width * height;
        dists  = new int[area, area];
    }

    /// <summary>
    /// Maps a 2D point to a 1D point based on the dimensions of dists.
    /// Used to create and adjacency matrix mapping each point to every
    /// other point.
    /// </summary>
    public int DistID(Point c) {
        // Because of zero-indexing v.y * width produces an
        // area one row less than v.y * width. Thus, all we
        // need to do to fill out that last row is add v.x.
        return Mathf.RoundToInt(c.y * board.width + c.x);
    }

    /// <param name="ndiv">
    /// The number of subdivisions where ndiv=2 indicates a
    /// square matrix be subdivided into four equal parts.
    /// That is, it was folded in half twice.
    /// </param>
    /// <returns> A Vector4 with begX, begY, endX, and endY as values. </returns>
    protected IEnumerable<Vector4Int> GetSubdivisionArgs(int ndiv) {
        int len = area / ndiv;
        for (int i = 0; i < ndiv; i++) {
            for (int j = 0; j <ndiv; j++) {
                int begX = j * len;
                int begY = i * len;
                int endX = begX + len;
                int endY = begY + len;
                yield return new Vector4Int(begX, begY, endX, endY);
            }
        }
    }

    public void Calculate() {
        int CPU_Cores = 4;
        int subdiv = Mathf.RoundToInt(Mathf.Sqrt(CPU_Cores));

        var threads = GetInitThreads(subdiv);
        foreach (Thread thr in threads) {
            thr.Start();
        }

        foreach (Thread thr in threads) {
            thr.Join();
        }
        threads.Clear();

        threads = GetBlockSetAdjacentThreads(subdiv);

        foreach (Thread thr in threads) {
            thr.Start();
        }

        foreach (Thread thr in threads) {
            thr.Join();
        }
        threads.Clear();

        Thread stitchVert = new Thread(() => StitchAdjacentVertical(subdiv));
        Thread stitchHorz = new Thread(() => StitchAdjacentHorizontal(subdiv));
        stitchVert.Start();
        stitchHorz.Start();
        stitchVert.Join();
        stitchHorz.Join();

        for (int k = 1; k < maxDist; k++) {
            List<Vector4Int> blocks = GetSubdivisionArgs(subdiv).ToList();
            foreach (Vector4Int block in blocks) {
                threads.Add(new Thread(() => BlockToBlockDistances(block, block, k)));
            }

            foreach (Thread thr in threads) {
                thr.Start();
            }

            foreach (Thread thr in threads) {
                thr.Join();
            }
            threads.Clear();

            foreach (List<Vector4Int> p in Permute(blocks)) {
                for (int i = 0; i < blocks.Count; i += 2) {
                    threads.Add(new Thread(() => BlockToBlockDistances(p[i], p[i + 1], k)));
                }

                foreach (Thread thr in threads) {
                    thr.Start();
                }

                foreach (Thread thr in threads) {
                    thr.Join();
                }
                threads.Clear();
            }
        }
    }

    public IEnumerator CalculateCoroutine() {
        int CPU_Cores = 4;
        int subdiv = Mathf.RoundToInt(Mathf.Sqrt(CPU_Cores));

        Debug.Log("inititializing distance matrix.");
        yield return new WaitForSeconds(1f);
        var threads = GetInitThreads(subdiv);
        foreach (Thread thr in threads) {
            thr.Start();
        }

        foreach (Thread thr in threads) {
            thr.Join();
        }
        threads.Clear();
        Debug.Log("Dists inititialized. Setting all immediately adjacent tiles.");
        yield return new WaitForSeconds(1f);

        threads = GetBlockSetAdjacentThreads(subdiv);

        foreach (Thread thr in threads) {
            thr.Start();
        }

        foreach (Thread thr in threads) {
            thr.Join();
        }
        threads.Clear();
        Debug.Log("Most adjacent tiles set. Adding finished touches.");
        yield return new WaitForSeconds(1f);

        Thread stitchVert = new Thread(() => StitchAdjacentVertical(subdiv));
        Thread stitchHorz = new Thread(() => StitchAdjacentHorizontal(subdiv));
        stitchVert.Start();
        stitchHorz.Start();
        stitchVert.Join();
        stitchHorz.Join();
        Debug.Log("All adjacent tiles set. Beginning the meat of the algorithm.");
        yield return new WaitForSeconds(1f);

        for (int k = 1; k < maxDist; k++) {
            Debug.Log("Setting all distances " + k + " tiles away from eachother.");
            yield return new WaitForSeconds(1f);
            List<Vector4Int> blocks = GetSubdivisionArgs(subdiv).ToList();
            foreach (Vector4Int block in blocks) {
                threads.Add(new Thread(() => BlockToBlockDistances(block, block, k)));
            }

            foreach (Thread thr in threads) {
                thr.Start();
            }

            foreach (Thread thr in threads) {
                thr.Join();
            }
            threads.Clear();

            int nperms = 0;
            foreach (List<Vector4Int> p in Permute(blocks)) {
                nperms++;
                Debug.Log("\tPemuting " + nperms + " of " + blocks.Count);
                yield return new WaitForSeconds(1f);
                for (int i = 0; i < blocks.Count; i += 2) {
                    threads.Add(new Thread(() => BlockToBlockDistances(p[i], p[i + 1], k)));
                }

                foreach (Thread thr in threads) {
                    thr.Start();
                }

                foreach (Thread thr in threads) {
                    thr.Join();
                }
                threads.Clear();
            }
        }
        Debug.Log("Distances calculated. Final step of saving beginning.");
        yield return new WaitForSeconds(1f);
        savef.SaveData<int>(dists, "bakedDists");
        Debug.Log("Baking complete.");
        yield return null;
    }

    public IEnumerable<Point> PointsInBlock(Vector4Int v4) {
        int begX = v4.x;
        int begY = v4.y;
        int endX = v4.z;
        int endY = v4.w;
        for (int y = begY; y < endY; y++) {
            for (int x = begX; x < endX; x++) {
                yield return new Point(x, y);
            }
        }
    }

    protected void BlockSetAdjacent(int begX, int begY, int endX, int endY) {
        for (int y = begY; y < endY; y++) {
            for (int x = begX; x < endX; x++) {
                var cntr  = new Point(x    , y    );
                var north = new Point(x    , y + 1);
                var east  = new Point(x + 1, y    );
                var south = new Point(x    , y - 1);
                var west  = new Point(x - 1, y    );

                if (board.IsOccupied(cntr))
                    continue;

                if (board.InBounds(north) && !board.IsOccupied(north))
                    dists[DistID(cntr), DistID(north)] = 1;

                if (board.InBounds(east ) && !board.IsOccupied(east))
                    dists[DistID(cntr), DistID(east )] = 1;

                if (board.InBounds(south) && !board.IsOccupied(south))
                    dists[DistID(cntr), DistID(south)] = 1;

                if (board.InBounds(west ) && !board.IsOccupied(west ))
                    dists[DistID(cntr), DistID(west )] = 1;
            }
        }
    }

    protected void StitchAdjacentVertical(int subdiv) {
        for (int y = area / subdiv; y < area; y += area / subdiv) {
            for (int x = 0; x < area; x++) {
                Point under = new Point(x, y - 1);
                Point over  = new Point(x, y);
                dists[DistID(under), DistID(over )] = 1;
                dists[DistID(over ), DistID(under)] = 1;
            }
        }
    }

    protected void BlockToBlockDistances(Vector4Int blockA, Vector4Int blockB, int k) {
        foreach (Point a in PointsInBlock(blockA)) {
            foreach (Point b in PointsInBlock(blockB)) {
                if (!board.IsOccupied(a) && !board.IsOccupied(b)) {
                    int sum = dists[DistID(a), k] + dists[k, DistID(b)];
                    if (dists[DistID(a), DistID(b)] > sum) {
                        dists[DistID(a), DistID(b)] = sum;
                    }
                }
            }
        }
    }

    protected void StitchAdjacentHorizontal(int subdiv) {
        for (int y = 0; y < area; y++) {
            for (int x = area / subdiv; x < area; x += area / subdiv) {
                Point left  = new Point(x - 1, y);
                Point right = new Point(x, y);
                dists[DistID(left ), DistID(right)] = 1;
                dists[DistID(right), DistID(left )] = 1;
            }
        }
    }

    protected List<Thread> GetInitThreads(int subdiv) {
        List<Thread> threads = new List<Thread>();
        foreach (Vector4Int v4 in GetSubdivisionArgs(subdiv)) {
            threads.Add(new Thread(() => Init(v4.x, v4.y, v4.z, v4.w)));
        }
        return threads;
    }

    protected List<Thread> GetBlockSetAdjacentThreads(int subdiv) {
        List<Thread> threads = new List<Thread>();
        foreach (Vector4Int v4 in GetSubdivisionArgs(subdiv)) {
            threads.Add(new Thread(() => BlockSetAdjacent(v4.x, v4.y, v4.z, v4.w)));
        }
        return threads;
    }

    protected IEnumerable<List<Thread>> GetBlockToBlockThreads(int subdiv, int k) {
        List<Thread> threads = new List<Thread>();
        List<Vector4Int> blocks = GetSubdivisionArgs(subdiv).ToList();
        foreach (List<Vector4Int> permuation in Permute(blocks)) {
            for (int i = 0; i < blocks.Count / 2; i += 2) {
                Vector4Int a = blocks[i];
                Vector4Int b = blocks[i + 1];
                threads.Add(new Thread(() => BlockToBlockDistances(a, b, k)));
            }
            yield return threads;
        }
    }

    // This simple little algorithm apparently goes back
    // to Narayana Pandita in 14th Century India.
    protected IEnumerable<List<Vector4Int>> Permute(List<Vector4Int> items) {
        List<Vector4Int> original = new List<Vector4Int>();
        original.AddRange(items);
        int i = items.Count - 1;
        int j = items.Count - 2;
        do {
            yield return items;

            // Swap last two values.
            var temp = items[i];
            items[i] = items[j];
            items[j] = temp;
            yield return items;

            // reverse
            items.Reverse();
        } while (!items.SequenceEqual(original));
    }

    /// 4 threads seems to be about ideal on my 4-core machine.
    public void PrintInitThreadTimes() {
        int samples = 50;

        for (int threadCount = 1; threadCount < 10; threadCount++) {
            List<float> sampleTimes = new List<float>();

            for (int i = 0; i < samples; i++) {
                var watch = System.Diagnostics.Stopwatch.StartNew();
                var threads = GetInitThreads(threadCount * threadCount);
                foreach (Thread thr in threads) {
                    thr.Start();
                }
                foreach (Thread thr in threads) {
                    threads[0].Join();
                }
                watch.Stop();
                sampleTimes.Add(watch.ElapsedMilliseconds);
                threads.Clear();
            }

            Summarize(sampleTimes, threadCount * threadCount);
            sampleTimes.Clear();
        }
    }

    private void Summarize(List<float> sampleTimes, int threads) {
        float mean  = sampleTimes.Average();
        float sumSqrs = sampleTimes.Select(t => Mathf.Pow(mean - t, 2)).Sum();
        float n = sampleTimes.Count();
        float stdev = Mathf.Sqrt(sumSqrs / n);
        Debug.Log(string.Format("Threads: {0}, Mean: {1}, Stdev: {2}", threads, mean, stdev));
    }

    protected void Init(int begX, int begY, int endX, int endY) {
        for (int i = begY; i < endY; i++) {
            for (int j = begX; j < endX; j++) {
                if (i == j)
                    dists[i, j] = 0;
                else
                    dists[i, j] = bigNum;
            }
        }
    }
}
