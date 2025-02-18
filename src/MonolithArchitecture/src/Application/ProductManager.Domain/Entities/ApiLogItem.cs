namespace ProductManager.Domain.Entities;

public class ApiLogItem
{
    [Key]
    public long Id { get; set; }

    [Required(ErrorMessage = "FieldRequired")]
    public DateTimeOffset RequestTime { get; set; }

    [Required(ErrorMessage = "FieldRequired")]
    public long ResponseMillis { get; set; }

    [Required(ErrorMessage = "FieldRequired")]
    public int StatusCode { get; set; }

    [Required(ErrorMessage = "FieldRequired")]
    public string Method { get; set; } = default!;

    [Required(ErrorMessage = "FieldRequired")]
    [MaxLength(2048)]
    public string Path { get; set; } = default!;

    [MaxLength(2048)]
    public string QueryString { get; set; } = default!;

    [MaxLength(256)]
    public string RequestBody { get; set; } = default!;

    [MaxLength(256)]
    public string ResponseBody { get; set; } = default!;

    [MaxLength(45)]
    public string IpAddress { get; set; } = default!;

    public Guid? ApplicationUserId { get; set; }

    public User User { get; set; } = default!;
}
