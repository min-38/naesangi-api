namespace naesangi.Api.Contracts;

public sealed record APIError(
    string Code,
    string? Message = null,
    IReadOnlyDictionary<string, string[]>? Details = null);


public sealed record APIResponse<T>(
    T? Data,
    APIError? Error)
{
    public bool Success => Error is null;

    public static APIResponse<T> Ok(T? data = default) => new(data, null);

    public static APIResponse<T> Fail(
        string code,
        string? message = null,
        IReadOnlyDictionary<string, string[]>? details = null)
        => new(default, new APIError(code, message, details));
}
