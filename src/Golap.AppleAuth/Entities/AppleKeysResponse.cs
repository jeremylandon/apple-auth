using System.Collections.Generic;
using Microsoft.IdentityModel.Tokens;

namespace Golap.AppleAuth.Entities
{
    /// <summary>
    /// Response send by apple 
    /// </summary>
    internal class AppleKeysResponse
    {
        public IReadOnlyCollection<JsonWebKey> Keys { get; set; }
    }
}
