# Apple-Auth

Sign in with Apple for .NET

## Usage

### Apple client

```csharp
var authSetting = new AppleAuthSetting("0ABCD123XQ", "com.my.app", "https://my.com/apple/redirect");
var keySetting = new AppleKeySetting("X987654321", File.ReadAllText("Files/key.p8"));

var client = new AppleAuthClient(authSetting, keySetting);
```

#### Methods

- client.LoginUri() - Creates the Login URL that your users will use to login to
- client.AccessTokenAsync(grantCode) - Gets the access token from the grant code received
- client.RefreshTokenAsync(refreshToken) - Gets the access token from a refresh token

### Apple Verifier

```csharp
var verifier = new AppleVerifier();
```

#### Methods

- client.ValidateAsync(jwtToken) - Validates a Apple-issued Json Web Token (JWT)

## License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details
