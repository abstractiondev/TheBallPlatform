using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using TheBall.Core.InstanceSupport;

namespace TheBall.Core
{
    public class ActivateEmailValidationImplementation
    {
        public static async Task<bool> ExecuteMethod_ProcessEmailValidationActivationAsync(Email email, bool sendValidationCodeIfReissued, bool resendValidationCode)
        {
            var utcNow = DateTime.UtcNow;
            bool isReissued = false;
            if (email.ValidationKey == null || email.ValidationProcessExpiration < utcNow)
            {
                var expirationTime = utcNow.AddMinutes(5);
                email.PendingValidation = true;
                email.ValidationKey = KeyGenerator.GetUniqueKey(8);
                email.ValidationProcessExpiration = expirationTime;
                isReissued = true;
            }
            bool sendEmailValidation = resendValidationCode || (isReissued && sendValidationCodeIfReissued);
            if (sendEmailValidation)
            {
                await EmailSupport.SendEmailValidationAsync(email.EmailAddress, email.ValidationKey);
            }
            return isReissued;
        }

        public static async Task ExecuteMethod_StoreEmailIfChangedAsync(Email email, bool processEmailValidationActivationOutput)
        {
            if (processEmailValidationActivationOutput)
                await email.StoreInformationAsync();
        }


        public class KeyGenerator
        {
            public static string GetUniqueKey(int maxSize)
            {
                char[] chars = new char[62];
                chars =
                "ABCDEFGHJKLMNPQRSTUVWXY3456789".ToCharArray();
                byte[] data = new byte[1];
                using (RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider())
                {
                    crypto.GetNonZeroBytes(data);
                    data = new byte[maxSize];
                    crypto.GetNonZeroBytes(data);
                }
                StringBuilder result = new StringBuilder(maxSize);
                foreach (byte b in data)
                {
                    result.Append(chars[b % (chars.Length)]);
                }
                return result.ToString();
            }
        }

    }
}