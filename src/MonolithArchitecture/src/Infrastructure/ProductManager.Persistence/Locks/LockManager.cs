using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using ProductManager.Shared.DateTimes;
using ProductManager.Shared.Exceptions;
using ProductManager.Shared.Locks;
namespace ProductManager.Persistence.Locks;

public class LockManager : ILockManager
{
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly ApplicationDbContext _dbContext;

    public LockManager(ApplicationDbContext dbContext, IDateTimeProvider dateTimeProvider)
    {
        _dbContext = dbContext;
        _dateTimeProvider = dateTimeProvider;
    }
    public bool AcquireLock(string entityName, string entityId, string ownerId, TimeSpan expirationIn)
    {
        CreateLock(entityName, entityId, ownerId);

        if (ExtendLock(entityName, entityId, ownerId, expirationIn))
        {
            return true;
        }

        var now = _dateTimeProvider.OffsetNow;
        var expired = now + expirationIn;

        var sql = @"
            Update Locks set OwnerId = @OwnerId,
            AcquiredDateTime = @AcquiredDateTime,
            ExpiredDateTime = @ExpiredDateTime
            where EntityId = @EntityId
            and EntityName = @EntityName
            and (OwnerId is NULL or ExpiredDateTime < @AcquiredDateTime)";

        var rs = _dbContext.Database.ExecuteSqlRaw(sql,
        new SqlParameter("EntityName", entityName),
        new SqlParameter("EntityId", entityId),
        new SqlParameter("OwnerId", ownerId),
        new SqlParameter("AcquiredDateTime", SqlDbType.DateTimeOffset)
        {
            Value = now
        },
        new SqlParameter("ExpiredDateTime", SqlDbType.DateTimeOffset)
        {
            Value = expired
        });

        return rs > 0;
    }

    public bool ExtendLock(string entityName, string entityId, string ownerId, TimeSpan expirationIn)
    {
        var now = _dateTimeProvider.OffsetNow;
        var expired = now + expirationIn;

        var sql = @"
            Update Locks set ExpiredDateTime = @ExpiredDateTime
            where EntityId = @EntityId
            and EntityName = @EntityName
            and OwnerId = @OwnerId";

        var rs = _dbContext.Database.ExecuteSqlRaw(sql,
        new SqlParameter("EntityName", entityName),
        new SqlParameter("EntityId", entityId),
        new SqlParameter("OwnerId", ownerId),
        new SqlParameter("ExpiredDateTime", SqlDbType.DateTimeOffset)
        {
            Value = expired
        });

        return rs > 0;
    }

    public bool ReleaseLock(string entityName, string entityId, string ownerId)
    {
        var sql = @"
            Update Locks set OwnerId = NULL,
            AcquiredDateTime = NULL,
            ExpiredDateTime = NULL
            where EntityId = @EntityId
            and EntityName = @EntityName
            and OwnerId = @OwnerId";

        _ = _dbContext.Database.ExecuteSqlRaw(sql,
        new SqlParameter("EntityName", entityName),
        new SqlParameter("EntityId", entityId),
        new SqlParameter("OwnerId", ownerId));

        return true;
    }

    public bool ReleaseLocks(string ownerId)
    {
        var sql = @"
            Update Locks set OwnerId = NULL,
            AcquiredDateTime = NULL,
            ExpiredDateTime = NULL
            where OwnerId = @OwnerId";

        _ = _dbContext.Database.ExecuteSqlRaw(sql,
        new SqlParameter("OwnerId", ownerId));

        return true;
    }

    public bool ReleaseExpiredLocks()
    {
        var now = _dateTimeProvider.OffsetNow;

        var sql = @"
            Update Locks set OwnerId = NULL,
            AcquiredDateTime = NULL,
            ExpiredDateTime = NULL
            where ExpiredDateTime < @now";

        _ = _dbContext.Database.ExecuteSqlRaw(sql,
        new SqlParameter("now", SqlDbType.DateTimeOffset)
        {
            Value = now
        });

        return true;
    }

    public void EnsureAcquiringLock(string entityName, string entityId, string ownerId, TimeSpan expirationIn)
    {
        if (!AcquireLock(entityName, entityId, ownerId, expirationIn))
        {
            throw new CouldNotAcquireLockException();
        }
    }
    private void CreateLock(string entityName, string entityId, string ownerId)
    {
        var sql = @"
            merge into
                [dbo].[Locks] with (holdlock) t
            using
                (values (@entityName, @entityId, @ownerId)) s([EntityName], [EntityId], [OwnerId])
            on
                t.[EntityName] = s.[EntityName] and t.[EntityId] = s.[EntityId]
            when not matched then
                insert ([EntityName], [EntityId], [OwnerId]) values (s.[EntityName], s.[EntityId], s.[OwnerId]);
            ";

        _dbContext.Database.ExecuteSqlRaw(sql,
        new SqlParameter("entityName", entityName),
        new SqlParameter("entityId", entityId),
        new SqlParameter("ownerId", ownerId));
    }
}
