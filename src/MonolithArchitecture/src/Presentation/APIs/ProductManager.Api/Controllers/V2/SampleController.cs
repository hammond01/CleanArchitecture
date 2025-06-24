using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;

namespace ProductManager.Api.Controllers.V2
{
    /// <summary>
    /// Sample controller for API version 2.0 - demonstrates enhanced versioning
    /// </summary>
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class SampleController : ControllerBase
    {
        /// <summary>
        /// Get sample data for version 2.0
        /// </summary>
        /// <returns>Enhanced sample data</returns>
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new
            {
                Version = "2.0",
                Message = "This is Version 2.0 with enhanced features",
                Timestamp = DateTime.UtcNow,
                Features = new[] { "Advanced CRUD", "Enhanced Authentication", "Real-time Updates", "Analytics" },
                Performance = new { ResponseTime = "Fast", Caching = "Enabled" }
            });
        }

        /// <summary>
        /// Get enhanced user info for version 2.0
        /// </summary>
        /// <param name="id">User ID</param>
        /// <returns>Enhanced user information</returns>
        [HttpGet("user/{id}")]
        public IActionResult GetUser(int id)
        {
            return Ok(new
            {
                Id = id,
                Version = "2.0",
                Name = $"User {id}",
                Email = $"user{id}@example.com",
                Profile = new
                {
                    LastLogin = DateTime.UtcNow.AddDays(-1),
                    Preferences = new { Theme = "Dark", Language = "en-US" },
                    Statistics = new { LoginCount = 42, PostsCount = 15 }
                }
            });
        }

        /// <summary>
        /// Enhanced endpoint only available in V2.0
        /// </summary>
        /// <returns>Enhanced features data</returns>
        [HttpGet("enhanced")]
        public IActionResult GetEnhanced()
        {
            return Ok(new
            {
                Version = "2.0",
                Message = "Enhanced endpoint only available in V2.0",
                Features = new[] { "AI Integration", "Advanced Analytics", "Real-time Collaboration" },
                EnhancedData = new
                {
                    Analytics = new { PageViews = 1234, UniqueVisitors = 567 },
                    AIInsights = new { Recommendations = 8, Predictions = 12 },
                    Performance = new { LoadTime = "0.2s", CacheHitRate = "95%" }
                }
            });
        }
    }
}
