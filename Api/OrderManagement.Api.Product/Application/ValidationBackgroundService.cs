using Microsoft.Extensions.Options;
using OrderManagement.Api.Products.Domain.Entities;
using OrderManagement.Infrastructure.Configurations;
using OrderManagement.Infrastructure.Extensions;
using OrderManagement.Infrastructure.HttpValidation;
using OrderManagement.Infrastructure.Interfaces;
using static OrderManagement.Infrastructure.HttpValidation.HttpValidation;

namespace OrderManagement.Api.Products.Application
{
    public class ValidationBackgroundService(IServiceScopeFactory serviceScopeFactory,
                                            ILogger<ValidationBackgroundService> logger,
                                            IHttpClientFactory httpClientFactory,
                                            IOptions<ValidationEndpointsOptions> options,
                                            HttpValidation validator) : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory = serviceScopeFactory;
        private readonly ILogger<ValidationBackgroundService> _logger = logger;
        private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
        private readonly ValidationEndpointsOptions _endpoints = options.Value;
        private readonly HttpValidation _validator = validator;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Order Validation Background Service running.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _serviceScopeFactory.CreateScope();

                    var repositoryFactory = scope.ServiceProvider.GetRequiredService<IRepositoryFactory>();
                    var repository = repositoryFactory.Create<Product>();

                    var items = await repository.GetAllAsync();
                    var itemsDraft = items.Where(o => o.Status == CurrentStatus.Draft);

                    if (itemsDraft == null || !itemsDraft.Any())
                    {
                        _logger.LogInformation("No draft product found to validate.");
                    }
                    else
                    {
                        var httpClientCategories = _httpClientFactory.CreateClient("CategoriesClient");

                        var options = new ParallelOptions { MaxDegreeOfParallelism = 3 };

                        var validationTasks = itemsDraft.Select(o =>
                            ValidateAsync(o!, _validator, httpClientCategories, _serviceScopeFactory, stoppingToken));

                        await Task.WhenAll(validationTasks);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Errore durante la validazione degli ordini");
                }

                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }

        private async Task ValidateAsync(Product item,
                                             HttpValidation validator,
                                             HttpClient httpClientCategories,
                                             IServiceScopeFactory serviceScopeFactory,
                                             CancellationToken cancellationToken)
        {
            var tasks = new List<Task<ValidationStatus>>
            {
                validator.CheckIdsAsync(httpClientCategories, _endpoints.CategoriesCheckIdsUri, new List<int> { item.CategoryId }),
            };

            var results = await Task.WhenAll(tasks);

            if (results.Any(r => r == ValidationStatus.Unavailable))
            {
                _logger.LogWarning("Validation service unavailable for Product {ProductId}. Skipping validation.", item.Id);
                return;
            }

            item.Status = results.All(r => r == ValidationStatus.Valid) ? CurrentStatus.Ready : CurrentStatus.Refused;

            using var scope = serviceScopeFactory.CreateScope();
            var repositoryFactory = scope.ServiceProvider.GetRequiredService<IRepositoryFactory>();
            var isolatedRepository = repositoryFactory.Create<Product>();

            await isolatedRepository.UpdateAsync(item);

            _logger.LogInformation("Product {ProductId} validated as {Status}", item.Id, item.Status);
        }
    }
}
