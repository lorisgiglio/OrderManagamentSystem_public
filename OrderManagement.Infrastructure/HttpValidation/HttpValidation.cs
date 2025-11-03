using Microsoft.Extensions.Logging;
using System.Net.Http.Json;

namespace OrderManagement.Infrastructure.HttpValidation
{
    public class HttpValidation(ILogger<HttpValidation> logger)
    {
        private readonly ILogger<HttpValidation> _logger = logger;

        public enum ValidationStatus
        {
            Valid,
            Invalid,
            Unavailable
        }

        public async Task<ValidationStatus> CheckIdsAsync(HttpClient httpClient, string requestUri, List<int> ids)
        {
            ArgumentNullException.ThrowIfNull(httpClient, nameof(httpClient));
            if (string.IsNullOrWhiteSpace(requestUri)) throw new ArgumentException("Request URI cannot be null or empty.", nameof(requestUri));
            if (ids == null || ids.Count == 0)
            {
                _logger.LogWarning("List of IDs to validate is null or empty.");
                return ValidationStatus.Invalid;
            }

            try
            {
                var response = await httpClient.PostAsJsonAsync(requestUri, ids);

                if (response.IsSuccessStatusCode)
                {
                    var allFound = await response.Content.ReadFromJsonAsync<bool>();
                    return allFound ? ValidationStatus.Valid : ValidationStatus.Invalid;
                }

                _logger.LogWarning("Validation service returned non-success status code: {StatusCode}", response.StatusCode);
                return ValidationStatus.Unavailable;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP request error during validation.");
                return ValidationStatus.Unavailable;
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogError(ex, "HTTP request timed out during validation.");
                return ValidationStatus.Unavailable;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during validation.");
                return ValidationStatus.Unavailable;
            }
        }
    }
}
