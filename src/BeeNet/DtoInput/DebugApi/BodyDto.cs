#pragma warning disable CA2227 // Disable "CS2227 Collection properties should be read only"

using System;
using System.Collections.Generic;
using System.Text;

namespace Etherna.BeeNet.DtoInput.DebugApi
{
    public class BodyDto : BaseDto
    {
        public BodyDto(string welcomeMessage, 
            IDictionary<string, object> additionalProperties)
            : base(additionalProperties)
        {
            WelcomeMessage = welcomeMessage;
        }

        public string WelcomeMessage { get; set; }
    }
}

#pragma warning restore CA2227