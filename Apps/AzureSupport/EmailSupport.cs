using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using AaltoGlobalImpact.OIP;
using Amazon;
using Amazon.Runtime;
using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using AzureSupport;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Blob;
using MimeKit;
using TheBall.CORE;
using TheBall.CORE.InstanceSupport;

namespace TheBall
{
    public static class EmailSupport
    {
        public class FileAttachment
        {
            public string FileName;
            public byte[] Data;
        }
        private static BodyBuilder GetMessageBodyBuilder(string htmlBody, string textBody, FileAttachment[] attachments = null)
        {
            var body = new BodyBuilder
            {
                HtmlBody = htmlBody,
                TextBody = textBody
            };
            if (attachments != null)
            {
                foreach (var attachment in attachments)
                {
                    var fileName = attachment.FileName;
                    var data = attachment.Data;
                    var mimeType = StorageSupport.GetMimeType(fileName);
                    var contentType = ContentType.Parse(mimeType);
                    body.Attachments.Add(fileName, data, contentType);
                }
            }
            return body;
        }


        public static async Task<bool> SendEmailAsync(string from, string to, string subject, string text,
            string html, FileAttachment[] attachments)
        {
            try
            {
                var request = createEmailRequestWithAttachments(from, to, subject, text, html, attachments);
                var ses = createEmailClient();
                var response = await ses.SendRawEmailAsync(request);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static async Task<Boolean> SendEmailAsync(String From, String To, String Subject, String Text = null, String HTML = null, String emailReplyTo = null, String returnPath = null)
        {
            if (Text != null || HTML != null)
            {
                try
                {
                    var request = createEmailRequest(From, To, Subject, Text, HTML, emailReplyTo, returnPath);

                    var ses = createEmailClient();
                    SendEmailResponse response = await ses.SendEmailAsync(request);
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    //throw;
                    return false;
                }
                finally
                {
                    String queueMessage = String.Format("From: {1}{0}To: {2}{0}Subject: {3}{0}Message:{0}{4}",
                                                        Environment.NewLine, From, To, Subject, Text ?? HTML);
                    await QueueSupport.CurrStatisticsQueue?.AddMessageAsync(new CloudQueueMessage(queueMessage));
                }
            }

            Console.WriteLine("Specify Text and/or HTML for the email body!");

            return false;
        }


        public static async Task<bool> SendEmail(String From, String To, String Subject, String Text = null, String HTML = null, String emailReplyTo = null, String returnPath = null)
        {
            if (Text != null || HTML != null)
            {
                try
                {
                    var request = createEmailRequest(From, To, Subject, Text, HTML, emailReplyTo, returnPath);

                    var ses = createEmailClient();
                    SendEmailResponse response = await ses.SendEmailAsync(request);
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    //throw;
                    return false;
                }
                finally
                {
                    String queueMessage = String.Format("From: {1}{0}To: {2}{0}Subject: {3}{0}Message:{0}{4}",
                                                        Environment.NewLine, From, To, Subject, Text ?? HTML);
                    await QueueSupport.CurrStatisticsQueue?.AddMessageAsync(new CloudQueueMessage(queueMessage));
                }
            }

            Console.WriteLine("Specify Text and/or HTML for the email body!");

            return false;
        }

        private static AmazonSimpleEmailServiceClient createEmailClient()
        {
            string awsAccessKey = SecureConfig.Current.AWSAccessKey;
            string awsSecretKey = SecureConfig.Current.AWSSecretKey;
            AmazonSimpleEmailServiceClient ses =
                new AmazonSimpleEmailServiceClient(new BasicAWSCredentials(awsAccessKey, awsSecretKey), RegionEndpoint.EUWest1);
            return ses;
        }

        private static SendRawEmailRequest createEmailRequestWithAttachments(string from, string to, string subject,
            string text, string html, FileAttachment[] attachments)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("", from));
            message.To.Add(new MailboxAddress("", to));
            message.Subject = subject;
            var bodyBuilder = GetMessageBodyBuilder(html, text, attachments);
            message.Body = bodyBuilder.ToMessageBody();

            var messageStream = new MemoryStream();
            message.WriteTo(messageStream);

            var sendRequest = new SendRawEmailRequest(new RawMessage(messageStream));
            return sendRequest;
        }

