using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FileSearch
{
    public sealed class SearchTargetEqualityComparer : IEqualityComparer<SearchTarget>
    {
        private string TargetString;

        public SearchTargetEqualityComparer(string s)
        {
            TargetString = s;
        }

        public bool Equals(SearchTarget first, SearchTarget second)
        {
            if (first == null || second == null) return false;
            return first.Encoding.GetBytes(TargetString).SequenceEqual(second.Encoding.GetBytes(TargetString));
        }

        //https://stackoverflow.com/questions/7244699/gethashcode-on-byte-array
        public int GetHashCode(SearchTarget e)
        {
            unchecked
            {
                if (e == null) return 0;
                int hash = 17;
                byte[] array = e.Encoding.GetBytes(TargetString);
                foreach (byte element in array)
                {
                    hash = hash * 31 + element.GetHashCode();
                }
                return hash;
            }
        }
    }

    public class SearchTarget
    {
        public SearchTarget(string s, Encoding e)
        {
            Encoding = e;
            Bytes = e.GetBytes(s);
        }

        public void CreateAlgorithm()
        {
            Algorithm = new KnuthMorrisPratt(Bytes);
        }

        public SearchResult Search(byte[] haystack)
        {
            return new SearchResult(this, Algorithm.Search(haystack));
        }

        public Encoding Encoding { get; }
        public byte[] Bytes { get; }
        private KnuthMorrisPratt Algorithm;
    }

    public struct SearchResult
    {
        public SearchResult(SearchTarget target, int res)
        {
            Target = target;
            Result = res;
        }

        public override string ToString()
        {
            return string.Format("{0}: {1}", Target.Encoding.EncodingName, Result);
        }

        public SearchTarget Target { get; }
        public int Result { get; }
    }

    public class SearchFile
    {
        public SearchFile(string path)
        {
            Path = path;
        }

        public void Search(SearchTarget[] targets)
        {
            byte[] text = File.ReadAllBytes(Path);
            Results = targets.Select(x => x.Search(text)).Where(x => x.Result > -1);
        }

        public override string ToString()
        {
            return string.Format("{0} === {1}", Path, string.Join(", ", Results.Select(x => x.ToString())));
        }

        public string Path { get; }
        public IEnumerable<SearchResult> Results { get; private set; }
    }
}
