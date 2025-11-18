namespace Entity.Records;

public record CameraSyncPayloadRecord(
    int Id,
    int ParkingId,
    string Name,
    string Url,
    string Resolution,
    bool Asset,
    bool? IsDeleted
);

