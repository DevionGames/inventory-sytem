
public interface ISaveProvider
{
    public void DeleteKey(string key);
    /// <summary>
    /// Called after the last load operation.
    /// </summary>
    public void EndLoad();
    /// <summary>
    /// Called after the last save operation.
    /// </summary>
    public void EndSave();
    public float GetFloat(string key);
    public float GetFloat(string key, float defaultValue);
    public int GetInt(string key);
    public int GetInt(string key, int defaultValue);
    public string GetString(string key);
    public string GetString(string key, string defaultValue);
    public bool HasKey(string key);
    public void SetFloat(string key, float value);
    public void SetInt(string key, int value);
    public void SetString(string key, string value);
    /// <summary>
    /// Called before the first load operation.
    /// </summary>
    public void StartLoad();
    /// <summary>
    /// Called before the first save operation.
    /// </summary>
    public void StartSave();
}
