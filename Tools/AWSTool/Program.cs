using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Amazon;
using Amazon.Route53;
using Amazon.Route53.Model;
using Amazon.Runtime;
using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;

namespace AWSTool
{
    public class AWSManager
    {
        private BasicAWSCredentials Credentials;

        public AWSManager(string accessKey, string secretKey)
        {
            Credentials = new BasicAWSCredentials(accessKey, secretKey);
        }

        public void InitDomainWithSES(string domainName)
        {
            string zoneId = initDNSZone(Credentials, domainName);
            validateSESDomain(domainName, zoneId);
        }

        private void validateSESDomain(string domainName, string zoneId)
        {
            AmazonSimpleEmailServiceClient client = new AmazonSimpleEmailServiceClient(Credentials, RegionEndpoint.EUWest1);
            var domainVerifyResponse = client.VerifyDomainIdentity(new VerifyDomainIdentityRequest()
            {
                Domain = domainName
            });
            var txtToVerify = domainVerifyResponse.VerificationToken;
            verifyTXTRecord(domainName, zoneId, "_amazonses", txtToVerify);

            var dkimStatus = client.GetIdentityDkimAttributes(new GetIdentityDkimAttributesRequest() { Identities = new List<string>(new string[] { domainName }) });
            var dkimAttributes = dkimStatus.DkimAttributes[domainName];
            if (dkimAttributes.DkimVerificationStatus != VerificationStatus.Success &&
                dkimAttributes.DkimVerificationStatus != VerificationStatus.Pending)
            {
                var dkimRecords = dkimAttributes.DkimTokens;
                dkimRecords.ForEach(dkim => verifyCNAMERecord(domainName, zoneId, dkim + "._domainkey", dkim + ".dkim.amazonses.com"));
                var dkimVerifyResponse = client.VerifyDomainDkim(new VerifyDomainDkimRequest()
                {
                    Domain = domainName
                });
                dkimStatus = client.GetIdentityDkimAttributes(new GetIdentityDkimAttributesRequest() { Identities = new List<string>(new string[] { domainName }) });
                dkimAttributes = dkimStatus.DkimAttributes[domainName];
            }
            if (dkimAttributes.DkimVerificationStatus == VerificationStatus.Success)
            {
                if (dkimAttributes.DkimEnabled == false)
                {
                    var dkimResult = client.SetIdentityDkimEnabled(new SetIdentityDkimEnabledRequest()
                    {
                        DkimEnabled = true,
                        Identity = domainName
                    });
                }
            }

        }

        private void verifyCNAMERecord(string domainName, string zoneId, string cName, string targetName)
        {
            AmazonRoute53Client client = new AmazonRoute53Client(Credentials, RegionEndpoint.EUWest1);
            var changes = new List<Change>()
            {
                new Change(ChangeAction.UPSERT, new ResourceRecordSet()
                {
                    Name = cName + "." + domainName,
                    Type = RRType.CNAME,
                    ResourceRecords = new List<ResourceRecord>() { new ResourceRecord(targetName) },
                    TTL = 300,
                })
            };
            var response = client.ChangeResourceRecordSets(new ChangeResourceRecordSetsRequest(
                zoneId, new ChangeBatch(changes)));
            var changeResult = response.ChangeInfo;
        }

        private void verifyTXTRecord(string domainName, string zoneId, string recordName, string txtToVerify)
        {
            AmazonRoute53Client client = new AmazonRoute53Client(Credentials, RegionEndpoint.EUWest1);
            string verifyRecordName = recordName + "." + domainName;
            var changes = new List<Change>()
            {
                new Change(ChangeAction.UPSERT, new ResourceRecordSet()
                {
                    Name = verifyRecordName,
                    Type = RRType.TXT,
                    ResourceRecords = new List<ResourceRecord>() { new ResourceRecord( "\"" + txtToVerify + "\"") },
                    TTL = 300,
                })
            };
            var response = client.ChangeResourceRecordSets(new ChangeResourceRecordSetsRequest(
                zoneId, new ChangeBatch(changes)));
            var changeResult = response.ChangeInfo;
        }

        private string initDNSZone(BasicAWSCredentials credentials, string domainName)
        {
            AmazonRoute53Client client = new AmazonRoute53Client(credentials, RegionEndpoint.EUWest1);
            HostedZone hostedZone = null;

            var listResponse = client.ListHostedZonesByName(new ListHostedZonesByNameRequest { DNSName = domainName });
            //var response = client.GetHostedZone(new GetHostedZoneRequest() { });
            if (listResponse.HostedZones.Count > 0)
                hostedZone = listResponse.HostedZones[0];
            else
            {
                var response = client.CreateHostedZone(new CreateHostedZoneRequest()
                {
                    Name = domainName,
                    CallerReference = new Guid().ToString(),
                    //HostedZoneConfig = new HostedZoneConfig() { Comment = "The Ball Managed Zone",  PrivateZone = false}
                });
                hostedZone = response.HostedZone;
            }
            var zoneRecords = client.ListResourceRecordSets(new ListResourceRecordSetsRequest(hostedZone.Id));
            return hostedZone.Id;
        }

    }

    class Program
    {
        static void Main(string[] args)
        {
            string accessKey = args[0];
            string secretKey = args[1];

            AWSManager manager = new AWSManager(accessKey, secretKey);
            manager.InitDomainWithSES("theball.me");
        }

    }
}
