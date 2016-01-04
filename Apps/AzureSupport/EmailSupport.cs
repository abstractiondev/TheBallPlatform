using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AaltoGlobalImpact.OIP;
using Amazon;
using Amazon.Runtime;
using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Blob;
using TheBall.CORE;
using TheBall.CORE.InstanceSupport;

namespace TheBall
{
    public static class EmailSupport
    {
        public static Boolean SendEmail(String From, String To, String Subject, String Text = null, String HTML = null, String emailReplyTo = null, String returnPath = null)
        {
            if (Text != null || HTML != null)
            {
                try
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

                    string awsAccessKey = SecureConfig.Current.AWSAccessKey;
                    string awsSecretKey = SecureConfig.Current.AWSSecretKey;
                    //AmazonSimpleEmailService ses = AWSClientFactory.CreateAmazonSimpleEmailServiceClient(AppConfig["AWSAccessKey"], AppConfig["AWSSecretKey"]);
                    AmazonSimpleEmailServiceClient ses = new AmazonSimpleEmailServiceClient(new BasicAWSCredentials(awsAccessKey, awsSecretKey), RegionEndpoint.EUWest1);

                    SendEmailRequest request = new SendEmailRequest();
                    request.Destination = destination;
                    request.Message = message;
                    request.Source = from;

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

                    SendEmailResponse response = ses.SendEmail(request);

                    Console.WriteLine("Email sent.");
                    Console.WriteLine(String.Format("Message ID: {0}",
                                                    response.MessageId));

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
                    QueueSupport.CurrStatisticsQueue.AddMessage(new CloudQueueMessage(queueMessage));
                }
            }

            Console.WriteLine("Specify Text and/or HTML for the email body!");

            return false;
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

        public static void SendInputJoinEmail(TBEmailValidation emailValidation, InformationInput informationInput, string[] ownerEmailAddresses)
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
                SendEmail(InstanceConfig.Current.EmailFromAddress, emailAddress, subject, message);
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
    }
}