// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace HaveIBeenPwned.Client.Extensions;

static class StringExtensions
{
    /// <summary>
    /// Converts the string <paramref name="value"/> into a
    /// computed <see cref="SHA1"/> equivalent.
    /// </summary>
    internal static string? ToSha1Hash(this string? value)
    {
        if (value is null or { Length: 0 })
        {
            return value;
        }

        var hash = SHA1.HashData(Encoding.UTF8.GetBytes(value));

        StringBuilder stringBuilder = new(hash.Length * 2);

        foreach (var b in hash)
        {
            stringBuilder.Append(b.ToString("x2"));
        }

        return stringBuilder.ToString();
    }

    /// <summary>
    /// Converts the string <paramref name="value"/> into a
    /// computed NTLM hash equivalent using MD4.
    /// </summary>
    internal static string? ToNtlmHash(this string? value)
    {
        if (value is null or { Length: 0 })
        {
            return value;
        }

        // NTLM uses MD4 hashing of the UTF-16LE (Unicode) representation of the password
        // Since .NET doesn't include MD4 in the standard library, we compute it using the algorithm
        var unicodeBytes = Encoding.Unicode.GetBytes(value);
        var hash = ComputeMD4Hash(unicodeBytes);

        StringBuilder stringBuilder = new(hash.Length * 2);

        foreach (var b in hash)
        {
            stringBuilder.Append(b.ToString("x2"));
        }

        return stringBuilder.ToString();
    }

    // MD4 implementation for NTLM hash computation
    // Based on RFC 1320: https://www.ietf.org/rfc/rfc1320.txt
    private static byte[] ComputeMD4Hash(byte[] input)
    {
        var md4 = new MD4();
        return md4.ComputeHash(input);
    }

    // MD4 Hash Algorithm Implementation
    private sealed class MD4
    {
        private readonly uint[] _state = new uint[4];
        private readonly uint[] _x = new uint[16];

        public byte[] ComputeHash(byte[] input)
        {
            // Initialize state
            _state[0] = 0x67452301;
            _state[1] = 0xEFCDAB89;
            _state[2] = 0x98BADCFE;
            _state[3] = 0x10325476;

            // Pad the input
            var paddedInput = PadInput(input);

            // Process each 64-byte block
            for (var i = 0; i < paddedInput.Length; i += 64)
            {
                ProcessBlock(paddedInput, i);
            }

            // Convert state to bytes
            var result = new byte[16];
            Buffer.BlockCopy(_state, 0, result, 0, 16);
            return result;
        }

        private static byte[] PadInput(byte[] input)
        {
            var length = input.Length;
            var paddingLength = (55 - length) % 64;
            if (paddingLength < 0) paddingLength += 64;

            var padded = new byte[length + paddingLength + 9];
            Array.Copy(input, padded, length);
            padded[length] = 0x80;

            // Append length in bits as 64-bit little-endian
            var bitLength = (long)length * 8;
            for (var i = 0; i < 8; i++)
            {
                padded[padded.Length - 8 + i] = (byte)(bitLength >> (i * 8));
            }

            return padded;
        }

        private void ProcessBlock(byte[] input, int offset)
        {
            // Copy block into X
            for (var i = 0; i < 16; i++)
            {
                _x[i] = BitConverter.ToUInt32(input, offset + (i * 4));
            }

            uint a = _state[0], b = _state[1], c = _state[2], d = _state[3];

            // Round 1
            a = FF(a, b, c, d, _x[0], 3);
            d = FF(d, a, b, c, _x[1], 7);
            c = FF(c, d, a, b, _x[2], 11);
            b = FF(b, c, d, a, _x[3], 19);
            a = FF(a, b, c, d, _x[4], 3);
            d = FF(d, a, b, c, _x[5], 7);
            c = FF(c, d, a, b, _x[6], 11);
            b = FF(b, c, d, a, _x[7], 19);
            a = FF(a, b, c, d, _x[8], 3);
            d = FF(d, a, b, c, _x[9], 7);
            c = FF(c, d, a, b, _x[10], 11);
            b = FF(b, c, d, a, _x[11], 19);
            a = FF(a, b, c, d, _x[12], 3);
            d = FF(d, a, b, c, _x[13], 7);
            c = FF(c, d, a, b, _x[14], 11);
            b = FF(b, c, d, a, _x[15], 19);

            // Round 2
            a = GG(a, b, c, d, _x[0], 3);
            d = GG(d, a, b, c, _x[4], 5);
            c = GG(c, d, a, b, _x[8], 9);
            b = GG(b, c, d, a, _x[12], 13);
            a = GG(a, b, c, d, _x[1], 3);
            d = GG(d, a, b, c, _x[5], 5);
            c = GG(c, d, a, b, _x[9], 9);
            b = GG(b, c, d, a, _x[13], 13);
            a = GG(a, b, c, d, _x[2], 3);
            d = GG(d, a, b, c, _x[6], 5);
            c = GG(c, d, a, b, _x[10], 9);
            b = GG(b, c, d, a, _x[14], 13);
            a = GG(a, b, c, d, _x[3], 3);
            d = GG(d, a, b, c, _x[7], 5);
            c = GG(c, d, a, b, _x[11], 9);
            b = GG(b, c, d, a, _x[15], 13);

            // Round 3
            a = HH(a, b, c, d, _x[0], 3);
            d = HH(d, a, b, c, _x[8], 9);
            c = HH(c, d, a, b, _x[4], 11);
            b = HH(b, c, d, a, _x[12], 15);
            a = HH(a, b, c, d, _x[2], 3);
            d = HH(d, a, b, c, _x[10], 9);
            c = HH(c, d, a, b, _x[6], 11);
            b = HH(b, c, d, a, _x[14], 15);
            a = HH(a, b, c, d, _x[1], 3);
            d = HH(d, a, b, c, _x[9], 9);
            c = HH(c, d, a, b, _x[5], 11);
            b = HH(b, c, d, a, _x[13], 15);
            a = HH(a, b, c, d, _x[3], 3);
            d = HH(d, a, b, c, _x[11], 9);
            c = HH(c, d, a, b, _x[7], 11);
            b = HH(b, c, d, a, _x[15], 15);

            _state[0] += a;
            _state[1] += b;
            _state[2] += c;
            _state[3] += d;
        }

        private static uint F(uint x, uint y, uint z) => (x & y) | (~x & z);
        private static uint G(uint x, uint y, uint z) => (x & y) | (x & z) | (y & z);
        private static uint H(uint x, uint y, uint z) => x ^ y ^ z;

        private static uint RotateLeft(uint value, int shift) => (value << shift) | (value >> (32 - shift));

        private static uint FF(uint a, uint b, uint c, uint d, uint x, int s) => RotateLeft(a + F(b, c, d) + x, s);
        private static uint GG(uint a, uint b, uint c, uint d, uint x, int s) => RotateLeft(a + G(b, c, d) + x + 0x5A827999, s);
        private static uint HH(uint a, uint b, uint c, uint d, uint x, int s) => RotateLeft(a + H(b, c, d) + x + 0x6ED9EBA1, s);
    }
}
