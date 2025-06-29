namespace ProductManager.Constants.ApiResponseConstants;

public static class CRUDMessage
{
    /// <summary>
    ///     Success message for CRUD operations
    /// </summary>
    public static string CreateSuccess => "Entity create success";
    public static string GetSuccess => "Entity get success";
    public static string UpdateSuccess => "Entity update success";
    public static string DeleteSuccess => "Entity delete success";

    /// <summary>
    ///     Failed message for CRUD operations
    /// </summary>
    public static string CreateFailed => "Entity create failed";
    public static string GetFailed => "Entity get failed";
    public static string UpdateFailed => "Entity update failed";
    public static string DeleteFailed => "Entity delete failed";
}
