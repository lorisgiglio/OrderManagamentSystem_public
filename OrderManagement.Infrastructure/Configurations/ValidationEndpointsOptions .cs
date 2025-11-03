namespace OrderManagement.Infrastructure.Configurations
{
    public class ValidationEndpointsOptions
    {
        public string UsersBaseUrl { get; set; } = string.Empty;
        public string UsersCheckIdsUri { get; set; } = string.Empty;
        public string AddressesBaseUrl { get; set; } = string.Empty;
        public string AddressesCheckIdsUri { get; set; } = string.Empty;
        public string ProductsBaseUrl { get; set; } = string.Empty;
        public string ProductsCheckIdsUri { get; set; } = string.Empty;
        public string CategoriesBaseUrl { get; set; } = string.Empty;
        public string CategoriesCheckIdsUri { get; set; } = string.Empty;
    }
}
