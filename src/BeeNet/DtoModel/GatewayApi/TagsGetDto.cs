#pragma warning disable CA2227 // Disable "CS2227 Collection properties should be read only"

using System;
using System.Collections.Generic;
using System.Text;

namespace Etherna.BeeNet.DtoModel.GatewayApi
{
    public class TagsGetDto : BaseDto
    {
        public TagsGetDto(
            ICollection<TagsDto> tags, 
            IDictionary<string, object> additionalProperties)
            : base(additionalProperties)
        {
            Tags = tags;
        }

        public ICollection<TagsDto> Tags { get; set; }
    }
}

#pragma warning restore CA2227
