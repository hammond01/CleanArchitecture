using System.ComponentModel.DataAnnotations;

namespace OrderManagement.API.Models;

public class Customer
{
    [Key]
    public int CustomerId { get; set; }
    
    [Required]
    public string FirstName { get; set; } = string.Empty;
    
    [Required]
    public string LastName { get; set; } = string.Empty;
    
    [Required]
    public string Email { get; set; } = string.Empty;
    
    public string? Phone { get; set; }
    
    public string? Address { get; set; }
    
    public string? City { get; set; }
    
    public string? State { get; set; }
    
    public string? ZipCode { get; set; }
    
    public string? Country { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime? UpdatedAt { get; set; }
    
    // Navigation properties
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
    
    // Computed properties
    public string FullName => $"{FirstName} {LastName}";
}
