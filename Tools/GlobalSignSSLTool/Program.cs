using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using GlobalSignSSLTool.GlobalSignSSLSvc;
using GlobalSignSSLTool.GlobalSignGASSvc;
using SSL = GlobalSignSSLTool.GlobalSignSSLSvc;
using GAS = GlobalSignSSLTool.GlobalSignGASSvc;
using SSLAuthToken = GlobalSignSSLTool.GlobalSignSSLSvc.AuthToken;
using GASAuthToken = GlobalSignSSLTool.GlobalSignGASSvc.AuthToken;

namespace GlobalSignSSLTool
{
    class Program
    {
        static void Main(string[] args)
        {
            string userName = args[0];
            string passWord = args[1];

            // https://system.globalsign.com/kb/ws/v1/ServerSSLService
            // https://testsystem.globalsign.com/kb/ws/v1/ServerSSLService
            ServerSSLV1Client client = new ServerSSLV1Client("ServerSSLV1Port",
                "https://testsystem.globalsign.com/kb/ws/v1/ServerSSLService");

            // https://system.globalsign.com/kb/ws/v1/GASService
            // https://testsystem.globalsign.com/kb/ws/v1/GASService
            GASV1Client gasClient = new GASV1Client("GASV1Port",
                "https://testsystem.globalsign.com/kb/ws/v1/GASService");


            GASAuthToken gasAuthToken = new GASAuthToken { UserName = userName, Password = passWord };
            SSLAuthToken sslAuthToken = new SSLAuthToken { UserName = userName, Password = passWord };

            string csr = @"-----BEGIN NEW CERTIFICATE REQUEST-----
MIIETzCCAzcCAQAwaTELMAkGA1UEBhMCRkkxDDAKBgNVBAgMA24vYTEQMA4GA1UE
BwwHVGFtcGVyZTETMBEGA1UECgwKRGlvcnkgTHRkLjELMAkGA1UECwwCSVQxGDAW
BgNVBAMMDyouZGlvc3BoZXJlLm9yZzCCASIwDQYJKoZIhvcNAQEBBQADggEPADCC
AQoCggEBAKQHuuH2Um99s37cc5z2wKNQb48sL1qVzYJS7ZfmdCnwSFwAKNkyRo4a
DGVcZXHxr50RLLucfuNXyYGjqze6oKkL2V07T2rYGMJrW/j02Tx9MF2gUSGI84kN
TnoJMrTBHwfnReBWHnfltqeGPQSHYI71NqNMb0MUneac+izdToUF/uW1Q9N4Ewei
GJQDa/Ve23D3qLlsF1p5lHGQE/yPBj4bcfx6FQD5Sw9TddMeK2RWGcvEBi7Wl5Tt
asVr1GT/uJTtWbhhPkLQKLaP+3BMzKKbFwNVOXI8Td78JeoLeGCQiev/FhUjdlXN
1IudE1oArWztqVLpQlE0mG83QwP1TI8CAwEAAaCCAZ8wHAYKKwYBBAGCNw0CAzEO
FgwxMC4wLjEwMjQwLjIwOQYJKwYBBAGCNxUUMSwwKgIBBQwIdGhlYmFsbDQMDlRI
RUJBTEw0XEthbGxlDAtJbmV0TWdyLmV4ZTByBgorBgEEAYI3DQICMWQwYgIBAR5a
AE0AaQBjAHIAbwBzAG8AZgB0ACAAUgBTAEEAIABTAEMAaABhAG4AbgBlAGwAIABD
AHIAeQBwAHQAbwBnAHIAYQBwAGgAaQBjACAAUAByAG8AdgBpAGQAZQByAwEAMIHP
BgkqhkiG9w0BCQ4xgcEwgb4wDgYDVR0PAQH/BAQDAgTwMBMGA1UdJQQMMAoGCCsG
AQUFBwMBMHgGCSqGSIb3DQEJDwRrMGkwDgYIKoZIhvcNAwICAgCAMA4GCCqGSIb3
DQMEAgIAgDALBglghkgBZQMEASowCwYJYIZIAWUDBAEtMAsGCWCGSAFlAwQBAjAL
BglghkgBZQMEAQUwBwYFKw4DAgcwCgYIKoZIhvcNAwcwHQYDVR0OBBYEFEQ7j3w5
aq9FpNil4wQTeo6qpM+YMA0GCSqGSIb3DQEBBQUAA4IBAQBnt/6ikLB87AKtu7GO
eobABi+9eTAbvyGm76ViVFw8Yrh0kx6tzmK0+OeHTPYc+tqCFoAsRemSddPyq308
VxttbX98fbQnyJp0q4Hqa4C6+1awbslSv57+WFENpXhx5ELAvFAg241BlJR3KOQH
Av3fjFPjchNfecF8uHlUVo4br8i2wrTcowtnlwls1yAb/fG+mI/wXc0/cRjnYDKT
mrrMSG6H5pmVTc9uFjW5rzFITvFRUAb1VAdF0Xov4yxC/k7yhzj8Z/C3v7nTA6XI
Lkv6WVyjAul1uswGIotte5i5GPBwPwJNtT5qOx7GEub55SHQoJJy80yFR39+mQP9
UVKM
-----END NEW CERTIFICATE REQUEST-----";

            DecodeCSR decodeCsr = CreateDecodeCSR(csr, gasAuthToken);
            var gasCSRResponse = gasClient.DecodeCSR(decodeCsr.Request);

            /*
            var commonName = gasCSRResponse.CSRData.CommonName;
            string fqdn = commonName.Substring(commonName.IndexOf("."));
            var getDVApproverList = CreateGetDVApproverList(fqdn, gasAuthToken);
            */

            string orderID = null;
            var order = CreateDvdnsOrder(csr, orderID, sslAuthToken);
            var sslResponse = client.DVDNSOrder(order.Request);

        }

