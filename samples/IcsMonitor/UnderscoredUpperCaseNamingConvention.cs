using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace IcsMonitor
{
    public sealed class UnderscoredUpperCaseNamingConvention : INamingConvention
    {
        private UnderscoredUpperCaseNamingConvention() { }

        public string Apply(string value)
        {
            return UnderscoredNamingConvention.Instance.Apply(value).ToUpperInvariant();
        }

        public static readonly INamingConvention Instance = new UnderscoredUpperCaseNamingConvention();
    }
}
