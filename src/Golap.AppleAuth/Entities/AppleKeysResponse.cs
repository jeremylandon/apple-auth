using System.Collections.Generic;
using Microsoft.IdentityModel.Tokens;

namespace Golap.AppleAuth.Entities
{
    public class AppleKeysResponse
    {
        public IReadOnlyCollection<JsonWebKey> Keys { get; set; }
    }
}
