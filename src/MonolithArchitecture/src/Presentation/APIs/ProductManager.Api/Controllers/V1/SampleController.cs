using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;

namespace ProductManager.Api.Controllers.V1
{
    /// <summary>
    /// Sample controller for API version 1.0 - demonstrates basic versioning
    /// </summary>
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class SampleController : ControllerBase
    {
        /// <summary>
        /// Get sample data for version 1.0
        /// </summary>
        /// <returns>Sample data</returns>
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new
            {
                Version = "1.0",
                Message = "This is Version 1.0",
                Timestamp = DateTime.UtcNow,
                Features = new[] { "Basic CRUD", "Authentication" }
            });
        }

        /// <summary>
        /// Get user info for version 1.0
        /// </summary>
        /// <param name="id">User ID</param>
        /// <returns>User information</returns>
        [HttpGet("user/{id}")]
        public IActionResult GetUser(int id)
        {
            return Ok(new
            {
                Id = id,
                Version = "1.0",
                Name = $"User {id}",
                Email = $"user{id}@example.com"
            });
        }
    }
}
