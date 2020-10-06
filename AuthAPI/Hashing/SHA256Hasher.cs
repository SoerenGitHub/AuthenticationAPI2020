using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace AuthAPI.Hashing {
    public class SHA256Hasher {

        public async Task<string> EncryptStringSHA256Async(string data, string salt) {
            return await Task.Run(() => {
                try {
                    if(salt == null) {
                        return null;
                    }
                    byte[] bytes = Encoding.UTF8.GetBytes(data + salt);
                    SHA256Managed sha256HashString = new SHA256Managed();
                    byte[] hash = sha256HashString.ComputeHash(bytes);
                    return Convert.ToBase64String(hash);
                } catch(Exception) {
                    return null;
                }
            });
        }

        public async Task<string> GenerateSaltAsync(int size) {
            return await Task.Run(() => {
                try {
                    RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider();
                    byte[] data = new byte[size];
                    provider.GetBytes(data);
                    return Convert.ToBase64String(data);
                } catch(Exception) {
                    return null;
                }
            });
        }
    }
}