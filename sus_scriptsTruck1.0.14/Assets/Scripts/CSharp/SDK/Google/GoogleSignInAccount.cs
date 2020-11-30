using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GoogleSignInAccount
{
    public string id;
    public string tokenId;
    public string email;
    public string displayName;
    public string givenName;
    public string familyName;
    public string photoUrl;
    public string serverAuthCode;
    public long expirationTime;
    public string obfuscatedIdentifier;
    public string[] grantedScopes;
}
