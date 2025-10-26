namespace Entity.Records;

public record class PlateDetectedEventRecord
{
    public string Plate { get; init; } = string.Empty;   // Placa detectada
    public string CameraId { get; init; } = string.Empty; // Identificador de la cámara
    public int? ParkingId { get; set; }              // Multi-parking (opcional)
    public DateTime Timestamp { get; init; }             // Momento detección
    public string FrameId { get; init; } = string.Empty; // Id del frame o detección
    public string? ImageUrl { get; init; }   
}
