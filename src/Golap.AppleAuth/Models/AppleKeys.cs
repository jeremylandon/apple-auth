using System.Collections.Generic;
using Microsoft.IdentityModel.Tokens;

namespace Golap.AppleAuth.Models
{
    public class AppleKeys
    {
        public IReadOnlyCollection<JsonWebKey> Keys { get; set; }
    }
}
