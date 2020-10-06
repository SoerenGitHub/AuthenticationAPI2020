using System;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;

namespace AuthAPI.Auth {
    public class RSAManager {
        private readonly RsaSecurityKey _key;

        private static readonly Lazy<RSAManager> instance = new Lazy<RSAManager>(
           () => new RSAManager()
        );

        private RSAManager()
        {
            _key = new RsaSecurityKey(RSA.Create(2048));
        }

        public RsaSecurityKey Key {
            get {
                return _key;
            }
        }

        public static RSAManager GetInstance
        {
            get
            {
                return instance.Value;
            }
        }
    }
}