using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

namespace BLAKE3.Reference
{
    public class BLAKE3 : KeyedHashAlgorithm
    {
#pragma warning disable IDE1006 // Naming Styles
        private const string Blake3Dll = "blake3_c";
        public const int KeyLength = 32;
        public const int HashLength = 256 / 8;

        [DllImport(Blake3Dll)]
        private static extern IntPtr blake3_new();

        [DllImport(Blake3Dll)]
        private static unsafe extern IntPtr blake3_new_keyed(byte* data_ptr);
        private static unsafe IntPtr NewKeyed(byte[] data)
        {
            if (data.Length < KeyLength) throw new ArgumentException($"Argument must be at least {KeyLength} bytes long", nameof(data));
            fixed (byte* ptr = data) return blake3_new_keyed(ptr);
        }

        [DllImport(Blake3Dll)]
        private static extern void blake3_free(IntPtr hasher);

        [DllImport(Blake3Dll)]
        private static extern void blake3_reset(IntPtr hasher);

        [DllImport(Blake3Dll)]
        private static unsafe extern void blake3_update(IntPtr hasher, byte* ptr, ulong length);
        private static unsafe void UpdateHasher(IntPtr hasher, byte[] data, int offset = 0, int? length = null)
        {
            fixed (byte* ptr = data) blake3_update(hasher, ptr + offset, (ulong)(length ?? (data.Length - offset)));
        }

        [DllImport(Blake3Dll)]
        private static unsafe extern void blake3_finalize(IntPtr hasher, byte* out_data, ulong out_length);
        private static unsafe void FinalizeTo(IntPtr hasher, byte[] data, int offset = 0, int? length = null)
        {
            fixed (byte* ptr = data) blake3_finalize(hasher, ptr + offset, (ulong)(length ?? (data.Length - offset)));
        }
        private static byte[] Finalize(IntPtr hasher)
        {
            var data = new byte[HashLength];
            FinalizeTo(hasher, data);
            return data;
        }
#pragma warning restore IDE1006 // Naming Styles

        private IntPtr hasher;

        public override int HashSize => HashLength * 8;

        private byte[] key;
        public override byte[] Key
        {
            get => key;
            set
            {
                blake3_free(hasher);
                key = value;
                hasher = NewKeyed(value);
            }
        }

        public BLAKE3()
            => hasher = blake3_new();
        public BLAKE3(byte[] key)
        {
            this.key = key;
            hasher = NewKeyed(key);
        }

        public override void Initialize()
            => blake3_reset(hasher);

        protected override void HashCore(byte[] array, int ibStart, int cbSize)
            => UpdateHasher(hasher, array, ibStart, cbSize);

        protected override byte[] HashFinal()
            => Finalize(hasher);

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            blake3_free(hasher);
        }
    }
}
