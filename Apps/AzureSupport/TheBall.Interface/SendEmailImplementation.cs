using System;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using TheBall.CORE;
using TheBall.CORE.InstanceSupport;
using TheBall.CORE.Storage;
using TheBall.Interface.INT;

namespace TheBall.Interface
{
    public class SendEmailImplementation
    {
        public static void ExecuteMethod_ValidateThatEmailSendingIsAllowed()
        {
            var currOwner = InformationContext.CurrentOwner;
            if (currOwner.IsGroupContainer() && InstanceConfig.Current.AllowEmailSendFromGroup)
                return;
            if (currOwner.IsAccountContainer() && InstanceConfig.Current.AllowEmailSendFromAccount)
                return;
            throw new InvalidOperationException("Email sending not allowed");
        }

        public static async Task<string[]> GetTarget_EmailAddressesAsync(string[] recipientAccountIDs)
        {
            if (recipientAccountIDs == null)
                return new string[0];
            var accountTasks =
                recipientAccountIDs.Select(accountID => ObjectStorage.RetrieveFromSystemOwner<Account>(accountID))
                    .ToArray();
            await Task.WhenAll(accountTasks);
            var accounts = accountTasks.Select(task => task.Result).ToArray();
            var emailAddresses = accounts.Select(account => account.Emails.FirstOrDefault())
                .Where(emailID => emailID != null)
                .Select(emailID => Email.GetEmailAddressFromID(emailID))
                .ToArray();
            return emailAddresses;
        }


        public static async Task<EmailSupport.FileAttachment[]> GetTarget_AttachmentsAsync(EmailAttachment[] attachments)
        {
            if (attachments == null)
                return new EmailSupport.FileAttachment[0];
            var owner = InformationContext.CurrentOwner;
            var invalidFilenameCharacters = Path.GetInvalidFileNameChars();
            var fileAttachmentTasks = attachments.Select(async item =>
            {
                var fileName = item.FileName;
                byte[] content = null;
                if (item.TextDataContent != null)
                {
                    content = Encoding.UTF8.GetBytes(item.TextDataContent);
                } else if (item.Base64Content != null)
                {
                    content = Convert.FromBase64String(item.Base64Content);
                }
                else
                {
                    var interfaceDataFile = item.InterfaceDataName;
                    bool invalidFilename = interfaceDataFile.Any(ch => invalidFilenameCharacters.Contains(ch));
                    if (invalidFilename)
                        throw new InvalidDataException("Invalid filename for attachment: " + item.InterfaceDataName);
                    var blobPath = BlobStorage.CombinePath("TheBall.Interface", "InterfaceData", interfaceDataFile);
                    content = await BlobStorage.GetBlobContentA(blobPath, true);
                    if (content == null)
                        throw new InvalidDataException("No requested attachment content available: " + interfaceDataFile);
                }
                if(content == null)
                    throw new InvalidDataException("No attachment content available: " + fileName);
                return new EmailSupport.FileAttachment { FileName = fileName, Data = content};
            }).ToArray();
            await Task.WhenAll(fileAttachmentTasks);
            var fileAttachments = fileAttachmentTasks.Select(task => task.Result).ToArray();
            return fileAttachments;
        }

        public static async Task ExecuteMethod_SendEmailsAsync(EmailPackage emailInfo, string[] emailAddresses, EmailSupport.FileAttachment[] attachments)
        {
            var sendTasks = emailAddresses.Select(emailAddress =>
            {
                var from = InstanceConfig.Current.EmailFromAddress;
                var to = emailAddress;
                var subject = emailInfo.Subject;
                var text = emailInfo.BodyText;
                var html = emailInfo.BodyHtml;
                return EmailSupport.SendEmailAsync(from, to, subject, text, html, attachments);
            }).ToArray();
            await Task.WhenAll(sendTasks);
        }

    }
}