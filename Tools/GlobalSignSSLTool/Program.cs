using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using GlobalSignSSLTool.GlobalSignSSLSvc;

namespace GlobalSignSSLTool
{
    class Program
    {
        static void Main(string[] args)
        {
            string userName = args[0];
            string passWord = args[1];

            GlobalSignSSLSvc.ServerSSLV1Client client = new ServerSSLV1Client();
            DVDNSOrder order = new DVDNSOrder();
            order.Request = new QbV1DvDnsOrderRequest();
            order.Request.ContactInfo = new ContactInfo()
            {
                Email = "kalle.launiala@protonit.net",
                FirstName = "Kalle",
                LastName = "Launiala",
                Phone = "+358445575665"
            }; 
            order.Request.SecondContactInfo = new SecondContactInfo
            {
                Email = "",
                FirstName = "",
                LastName = "",
            };
            order.Request.OrderRequestParameter = new OrderRequestParameter
            {
                ProductCode = "DV_LOW_DNS",
                BaseOption = "wildcard",
                OrderKind = "new",
                Licenses = "1",
                ValidityPeriod = new ValidityPeriod { Months = "12" },
                CSR =
@"-----BEGIN NEW CERTIFICATE REQUEST-----
MIIEajCCA1ICAQAwgYMxCzAJBgNVBAYTAkZJMRAwDgYDVQQIDAdub3QgYW55MRIw
EAYDVQQHDAlUZXN0IENpdHkxGjAYBgNVBAoMEVRlc3Qgb3JnYW5pc2F0aW9uMRIw
EAYDVQQLDAlUZXN0IHVuaXQxHjAcBgNVBAMMFXRlc3RyZXEubG9jYWxob3N0Lm5v
dDCCASIwDQYJKoZIhvcNAQEBBQADggEPADCCAQoCggEBAMt/1ROuule5weBxVsNU
Y6fuaudTYta+evd65IpWI7Rjvk7mc+IhjhAitJKtboQ3zrT8NnFJFcJclJBaesxd
sFe3QGf6YkVlvXOPl7+n3d/BadOpe1etqPTBbCsu6l9JBZ0zyMehEr9SUaKRZdMO
p2aILa0vc4deAk6jBQxaR7V+BL5MGfETgW0oLPriahMTGGksOj3S9H4RrPlRJV/W
5Rv5DzJ4Thn3ub9fF1173M9m6MIGFYqwRhQEt8ONkQNkIwRx7TxvPcVa40pFDY9H
sHpv9HyP1mhuxnF5A8eanzdRHClHX3yWMgkXsBK9hN2ANW1867h4zur4Uwe24u8C
DlUCAwEAAaCCAZ8wHAYKKwYBBAGCNw0CAzEOFgwxMC4wLjEwMjQwLjIwOQYJKwYB
BAGCNxUUMSwwKgIBBQwIdGhlYmFsbDQMDlRIRUJBTEw0XEthbGxlDAtJbmV0TWdy
LmV4ZTByBgorBgEEAYI3DQICMWQwYgIBAR5aAE0AaQBjAHIAbwBzAG8AZgB0ACAA
UgBTAEEAIABTAEMAaABhAG4AbgBlAGwAIABDAHIAeQBwAHQAbwBnAHIAYQBwAGgA
aQBjACAAUAByAG8AdgBpAGQAZQByAwEAMIHPBgkqhkiG9w0BCQ4xgcEwgb4wDgYD
VR0PAQH/BAQDAgTwMBMGA1UdJQQMMAoGCCsGAQUFBwMBMHgGCSqGSIb3DQEJDwRr
MGkwDgYIKoZIhvcNAwICAgCAMA4GCCqGSIb3DQMEAgIAgDALBglghkgBZQMEASow
CwYJYIZIAWUDBAEtMAsGCWCGSAFlAwQBAjALBglghkgBZQMEAQUwBwYFKw4DAgcw
CgYIKoZIhvcNAwcwHQYDVR0OBBYEFINL49lFkKAaTF17vUp8PuYF3lnuMA0GCSqG
SIb3DQEBBQUAA4IBAQBVKPU1oZ3oKrcPlPciNKnZ9Wf23A2FwT15pBOlAg3T5byk
2VD6I9iueQVM1OL+nek/X7OQZk7eJ3cnv6yiJzSE5Np8QyzAjRXiPZQAntZgiOSR
XbEZt4qVsgSHH3m5nSEuNwRmKu73GkeUQ+1a8cykdg5fCs8fWsl8c/scu/Zoz9Vp
tNEBNZ2DsVg6iiOoAdLMex91gt/6gbr9jdCrCifKzW5jAek2stPLXekLeyqUIYys
DZgxn5TF32NdJ5H3i8qZx9RnFN4Lv/HbgploRfYFgnY51WdYZJadow447zyLHsdm
U84GnPPAJbWQfdmwftUfeXtdUUcn5Tbq8bDlezcs
-----END NEW CERTIFICATE REQUEST-----"
            };
            order.Request.OrderRequestHeader = new OrderRequestHeader
            {
                AuthToken = new AuthToken {UserName = userName, Password = passWord}
            };

            var response = client.DVDNSOrder(order.Request);

        }
    }
}
