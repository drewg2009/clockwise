using System;
using System.Text;

namespace Clockwise
{
    public class EncoderHelper
    {
        public static EncoderHelper Instance = new EncoderHelper();

        private EncoderHelper(){
            
        }

		public string Base64Encode(string plainText)
		{
			var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
			return Convert.ToBase64String(plainTextBytes);
		}

		public string Base64Decode(string base64EncodedData)
		{
			byte[] base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return Encoding.UTF8.GetString(base64EncodedBytes, 0 , base64EncodedData.Length);
		}
    }
}
