using Newtonsoft.Json;

public class UsableCardResponse
{
    /// <summary>
    /// �g�p�\�J�[�hID�̃v���p�e�B
    /// </summary>
    [JsonProperty("id")]
    public int CardID { get; set; }

    /// <summary>
    /// �g�p�\�J�[�h���̃v���p�e�B
    /// </summary>
    [JsonProperty("name")]
    public string Name { get; set; }

    /// <summary>
    /// �g�p�\�J�[�h�����̃v���p�e�B
    /// </summary>
    [JsonProperty("stack")]
    public string Stack { get; set; }
}
