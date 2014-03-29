using SherpaDesk.Common;

namespace SherpaDesk.Models.Request
{
    public class KeyRequest : GetRequest, IPath
    {
        public KeyRequest(string key)
        {
            this.Path = key;
        }

        [Details]
        public string Path { get; set; }

        public override string ToString()
        {
            return this.Path ?? string.Empty;
        }
    }
}
