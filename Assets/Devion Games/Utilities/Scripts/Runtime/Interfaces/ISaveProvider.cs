
public interface ISaveProvider
{
    void DeleteKey(string key);
    /// <summary>
    /// Called after the last load operation.
    /// </summary>
    void EndLoad();
    /// <summary>
    /// Called after the last save operation.
    /// </summary>
    void EndSave();
    float GetFloat(string key);
    float GetFloat(string key, float defaultValue);
    int GetInt(string key);
    int GetInt(string key, int defaultValue);
    string GetString(string key);
    string GetString(string key, string defaultValue);
    bool HasKey(string key);
    void SetFloat(string key, float value);
    void SetInt(string key, int value);
    void SetString(string key, string value);
    /// <summary>
    /// Called before the first load operation.
    /// </summary>
    void StartLoad();
    /// <summary>
    /// Called before the first save operation.
    /// </summary>
    void StartSave();
}