        private static SendEmailRequest createEmailRequest(string From, string To, string Subject, string Text,
            string HTML, string emailReplyTo, string returnPath)
        {
            String from = From;

            List<String> to
                = To
                    .Replace(", ", ",")
                    .Split(',')
                    .ToList();

            Destination destination = new Destination(to);
            //destination.WithCcAddresses(cc);
            //destination.WithBccAddresses(bcc);

            Content subject = new Content();
            subject.Charset = "UTF-8";
            subject.Data = Subject;

            Body body = new Body();

            if (HTML != null)
            {
                Content html = new Content();
                html.Charset = "UTF-8";
                html.Data = HTML;
                body.Html = html;
            }

            if (Text != null)
            {
                Content text = new Content();
                text.Charset = "UTF-8";
                text.Data = Text;
                body.Text = text;
            }

            Message message = new Message();
            message.Body = body;
            message.Subject = subject;

            var request = new SendEmailRequest();
            request.Destination = destination;
            request.Message = message;
            request.Source = @from;

            if (emailReplyTo != null)
            {
                List<String> replyto
                    = emailReplyTo
                        .Replace(", ", ",")
                        .Split(',')
                        .ToList();

                request.ReplyToAddresses = replyto;
            }

            if (returnPath != null)
            {
                request.ReturnPath = returnPath;
            }
            return request;
        }

        public static void SendValidationEmail(TBEmailValidation emailValidation)
        {
            string urlLink = GetUrlLink(emailValidation.ID);
            string emailMessageFormat = InstanceConfig.Current.EmailValidationMessageFormat;
#if never
#endif
            string message = string.Format(emailMessageFormat, emailValidation.Email, urlLink);
            SendEmail(InstanceConfig.Current.EmailFromAddress, emailValidation.Email, InstanceConfig.Current.EmailValidationSubjectFormat, message);
        }

        public static void SendGroupJoinEmail(TBEmailValidation emailValidation, TBCollaboratingGroup collaboratingGroup)
        {
            string urlLink = GetUrlLink(emailValidation.ID);
            string emailMessageFormat = InstanceConfig.Current.EmailGroupJoinMessageFormat;
            string message = String.Format(emailMessageFormat, collaboratingGroup.Title, urlLink);
            SendEmail(InstanceConfig.Current.EmailFromAddress, emailValidation.Email,
                String.Format(InstanceConfig.Current.EmailGroupJoinSubjectFormat, collaboratingGroup.Title),
                      message);
        }

        public static void SendMergeAccountsConfirmationEmail(TBEmailValidation mergeAccountEmailConfirmation)
        {
            string urlLink = GetUrlLink(mergeAccountEmailConfirmation.ID);
            string emailMessageFormat = InstanceConfig.Current.EmailAccountMergeValidationMessageFormat;
#if never
#endif
            string message = string.Format(emailMessageFormat, mergeAccountEmailConfirmation.Email, urlLink);
            SendEmail(InstanceConfig.Current.EmailFromAddress, mergeAccountEmailConfirmation.Email, InstanceConfig.Current.EmailAccountMergeValidationSubjectFormat, message);
        }

        private static string GetUrlLink(string emailValidationID)
        {
            string urlLink = InstanceConfig.Current.EmailValidationURLWithoutID + emailValidationID;
            return urlLink;
        }

