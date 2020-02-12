using System;
using System.Security.Cryptography;
using BenchmarkDotNet.Running;

namespace BLAKE3.Benchmarks
{
    class Program
    {
#if DEBUG
        static void Main()
        {
            var hasher = new Reference.BLAKE3();
            var data = new byte[4096];
            new Random().NextBytes(data);
            var hash = hasher.ComputeHash(data);
        }
#else
        static void Main(string[] args)
            => BenchmarkSwitcher
                .FromAssembly(typeof(Program).Assembly)
                .Run(args);
#endif
    }
}
