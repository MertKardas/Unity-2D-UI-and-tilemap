[System.Serializable]
public class SaveResult {
    public bool Success { get; set; }
    public string ErrorMessage { get; set; }
    public static SaveResult Ok() => new SaveResult { Success = true };
    public static SaveResult Fail(string error) => new SaveResult { Success = false, ErrorMessage = error };
}