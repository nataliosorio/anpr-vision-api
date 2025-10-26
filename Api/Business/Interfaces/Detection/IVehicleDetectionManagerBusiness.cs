using System;
using Entity.Records;

namespace Business.Interfaces.Detection;

public interface IVehicleDetectionManagerBusiness
{
    Task ProcessDetectionAsync(PlateDetectedEventRecord evt,CancellationToken cancellationToken = default);
}
