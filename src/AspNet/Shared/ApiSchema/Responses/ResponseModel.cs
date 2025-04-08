namespace Shared.ApiSchema.Responses;

public class ResponseModel<TEntity>
{
    public required bool Success { get; set; }
    public TEntity? Entity { get; set; }
    public string? Message { get; set; }
    public Exception? Exception { get; set; }
}