        private static GetDVApproverList CreateGetDVApproverList(string fqdn, GASAuthToken gasAuthToken)
        {
            var getDVAApproverList = new GetDVApproverList
            {
                Request = new QbV1GetDVApproverListRequest
                {
                    //FQDN = 
                }
            };
            return getDVAApproverList;
        }

        private static DecodeCSR CreateDecodeCSR(string csr, GASAuthToken gasAuthToken)
        {
            var decodeCSR = new GAS.DecodeCSR
            {
                Request = new QbV1DecodeCsrRequest
                {
                    CSR = csr,
                    ProductType = "DV_LOW",
                    QueryRequestHeader = new GAS.QueryRequestHeader
                    {
                        AuthToken = gasAuthToken
                    }
                }
            };
            return decodeCSR;
        }

        private static DVDNSOrder CreateDvdnsOrder(string csr, string orderID, SSLAuthToken authToken)
        {
            DVDNSOrder order = new DVDNSOrder
            {
                Request = new QbV1DvDnsOrderRequest
                {
                    ContactInfo = new SSL.ContactInfo()
                    {
                        Email = "kalle.launiala@protonit.net",
                        FirstName = "Kalle",
                        LastName = "Launiala",
                        Phone = "+358445575665"
                    },
                    SecondContactInfo = new SSL.SecondContactInfo
                    {
                        Email = "kalle.launiala@protonit.net",
                        FirstName = "Kalle",
                        LastName = "Launiala",
                    },
                    OrderRequestParameter = new SSL.OrderRequestParameter
                    {
                        ProductCode = "DV_LOW_DNS",
                        BaseOption = "wildcard",
                        OrderKind = "new",
                        Licenses = "1",
                        ValidityPeriod = new SSL.ValidityPeriod {Months = "12"},
                        CSR = csr,
                    },
                    OrderRequestHeader = new SSL.OrderRequestHeader
                    {
                        AuthToken = authToken
                    },
                }
            };
            return order;
        }
    }
}
