using Core;

namespace Server.RequestHandlers
{
    public class FileDownloadRequestHandler : IRequestHandler
    {
        private Dictionary<Guid, State> _userState;

        public FileDownloadRequestHandler() 
        {
            _userState = new Dictionary<Guid, State>();
        }

        public string Handle(string content, out Status status, out string error)
        {
            //if (!_userState.TryGetValue(userId.Value, out var state))
            //    _userState.Add(userId.Value, new State());

            var fileName = content;

            var isSuccess = TryGetFile(fileName, out var respContent, out error);

            status = isSuccess ? Status.FileSended : Status.Error;

            return respContent;
        }

        private bool TryGetFile(string fileName, out string content, out string error)
        {
            error = string.Empty;
            content = string.Empty;

            try
            {
                content = File.ReadAllText(fileName);
                return true;
            }
            catch (Exception ex)
            {
                error = "An error occured";
                return false;
            }
        }
    }
}
