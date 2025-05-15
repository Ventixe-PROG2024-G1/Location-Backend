namespace WebApi.Models;

public class DataResponse
{
    public bool Succeded { get; set; }
    public int? StatusCode { get; set; }
    public string? Message { get; set; }
}

public class DataResponse<T>
{
    public bool Succeded { get; set; }
    public int? StatusCode { get; set; }
    public string? Message { get; set; }
    public T? Result { get; set; }
}