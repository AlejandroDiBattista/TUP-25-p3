namespace Cliente.Constants;

public static class AppConstants
{
    public const int PRODUCTOS_POR_PAGINA_DEFAULT = 6;
    public const int TOAST_DURATION_SUCCESS = 4000;
    public const int TOAST_DURATION_ERROR = 5000;
    public const int TOAST_DURATION_WARNING = 4000;
    public const int TOAST_DURATION_INFO = 3000;
    public const int HTTP_TIMEOUT_SECONDS = 30;
    public const int RETRY_ATTEMPTS = 3;
    public const int SUCCESS_ANIMATION_DURATION = 2000;
    public const int DEBOUNCE_DELAY_MS = 300;
}

public class ApiSettings
{
    public string BaseUrl { get; set; } = string.Empty;
    public int Timeout { get; set; } = AppConstants.HTTP_TIMEOUT_SECONDS;
    public int RetryAttempts { get; set; } = AppConstants.RETRY_ATTEMPTS;
}

public class AppSettings
{
    public int ProductosPorPagina { get; set; } = AppConstants.PRODUCTOS_POR_PAGINA_DEFAULT;
    public ToastDurationSettings ToastDuration { get; set; } = new();
}

public class ToastDurationSettings
{
    public int Success { get; set; } = AppConstants.TOAST_DURATION_SUCCESS;
    public int Error { get; set; } = AppConstants.TOAST_DURATION_ERROR;
    public int Warning { get; set; } = AppConstants.TOAST_DURATION_WARNING;
    public int Info { get; set; } = AppConstants.TOAST_DURATION_INFO;
}
