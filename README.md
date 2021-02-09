# Apple-Auth

Sign in with Apple for .NET

## Install via [Nuget.org](https://www.nuget.org/packages/Golap.AppleAuth/)

`Install-Package Golap.AppleAuth`

## Usage

### Apple client

```csharp
var teamId = "0ABCD123XQ"; // Can be retrieve on the Membership information section (https://developer.apple.com/account/#/membership)
var clientId = "com.my.app"; // Identifier of the Apple Service used to Sign in with Apple (https://developer.apple.com/account/resources/identifiers/list/serviceId)
var redirectUri = "https://my.com/apple/redirect"; // your callback
var keyId = "X987654321"; // Id of the key (https://developer.apple.com/account/resources/authkeys)
var keyPath = "Files/key.p8"; // Can be download during the key creation (https://developer.apple.com/account/resources/authkeys)

var authSetting = new AppleAuthSetting(teamId, clientId, redirectUri);
var keySetting = new AppleKeySetting(keyId, File.ReadAllText(keyPath));

var client = new AppleAuthClient(authSetting, keySetting);
```

#### Methods

- **LoginUri()** - Generates the Login URI that your users will use to login to
- **AccessTokenAsync(grantCode)** - Get the access token from the server based on the grant code returned by Apple
- **RefreshTokenAsync(refreshToken)** - Refresh an accessToken from a refresh token

### Apple Verifier

```csharp
var verifier = new AppleVerifier();
```

#### Methods

- **ValidateAsync(jwtToken, clientId)** - Check an Apple token validity

## Common errors

- **"Invalid_grant"** error when you go to a generated login uri

  - **Check that the redirect url is added to your [Apple service](https://developer.apple.com/account/resources/identifiers/list/serviceId) in your developer account**
  - Check the [validity of your settings](https://auth0.com/blog/what-is-sign-in-with-apple-a-new-identity-provider/)
- **AppleAuthException** when you try to get an access token
  - The grant code is invalid
  - **An Apple grant code sent on your redirect URI can only used once for security reasons**

## Useful links

- [GitHub Sample](https://github.com/Golapadeog/apple-auth/tree/master/sample)
- [Configuring your Apple Developer Account](https://auth0.com/blog/what-is-sign-in-with-apple-a-new-identity-provider/)

## License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details
