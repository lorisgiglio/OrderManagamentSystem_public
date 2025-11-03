using Microsoft.Extensions.Options;
using OrderManagement.Api.Orders.Domain.Entities;
using OrderManagement.Infrastructure.Configurations;
using OrderManagement.Infrastructure.Extensions;
using OrderManagement.Infrastructure.HttpValidation;
using OrderManagement.Infrastructure.Interfaces;
using static OrderManagement.Infrastructure.HttpValidation.HttpValidation;

namespace OrderManagement.Api.Orders.Application
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
                    var repository = repositoryFactory.Create<Order>();

                    var items = await repository.GetAllWithIncludesAsync(o => o.Items);
                    var itemsDraft = items.Where(o => o.Status == CurrentStatus.Draft);

                    if (itemsDraft == null || !itemsDraft.Any())
                    {
                        _logger.LogInformation("No draft orders found to validate.");
                    }
                    else
                    {
                        var httpClientUsers = _httpClientFactory.CreateClient("UsersClient");
                        var httpClientAddresses = _httpClientFactory.CreateClient("AddressesClient");
                        var httpClientProducts = _httpClientFactory.CreateClient("ProductsClient");

                        var options = new ParallelOptions { MaxDegreeOfParallelism = 3 };

                        var validationTasks = itemsDraft.Select(o =>
                            ValidateOrderAsync(o!, _validator, httpClientUsers, httpClientAddresses, httpClientProducts, _serviceScopeFactory, stoppingToken));

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

        private async Task ValidateOrderAsync(Order item,
                                             HttpValidation validator,
                                             HttpClient httpClientUsers,
                                             HttpClient httpClientAddresses,
                                             HttpClient httpClientProducts,
                                             IServiceScopeFactory serviceScopeFactory,
                                             CancellationToken cancellationToken)
        {
            var tasks = new List<Task<ValidationStatus>>
            {
                validator.CheckIdsAsync(httpClientUsers, _endpoints.UsersCheckIdsUri, new List<int> { item.UserId }),
                validator.CheckIdsAsync(httpClientAddresses, _endpoints.AddressesCheckIdsUri, new List<int> { item.DeliveryAddressId }),
                validator.CheckIdsAsync(httpClientProducts, _endpoints.ProductsCheckIdsUri, item.Items.Select(i => i.ProductId).ToList())
            };

            var results = await Task.WhenAll(tasks);

            if (results.Any(r => r == ValidationStatus.Unavailable))
            {
                _logger.LogWarning("Validation service unavailable for Order {OrderId}. Skipping validation.", item.Id);
                return;
            }

            item.Status = results.All(r => r == ValidationStatus.Valid) ? CurrentStatus.Ready : CurrentStatus.Refused;

            using var scope = serviceScopeFactory.CreateScope();
            var repositoryFactory = scope.ServiceProvider.GetRequiredService<IRepositoryFactory>();
            var isolatedRepository = repositoryFactory.Create<Order>();

            await isolatedRepository.UpdateAsync(item);

            _logger.LogInformation("Order {OrderId} validated as {Status}", item.Id, item.Status);
        }

    }
}
