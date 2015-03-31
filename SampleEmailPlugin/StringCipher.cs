using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace SampleEmailPlugin
{
	public static class StringCipher
	{
		// This constant string is used as a "salt" value for the PasswordDeriveBytes function calls.
		// This size of the IV (in bytes) must = (keysize / 8).  Default keysize is 256, so the IV must be
		// 32 bytes long.  Using a 16 character string here gives us 32 bytes when converted to a byte array.
		private static readonly byte[] initVectorBytes = Encoding.ASCII.GetBytes("tu89geji340t89u2");

		private const string PASS_PHRASE = "emailPassEncryption";

		// Artifact of encryption algorithm, used to determine if encrypted
		private const string PADDING = "==";

		// This constant is used to determine the keysize of the encryption algorithm.
		private const int KEY_SIZE = 256;

		private static readonly byte[] saltArray = Encoding.ASCII.GetBytes("this is my salt");

		public static bool IsEncrypted(string text)
		{
			return text.EndsWith(PADDING);
		}

		public static string Encrypt(string plainText)
		{
			if(IsEncrypted(plainText))
			{
				return plainText;
			}

			byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);
			
			using (Rfc2898DeriveBytes password = new Rfc2898DeriveBytes(PASS_PHRASE, saltArray))
			{
				byte[] keyBytes = password.GetBytes(KEY_SIZE / 8);
				using (RijndaelManaged symmetricKey = new RijndaelManaged())
				{
					symmetricKey.Mode = CipherMode.CBC;
					using (ICryptoTransform encryptor = symmetricKey.CreateEncryptor(keyBytes, initVectorBytes))
					{
						using (MemoryStream memoryStream = new MemoryStream())
						{
							using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
							{
								cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
								cryptoStream.FlushFinalBlock();
								byte[] cipherTextBytes = memoryStream.ToArray();
								return Convert.ToBase64String(cipherTextBytes);
							}
						}
					}
				}
			}
		}

		public static string Decrypt(string cipherText)
		{
			if(!cipherText.EndsWith(PADDING))
			{
				return cipherText;
			}
			
			byte[] cipherTextBytes = Convert.FromBase64String(cipherText);
			using (Rfc2898DeriveBytes password = new Rfc2898DeriveBytes(PASS_PHRASE, saltArray))
			{
				byte[] keyBytes = password.GetBytes(KEY_SIZE / 8);
				using (RijndaelManaged symmetricKey = new RijndaelManaged())
				{
					symmetricKey.Mode = CipherMode.CBC;
					using (ICryptoTransform decryptor = symmetricKey.CreateDecryptor(keyBytes, initVectorBytes))
					{
						using (MemoryStream memoryStream = new MemoryStream(cipherTextBytes))
						{
							using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
							{
								byte[] plainTextBytes = new byte[cipherTextBytes.Length];
								int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
								return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
							}
						}
					}
				}
			}
		}
	}
}
