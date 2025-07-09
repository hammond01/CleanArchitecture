using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.OData;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData.ModelBuilder;
using ProductManager.Domain.Entities;

namespace ProductManager.Infrastructure.Configuration
{
    /// <summary>
    /// Configuration for OData services
    /// </summary>
    public static class ODataConfiguration
    {
        /// <summary>
        /// Add OData services to the service collection
        /// </summary>
        /// <param name="services">Service collection</param>
        /// <returns>Service collection</returns>
        public static IServiceCollection AddODataServices(this IServiceCollection services)
        {
            services.AddControllers().AddOData(options =>
            {
                options.Select().Filter().OrderBy().Count().SetMaxTop(1000);
                options.AddRouteComponents("odata", GetEdmModel());
            });

            return services;
        }

        /// <summary>
        /// Build the EDM model for OData
        /// </summary>
        /// <returns>EDM model</returns>
        public static Microsoft.OData.Edm.IEdmModel GetEdmModel()
        {
            var builder = new ODataConventionModelBuilder();

            // Configure Category entity set
            builder.EntitySet<Categories>("Categories");

            // Configure Product entity set (if needed in the future)
            builder.EntitySet<Products>("Products");

            // Configure Employee entity set (if needed in the future)
            builder.EntitySet<Employees>("Employees");

            // Configure Customer entity set (if needed in the future)
            builder.EntitySet<Customers>("Customers");

            // Configure Order entity set (if needed in the future)
            builder.EntitySet<Order>("Orders");

            // Configure OrderDetail entity set (if needed in the future)
            builder.EntitySet<OrderDetails>("OrderDetails");

            return builder.GetEdmModel();
        }
    }
}
