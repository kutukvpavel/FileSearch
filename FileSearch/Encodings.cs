using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MoreLinq.Extensions;

namespace FileSearch
{
    /// <summary>
    /// Hexadecimal bytes split with spaces
    /// </summary>
    public class RawEncoding : Encoding
    {
        public static RawEncoding Instance { get; } = new RawEncoding();

        protected const char Separator = ' ';
        protected const string Prefix = "0x";

        public override string WebName
        {
            get
            {
                return "RawBytes";
            }
        }

        public override int GetByteCount(char[] chars, int index, int count)
        {
            var inputString = new string(chars).Substring(index, count).Replace(Prefix, "");
            if (inputString.Contains(Separator))
            {
                return inputString.Count(x => x == Separator)
                   - (chars.Last() == Separator ? 1 : 0)
                   - (chars.First() == Separator ? 1 : 0)
                   + 1;
            }
            else
            {
                return (int)Math.Ceiling(inputString.Length / 2d);
            }
        }

        protected byte[] DecodeHexString(char[] chars, int charIndex, int charCount, bool reverse = false)
        {
            var inputString = new string(chars).Substring(charIndex, charCount);
            inputString = inputString.Replace(Prefix, "");
            IEnumerable<IEnumerable<char>> correctedChars;
            if (!inputString.Contains(Separator))
            {
                if (inputString.Length % 2 != 0) inputString.Insert(0, "0");
                correctedChars = inputString.Batch(2).Reverse();     //Assume little-endian by default for consolidated input
            }
            else
            {
                correctedChars = inputString.Split(' ').Where(x => x.Any());  //Don't touch ordered input
            }
            var res = correctedChars.Select(x => byte.Parse(new string(x.ToArray()), System.Globalization.NumberStyles.HexNumber));
            if (reverse) res = res.Reverse();
            return res.ToArray();
        }

        public override int GetBytes(char[] chars, int charIndex, int charCount, byte[] bytes, int byteIndex)
        {
            byte[] local = DecodeHexString(chars, charIndex, charCount);
            WriteBuffer(bytes, byteIndex, local);
            return local.Length;
        }

        protected void WriteBuffer<T>(T[] bytes, int index, T[] local)
        {
            for (int i = 0; i < local.Length; i++)
            {
                bytes[i + index] = local[i];
            }
        }

        public override int GetCharCount(byte[] bytes, int index, int count)
        {
            if (index + count > bytes.Length)
            {
                count = bytes.Length - index;
            }
            return GetMaxCharCount(count);
        }

        public override int GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex)
        {
            char[] local = string.Join(" ", bytes.Skip(byteIndex).Take(byteCount).Select(x => x.ToString("X2")).ToArray()).ToCharArray();
            WriteBuffer(chars, charIndex, local);
            return local.Length;
        }

        public override int GetMaxByteCount(int charCount)
        {
            return (int)Math.Ceiling(charCount / 2d);
        }

        public override int GetMaxCharCount(int byteCount)
        {
            return (byteCount - 1) + (byteCount * 2); //Spaces + bytes with leading zeros
        }
    }

    /// <summary>
    /// Hexadecimal bytes split with spaces (reversed order)
    /// </summary>
    public class RawReversedEncoding : RawEncoding
    {
        public static new RawReversedEncoding Instance { get; } = new RawReversedEncoding();

        public override string WebName
        {
            get
            {
                return "RawBytes-Reversed";
            }
        }

        public override int GetBytes(char[] chars, int charIndex, int charCount, byte[] bytes, int byteIndex)
        {
            byte[] local = DecodeHexString(chars, charIndex, charCount, true);
            WriteBuffer(bytes, byteIndex, local);
            return local.Length;
        }

        public override int GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex)
        {
            char[] local = string.Join(" ", 
                bytes.Skip(byteIndex).Take(byteCount).Reverse().Select(x => x.ToString("X2")).ToArray()
                ).ToCharArray();
            WriteBuffer(chars, charIndex, local);
            return local.Length;
        }
    }
}
