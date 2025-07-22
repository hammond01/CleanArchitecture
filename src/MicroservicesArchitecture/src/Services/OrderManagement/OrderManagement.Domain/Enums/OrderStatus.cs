namespace OrderManagement.Domain.Entities;

/// <summary>
/// Order status enumeration
/// </summary>
public enum OrderStatus
{
    /// <summary>
    /// Order has been created but not yet confirmed
    /// </summary>
    Pending = 1,

    /// <summary>
    /// Order has been confirmed and is ready for processing
    /// </summary>
    Confirmed = 2,

    /// <summary>
    /// Order is being processed (items being prepared)
    /// </summary>
    Processing = 3,

    /// <summary>
    /// Order has been shipped to customer
    /// </summary>
    Shipped = 4,

    /// <summary>
    /// Order has been delivered to customer
    /// </summary>
    Delivered = 5,

    /// <summary>
    /// Order has been cancelled
    /// </summary>
    Cancelled = 6,

    /// <summary>
    /// Order has been refunded
    /// </summary>
    Refunded = 7
}
