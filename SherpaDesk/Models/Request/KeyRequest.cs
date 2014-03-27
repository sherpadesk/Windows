
namespace SherpaDesk.Models.Request
{
    public class KeyRequest : GetRequest
    {
        public KeyRequest(string key)
        {
            this.Key = key;
        }

        [Details]
        public string Key { get; set; }

        public override string ToString()
        {
            return this.Key ?? string.Empty;
        }
    }
}
