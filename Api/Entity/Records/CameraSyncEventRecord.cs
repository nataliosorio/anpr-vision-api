namespace Entity.Records;

public record CameraSyncEventRecord(
    string Action,                   // "CREATE", "UPDATE", "DELETE"
    CameraSyncPayloadRecord Camera
);


