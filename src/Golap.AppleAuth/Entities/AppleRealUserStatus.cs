namespace Golap.AppleAuth.Entities
{
    /// <summary>
    /// Apple determines whether a user is a real person by combining on-device machine learning, account history, and hardware attestation using privacy-preserving mechanisms.
    /// </summary>
    public enum AppleRealUserStatus
    {
        /// <summary>
        /// Real user status is only supported on iOS 14 and later, macOS 11 and later, watchOS 7 and later, and tvOS 14 and later.
        /// </summary>
        Unsupported = 0,
        /// <summary>
        /// The system can't determine whether the user is a real person.
        /// </summary>
        Unknown = 1,
        /// <summary>
        /// The user appears to be a real person, and you can treat this account as a valid user.
        /// </summary>
        LikelyReal = 2
    }
}
