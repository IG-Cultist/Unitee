using Newtonsoft.Json;

public class RivalResponse 
{
    /// <summary>
    /// ���[�UID�̃v���p�e�B
    /// </summary>
    [JsonProperty("user_id")]
    public int UserID { get; set; }

    /// <summary>
    /// �J�[�hID�̃v���p�e�B
    /// </summary>
    [JsonProperty("card_id")]
    public int CardID { get; set; }
}
