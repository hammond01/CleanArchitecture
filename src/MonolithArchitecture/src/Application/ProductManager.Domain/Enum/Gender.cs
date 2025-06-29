using System.Text.Json.Serialization;

namespace ProductManager.Domain.Enum;

[JsonConverter(typeof(JsonStringEnumConverter<Gender>))]
public enum Gender
{
    Other,
    Male,
    Female
}
