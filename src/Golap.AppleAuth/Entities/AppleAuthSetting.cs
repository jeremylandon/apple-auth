namespace Golap.AppleAuth.Entities
{
    /// <summary>
    /// Configuration of <see cref="AppleAuthClient"/> 
    /// </summary>
    public class AppleAuthSetting
    {
        /// <summary>
        /// The subscription identifier
        /// </summary>
        public string TeamId { get; }
        /// <summary>
        /// The service's identfier or bundle id
        /// </summary>
        public string ClientId { get; }
        /// <summary>
        /// The OAuth Redirect URI
        /// </summary>
        public string RedirectUri { get; }
        /// <summary>
        /// The scope of information you want to get from the user
        /// </summary>
        public string Scope { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AppleAuthSetting"/> class.
        /// </summary>
        /// <param name="teamId">Your subscription identifier present in the membership section in your apple developer account</param>
        /// <param name="clientId">The service's identfier or bundle id of your application</param>
        /// <param name="redirectUri">The OAuth Redirect URI of your service</param>
        /// <param name="scope">The scope of information you want to get from the user</param>
        public AppleAuthSetting(string teamId, string clientId, string redirectUri, string scope = null)
        {
            TeamId = teamId;
            ClientId = clientId;
            RedirectUri = redirectUri;
            Scope = scope;
        }
    }
}
