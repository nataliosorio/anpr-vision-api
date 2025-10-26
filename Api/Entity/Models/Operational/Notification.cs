using Entity.Models.Parameter;
using System;

namespace Entity.Models.Operational;

public class Notification : BaseModel
{

    public int? ParkingId { get; set; }   // Multi-parking
    public Parking? Parking { get; set; } = null!;

    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;

    public string Type { get; set; } = "Info"; // Info, Success, Warning, Error

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsRead { get; set; } = false;

    // This attr it's for know what is the Id of the created vehicleEntry
    public int? RelatedEntityId { get; set; } 
}
