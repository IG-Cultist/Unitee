using Newtonsoft.Json;

public class StageResponse
{
    /// <summary>
    /// �X�e�[�WID�̃v���p�e�B
    /// </summary>
    [JsonProperty("id")]
    public int StageID { get; set; }

    /// <summary>
    /// �X�e�[�W�N���A����̃v���p�e�B
    /// </summary>
    [JsonProperty("clear")]
    public int Clear { get; set; }

    /// <summary>
    /// �X�e�[�W���S�N���A����̃v���p�e�B
    /// </summary>
    [JsonProperty("perfect")]
    public int Perfect { get; set; }
}
