namespace BuildingBlocks.Domain.Entities;

public interface ITrackable
{
    int RowId { get; set; }

    DateTimeOffset CreatedDateTime { get; set; }

    DateTimeOffset? UpdatedDateTime { get; set; }
}
