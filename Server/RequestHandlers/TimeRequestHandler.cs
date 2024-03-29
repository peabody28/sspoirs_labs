﻿using Core;

namespace Server.RequestHandlers
{
    public class TimeRequestHandler : IRequestHandler
    {
        private readonly DateTime _startTime;

        public TimeRequestHandler()
        {
            _startTime = DateTime.UtcNow;
        }

        public byte[] Handle(byte[] contentBytes, State state, out Status status, out string error)
        {
            error = string.Empty;

            var workingTime = DateTime.UtcNow - _startTime;

            status = Status.Ok;

            return StringHelper.ToBytes(workingTime.ToString());
        }
    }
}
