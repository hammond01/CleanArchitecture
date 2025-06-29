using System.Runtime.Serialization;

namespace ProductManager.Domain.Common;

[Serializable]
[DataContract]
public class ApiResponse<T>
{
    [DataMember]
    public int StatusCode { get; set; }

    public bool IsSuccessStatusCode => StatusCode is >= 200 and < 300;

    [DataMember]
    public string Message { get; set; } = string.Empty;

    [DataMember(EmitDefaultValue = false)]
    public T Result { get; set; } = default!;
}
public class ApiResponse : ApiResponse<object>
{
    public ApiResponse()
    {
    }

    public ApiResponse(int statusCode, string message)
    {
        StatusCode = statusCode;
        Message = message;
    }
    public ApiResponse(int statusCode, string message, object result)
    {
        StatusCode = statusCode;
        Message = message;
        Result = result;
    }
}
