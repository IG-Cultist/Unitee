using Newtonsoft.Json;


public class ProfileResponse
{
    /// <summary>
    /// ���[�UID�̃v���p�e�B
    /// </summary>
    [JsonProperty("user_id")]
    public int UserID { get; set; }
}
