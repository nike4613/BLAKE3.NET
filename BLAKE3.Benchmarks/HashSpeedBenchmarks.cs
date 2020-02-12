using System.Security.Cryptography;
using System.Collections.Generic;
using System.Text;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Jobs;
using System.Linq;

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
    public class HashSpeedBenchmarks
    {
        private readonly SHA1 sha1 = SHA1.Create();
        private readonly SHA256 sha256 = SHA256.Create();
        private readonly SHA384 sha384 = SHA384.Create();
        private readonly SHA512 sha512 = SHA512.Create();
        private readonly MD5 md5 = MD5.Create();

        private readonly BLAKE3 blake3 = new BLAKE3();

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

        [Benchmark]
        public byte[] Sha1() => sha1.ComputeHash(data);

        [Benchmark]
        public byte[] Sha256() => sha256.ComputeHash(data);

        [Benchmark]
        public byte[] Sha384() => sha384.ComputeHash(data);

        [Benchmark]
        public byte[] Sha512() => sha512.ComputeHash(data);

        [Benchmark]
        public byte[] Md5() => md5.ComputeHash(data);

        [Benchmark]
        public byte[] Blake3() => blake3.ComputeHash(data);
    }
}
