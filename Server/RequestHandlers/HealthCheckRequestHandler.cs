using Core;

namespace Server.RequestHandlers
{
    public class HealthCheckRequestHandler : IRequestHandler
    {
        public byte[] Handle(byte[] content, State state, out Status status, out string error)
        {
            error = string.Empty;
            status = Status.Ok;

            state.LastHandshake = DateTime.UtcNow;
            state.PackagesSendedWhenHandshake = state.PackagesSended;

            Console.WriteLine("HealthCheck passed");
            return null;
        }
    }
}
