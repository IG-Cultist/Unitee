using Newtonsoft.Json;

public class ItemResponse 
{
    /// <summary>
    /// �A�C�e��ID�̃v���p�e�B
    /// </summary>
    [JsonProperty("id")]
    public int ItemID { get; set; }

    /// <summary>
    /// �A�C�e�����̃v���p�e�B
    /// </summary>
    [JsonProperty("name")]
    public string Name { get; set; }

    /// <summary>
    /// �A�C�e�����ʒl�̃v���p�e�B
    /// </summary>
    [JsonProperty("effect")]
    public int Effect { get; set; }

    /// <summary>
    /// �K���A�C�e�����̃v���p�e�B
    /// </summary>
    [JsonProperty("bestItem_name")]
    public string BestItemName { get; set; }

    /// <summary>
    /// �A�C�e�������̃v���p�e�B
    /// </summary>
    [JsonProperty("explain")]
    public string Explain { get; set; }
}