        public static void SendDeviceJoinEmail(TBEmailValidation emailValidation, DeviceMembership deviceMembership, string[] ownerEmailAddresses)
        {
            string urlLink = GetUrlLink(emailValidation.ID);
            bool isAccount = emailValidation.DeviceJoinConfirmation.AccountID != null;
            string ownerID = isAccount
                                 ? emailValidation.DeviceJoinConfirmation.AccountID
                                 : emailValidation.DeviceJoinConfirmation.GroupID;
            string emailMessageFormat = InstanceConfig.Current.EmailDeviceJoinMessageFormat;
            string message = String.Format(emailMessageFormat, deviceMembership.DeviceDescription,
                                           isAccount ? "account" : "collaboration group", ownerID, urlLink);
            string subject = String.Format(InstanceConfig.Current.EmailDeviceJoinSubjectFormat, ownerID);
            foreach (string emailAddress in ownerEmailAddresses)
            {
                SendEmail(InstanceConfig.Current.EmailFromAddress, emailAddress, subject, message);
            }
        }

        public static async Task SendInputJoinEmail(TBEmailValidation emailValidation, InformationInput informationInput, string[] ownerEmailAddresses)
        {
            string urlLink = GetUrlLink(emailValidation.ID);
            bool isAccount = emailValidation.InformationInputConfirmation.AccountID != null;
            string ownerID = isAccount
                                 ? emailValidation.InformationInputConfirmation.AccountID
                                 : emailValidation.InformationInputConfirmation.GroupID;
            string emailMessageFormat = InstanceConfig.Current.EmailInputJoinMessageFormat;
            string message = String.Format(emailMessageFormat, informationInput.InputDescription,
                                           isAccount ? "account" : "collaboration group", ownerID, urlLink);
            string subject = String.Format(InstanceConfig.Current.EmailInputJoinSubjectFormat, ownerID);
            foreach (string emailAddress in ownerEmailAddresses)
            {
                await SendEmail(InstanceConfig.Current.EmailFromAddress, emailAddress, subject, message);
            }
        }

        public static void SendOutputJoinEmail(TBEmailValidation emailValidation, InformationOutput informationOutput, string[] ownerEmailAddresses)
        {
            string urlLink = GetUrlLink(emailValidation.ID);
            var confirmation = emailValidation.InformationOutputConfirmation;
            bool isAccount = confirmation.AccountID != null;
            string ownerID = isAccount
                                 ? confirmation.AccountID
                                 : confirmation.GroupID;
            string emailMessageFormat = InstanceConfig.Current.EmailOutputJoinMessageFormat;
            string message = String.Format(emailMessageFormat, informationOutput.OutputDescription,
                                           isAccount ? "account" : "collaboration group", ownerID, urlLink);
            string subject = String.Format(InstanceConfig.Current.EmailOutputJoinSubjectFormat, ownerID);
            foreach (string emailAddress in ownerEmailAddresses)
            {
                SendEmail(InstanceConfig.Current.EmailFromAddress, emailAddress, subject, message);
            }
        }

        public static void SendGroupAndPlatformJoinEmail(TBEmailValidation emailValidation, TBCollaboratingGroup collaboratingGroup)
        {
            string urlLink = GetUrlLink(emailValidation.ID);
            string emailMessageFormat = InstanceConfig.Current.EmailGroupAndPlatformJoinMessageFormat;
            string message = String.Format(emailMessageFormat, collaboratingGroup.Title, urlLink);
            SendEmail(InstanceConfig.Current.EmailFromAddress, emailValidation.Email,
                String.Format(InstanceConfig.Current.EmailGroupAndPlatformJoinSubjectFormat, collaboratingGroup.Title),
                      message);
        }

        public static async Task SendEmailValidationAsync(string emailAddress, string validationKey)
        {
            var emailMessageFormat = InstanceConfig.Current.EmailValidationCodeMessageFormat;
            var emailSubject = InstanceConfig.Current.EmailValidationCodeSubjectFormat;

            var emailMessage = String.Format(emailMessageFormat, emailAddress, validationKey);
            await SendEmailAsync(InstanceConfig.Current.EmailFromAddress, emailAddress, emailSubject, emailMessage);
        }
    }
}