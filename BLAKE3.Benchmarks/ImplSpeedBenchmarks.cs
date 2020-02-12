using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLAKE3.Benchmarks
{
    [SimpleJob(RuntimeMoniker.NetCoreApp22)]
    [SimpleJob(RuntimeMoniker.NetCoreApp31)]
    [SimpleJob(RuntimeMoniker.CoreRt22)]
    [SimpleJob(RuntimeMoniker.CoreRt31)]
    [SimpleJob(RuntimeMoniker.Net461)]
    [SimpleJob(RuntimeMoniker.Net472)]
    [SimpleJob(RuntimeMoniker.Mono)]
    [RPlotExporter, HtmlExporter, CsvExporter]
    public class ImplSpeedBenchmarks
    {
        private readonly Reference.BLAKE3 referenceRust = new Reference.BLAKE3();
        private readonly Naïve.BLAKE3 naïveNet = new Naïve.BLAKE3();

        private readonly BLAKE3 currentNet = new BLAKE3();

        [ParamsSource(nameof(DataGenerator))]
        public DataWrapper DataSize
        {
            get => new DataWrapper(data);
            set => data = value.Data;
        }

        private byte[] data;

        public static IEnumerable<DataWrapper> DataGenerator
            => Enumerable.Range(1, 10)
                         .Select(i => (i, size: i * 256 * 1024))
                         .Select(t => (t.i, data: new DataWrapper(t.size)))
                         .Select(t => t.data.Randomize(t.i));

        [Benchmark(Baseline = true), BenchmarkCategory("Native")]
        public byte[] Reference() => referenceRust.ComputeHash(data);

        [Benchmark, BenchmarkCategory(".NET")]
        public byte[] NaïveNet() => naïveNet.ComputeHash(data);

        [Benchmark, BenchmarkCategory(".NET")]
        public byte[] CurrentNet() => currentNet.ComputeHash(data);
    }
}
