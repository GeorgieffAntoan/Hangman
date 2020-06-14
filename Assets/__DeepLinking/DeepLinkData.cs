using Newtonsoft.Json;

public class DeepLinkData {
    [JsonProperty("SenderAppId", NullValueHandling = NullValueHandling.Ignore)]
    public string SenderAppId { get; set; }
    [JsonProperty("MultiplayerRoomId", NullValueHandling = NullValueHandling.Ignore)]
    public string MultiplayerRoomId { get; set; }
    [JsonProperty("ExtraData", NullValueHandling = NullValueHandling.Ignore)]
    public string ExtraData { get; set; }
    [JsonProperty("DeviceId", NullValueHandling = NullValueHandling.Ignore)]
    public string DeviceId { get; set; }
}
