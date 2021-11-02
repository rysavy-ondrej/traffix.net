using Microsoft.Extensions.Configuration.Json;
using System.IO;

namespace Traffix.Storage.Faster
{
    public partial class FasterFrameTable
    {
        public sealed class Configuration
        {
            private JsonConfigurationProvider _provider;

            public Configuration(string path)
            {
                _provider = new JsonConfigurationProvider(new JsonConfigurationSource { Path = Path.GetFullPath(path) }); ;
            }
            public long FramesCapacity
            {
                get
                {
                    return _provider.TryGet(nameof(FramesCapacity), out var value) ? long.Parse(value) : 100000;
                }
                set
                {
                    _provider.Set(nameof(FramesCapacity), value.ToString());
                }
            }

            /// <summary>
            /// Saves the configration file.
            /// </summary>
            public Configuration Save()
            {
                string json = System.Text.Json.JsonSerializer.Serialize<Configuration>(this, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(_provider.Source.Path, json);
                return this;
            }

            public Configuration Load()
            {
                using var stream = File.OpenRead(_provider.Source.Path);
                _provider.Load(stream);
                return this;
            }
        }
    }
}
