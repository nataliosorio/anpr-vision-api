using System;
using Entity.Records;

namespace Business.Interfaces.Producer;

public interface IKafkaProducerService
{
    Task SendCameraSyncAsync(CameraSyncEventRecord eventRecord, CancellationToken cancellationToken = default);
}
