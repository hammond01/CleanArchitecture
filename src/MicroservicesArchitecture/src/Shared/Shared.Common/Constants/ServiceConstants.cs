namespace Shared.Common.Constants;

/// <summary>
/// Service names and endpoints constants
/// </summary>
public static class ServiceConstants
{
    /// <summary>
    /// Service names
    /// </summary>
    public static class ServiceNames
    {
        public const string ApiGateway = "ApiGateway";
        public const string ProductCatalogService = "ProductCatalogService";
        public const string CustomerManagementService = "CustomerManagementService";
        public const string OrderManagementService = "OrderManagementService";
        public const string InventoryManagementService = "InventoryManagementService";
        public const string ShippingLogisticsService = "ShippingLogisticsService";
        public const string IdentityAccessService = "IdentityAccessService";
    }

    /// <summary>
    /// API endpoints
    /// </summary>
    public static class ApiEndpoints
    {
        public const string Health = "/health";
        public const string Ready = "/ready";
        public const string Metrics = "/metrics";

        public static class Products
        {
            public const string Base = "/api/products";
            public const string GetById = "/api/products/{id}";
            public const string Create = "/api/products";
            public const string Update = "/api/products/{id}";
            public const string Delete = "/api/products/{id}";
        }

        public static class Customers
        {
            public const string Base = "/api/customers";
            public const string GetById = "/api/customers/{id}";
            public const string Create = "/api/customers";
            public const string Update = "/api/customers/{id}";
            public const string Delete = "/api/customers/{id}";
        }

        public static class Orders
        {
            public const string Base = "/api/orders";
            public const string GetById = "/api/orders/{id}";
            public const string Create = "/api/orders";
            public const string Update = "/api/orders/{id}";
            public const string Delete = "/api/orders/{id}";
            public const string Ship = "/api/orders/{id}/ship";
            public const string Cancel = "/api/orders/{id}/cancel";
        }
    }

    /// <summary>
    /// Message queue names
    /// </summary>
    public static class MessageQueues
    {
        public const string ProductEvents = "product-events";
        public const string CustomerEvents = "customer-events";
        public const string OrderEvents = "order-events";
        public const string InventoryEvents = "inventory-events";
        public const string ShippingEvents = "shipping-events";
    }

    /// <summary>
    /// Database connection names
    /// </summary>
    public static class DatabaseConnections
    {
        public const string ProductCatalog = "ProductCatalogConnection";
        public const string CustomerManagement = "CustomerManagementConnection";
        public const string OrderManagement = "OrderManagementConnection";
        public const string InventoryManagement = "InventoryManagementConnection";
        public const string ShippingLogistics = "ShippingLogisticsConnection";
        public const string IdentityAccess = "IdentityAccessConnection";
    }
}
