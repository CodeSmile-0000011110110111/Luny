namespace Luny
{
    /// <summary>
    /// Lifecycle observer interface - receives callbacks from dispatcher.
    /// </summary>
    public interface IEngineLifecycle
    {
        // Lifecycle observer interface - receives callbacks from dispatcher
        void OnStartup();
        void OnUpdate(double deltaTime);
        void OnFixedStep(double fixedDeltaTime);
        void OnShutdown();
        // add remaining lifecycle callbacks here
    }
}
