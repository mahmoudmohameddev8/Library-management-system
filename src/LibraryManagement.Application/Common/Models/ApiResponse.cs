namespace LibraryManagement.Application.Common.Models;

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
    public IEnumerable<string>? Errors { get; set; }
    public int StatusCode { get; set; }

    public static ApiResponse<T> Ok(T data, string message = "Success")
        => new() { Success = true, Data = data, Message = message, StatusCode = 200 };

    public static ApiResponse<T> Created(T data, string message = "Created successfully")
        => new() { Success = true, Data = data, Message = message, StatusCode = 201 };

    public static ApiResponse<T> Fail(string message, int statusCode, IEnumerable<string>? errors = null)
        => new() { Success = false, Message = message, StatusCode = statusCode, Errors = errors };
}

public class ApiResponse : ApiResponse<object>
{
    public static ApiResponse NoContent(string message = "Operation completed successfully")
        => new() { Success = true, Message = message, StatusCode = 204 };
}
