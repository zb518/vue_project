namespace PPE.Model.Shared;

public class RepsonResult<T> where T : new()
{
    public bool Succeeded { get; set; }
    public string? Message { get; set; }
    public T? Data { get; set; }
}