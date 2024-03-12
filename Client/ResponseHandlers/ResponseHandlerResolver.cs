using Core;

namespace Client.ResponseHandlers
{
    public class ResponseHandlerResolver
    {
        private State _state;

        private readonly IDictionary<Command, IResponseHandler> _responseHandlers;

        public ResponseHandlerResolver()
        {
            _responseHandlers = new Dictionary<Command, IResponseHandler>
            {
                { Command.Download, new DownloadFileReponseHandler() },
                { Command.Echo, new DefaultReponseHandler() },
                { Command.Time, new DefaultReponseHandler() },
                { Command.Upload, new DefaultReponseHandler() },
            };

            _state = new State();
        }

        public void Handle(Packet packet)
        {
            _responseHandlers[packet.Command].Handle(packet);
        }
    }
}
