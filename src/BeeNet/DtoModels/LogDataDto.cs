using System;
using System.Collections.Generic;
using System.Linq;

namespace Etherna.BeeNet.DtoModels
{
    public class LogDataDto
    {
        // Constructors.
        internal LogDataDto(Clients.DebugApi.V4_0_0.Response44 response44)
        {
            if (response44 is null)
                throw new ArgumentNullException(nameof(response44));
            
            Tree = response44.Tree.ToDictionary(i => i.Key, i => i.Value?.Plus?.ToList() ?? new List<string>());
            Loggers = response44.Loggers.Select(i => new LoggersDto(i)).ToList();
        }

        internal LogDataDto(Clients.DebugApi.V4_0_0.Response45 response45)
        {
            if (response45 is null)
                throw new ArgumentNullException(nameof(response45));

            Tree = response45.Tree.ToDictionary(i => i.Key, i => i.Value?.Plus?.ToList() ?? new List<string>());
            Loggers = response45.Loggers.Select(i => new LoggersDto(i)).ToList();
        }

        // Properties.
        public IDictionary<string, List<string>> Tree { get; set; }
        public ICollection<LoggersDto> Loggers { get; set; }
    }
}
