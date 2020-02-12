using System;
using System.Diagnostics.CodeAnalysis;

namespace BLAKE3.Benchmarks
{
    public struct DataWrapper 
    {
        [SuppressMessage("Performance", "CA1819:Properties should not return arrays", 
            Justification = "This is a wrapper for a byte array, and so must return an array.")]
        public byte[] Data { get; }
        public int Length => Data.Length;

        public DataWrapper(int size) : this(new byte[size]) { }
        public DataWrapper(byte[] data) => Data = data;

        public DataWrapper Randomize(int seed)
        {
            new Random(seed).NextBytes(Data);
            return this;
        }

        public override string ToString()
            => $"{Length/1024:N1}kb";
    }
}
