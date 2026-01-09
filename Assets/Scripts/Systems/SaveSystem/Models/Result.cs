[System.Serializable]
public class Result {
    public bool Success { get; set; }
    public string ErrorMessage { get; set; }
    public static Result Ok() => new Result { Success = true };
    public static Result Fail(string error) => new Result { Success = false, ErrorMessage = error };
}