using System;

namespace Etherna.BeeNet.DtoModels
{
    public class LoggersDto
    {
        // Constructors.
        internal LoggersDto(Clients.DebugApi.V4_0_0.Loggers loggers)
        {
            if (loggers is null)
                throw new ArgumentNullException(nameof(loggers));

            Id = loggers.Id;
            Logger = loggers.Logger;
            Subsystem = loggers.Subsystem;
            Verbosity = loggers.Verbosity;
        }

        internal LoggersDto(Clients.DebugApi.V4_0_0.Loggers2 loggers2)
        {
            if (loggers2 is null)
                throw new ArgumentNullException(nameof(loggers2));

            Id = loggers2.Id;
            Logger = loggers2.Logger;
            Subsystem = loggers2.Subsystem;
            Verbosity = loggers2.Verbosity;
        }

        // Properties.
        public string Id { get; set; }
        public string Logger { get; set; }
        public string Subsystem { get; set; }
        public string Verbosity { get; set; }
    }
}
