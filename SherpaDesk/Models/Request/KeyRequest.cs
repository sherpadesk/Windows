using SherpaDesk.Common;
using SherpaDesk.Interfaces;

namespace SherpaDesk.Models.Request
{
    public class KeyRequest : GetRequest, IPath
    {
        public KeyRequest(string key)
        {
            this.Path = "/" + key;
        }

        public KeyRequest(string symbols, string key)
        {
            this.Path = string.Concat(symbols, key);
        }

        [Details]
        public string Path { get; set; }

        public override string ToString()
        {
            return this.Path ?? string.Empty;
        }
    }
}
