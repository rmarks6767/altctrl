using Newtonsoft.Json;

namespace glipglop
{
    public class Data
    {
        [JsonProperty("device")]
        public string Device { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("port")]
        public string Port { get; set; }
    }
}
