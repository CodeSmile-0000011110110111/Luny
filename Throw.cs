using System;

namespace Luny
{
    public sealed class Throw
    {
        public static void LifecycleAdapterSingletonDuplicationException(String adapterTypeName, String existingObjectName,
            Int64 existingInstanceId, String duplicateObjectName, Int64 duplicateInstanceId) => throw new InvalidOperationException(
            $"Duplicate {adapterTypeName} singleton detected! " +
            $"Existing: Name='{existingObjectName}' InstanceID={existingInstanceId}, " +
            $"Duplicate: Name='{duplicateObjectName}' InstanceID={duplicateInstanceId}");

        public static void SingletonDuplicationException(String typeName) =>
            throw new InvalidOperationException($"Duplicate {typeName} singleton detected!");

        public static void ServiceNotFoundException(String serviceName) =>
            throw new InvalidOperationException($"Service {serviceName} not found in registry.");

        public static void LifecycleAdapterPrematurelyRemovedException(String godotLifecycleAdapterName) =>
            throw new InvalidOperationException(
                $"{godotLifecycleAdapterName} unexpectedly removed from SceneTree! It must remain in scene at all times.");
    }
}
