using Newtonsoft.Json;

public class StoreProfileRequest
{
    /// <summary>
    /// �f�B�X�v���C�l�[���̃v���p�e�B
    /// </summary>
    [JsonProperty("name")]
    public string Name { get; set; }

    /// <summary>
    /// �A�C�R�����̃v���p�e�B
    /// </summary>
    [JsonProperty("icon_name")]
    public string IconName { get; set; }
}
