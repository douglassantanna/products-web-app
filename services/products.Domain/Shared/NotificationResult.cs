namespace products.Domain.Shared;

    public record NotificationResult(string Message,bool Success, object? Data = null);
   