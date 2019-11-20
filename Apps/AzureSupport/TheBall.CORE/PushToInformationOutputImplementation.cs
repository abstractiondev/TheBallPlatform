using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Security;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web;

namespace TheBall.Core
{
    public class PushToInformationOutputImplementation
    {
        public static async Task<InformationOutput> GetTarget_InformationOutputAsync(IContainerOwner owner, string informationOutputId)
        {
            return await ObjectStorage.RetrieveFromOwnerContentA<InformationOutput>(owner, informationOutputId);
        }

        public static void ExecuteMethod_VerifyValidOutput(InformationOutput informationOutput)
        {
            if (informationOutput.IsValidatedAndActive == false)
                throw new SecurityException("InformationOutput is not active");
        }

        public static string GetTarget_DestinationURL(InformationOutput informationOutput)
        {
            return informationOutput.DestinationURL;
        }

        public static string GetTarget_LocalContentURL(string localContentName, InformationOutput informationOutput)
        {
            var invalidFilenameChars = Path.GetInvalidFileNameChars();
            bool hasInvalidFilenameCharacter = localContentName.Any(invalidFilenameChars.Contains);
            if(hasInvalidFilenameCharacter)
                throw new ArgumentException("Invalid filename character in localContentName: " + localContentName, "localContentName");
            bool requiresLocalName = informationOutput.LocalContentURL.EndsWith("/");
            bool hasLocalContentName = String.IsNullOrEmpty(localContentName) == false;
            if (requiresLocalName)
            {
                if(String.IsNullOrEmpty(localContentName))
                    throw new ArgumentException("Valid argument missing for localContentName", "localContentName");
                return informationOutput.LocalContentURL + localContentName;
            } else if(hasLocalContentName)
                throw new ArgumentException("InformationOutput LocalContentUrl needs to end to / to support localContentName");
            return informationOutput.LocalContentURL;
        }

        public static async Task<AuthenticatedAsActiveDevice> GetTarget_AuthenticatedAsActiveDeviceAsync(InformationOutput informationOutput)
        {
            var authenticationID = informationOutput.AuthenticatedDeviceID;
            if (string.IsNullOrEmpty(authenticationID))
                return null;
            var owner = VirtualOwner.FigureOwner(informationOutput.RelativeLocation);
            return await ObjectStorage.RetrieveFromOwnerContentA<AuthenticatedAsActiveDevice>(owner, authenticationID);
        }

        public static async Task ExecuteMethod_PushToInformationOutputAsync(IContainerOwner owner, InformationOutput informationOutput, string destinationUrl, string destinationContentName, string localContentUrl, AuthenticatedAsActiveDevice authenticatedAsActiveDevice)
        {
            if(authenticatedAsActiveDevice == null)
                throw new NotSupportedException("Push not currently supported without authenticated as device connection");
            await DeviceSupport.PushContentToDevice(authenticatedAsActiveDevice, localContentUrl, destinationContentName);
        }

        public static string GetTarget_DestinationContentName(string specificDestinationContentName, InformationOutput informationOutput)
        {
            string destinationContentName = string.IsNullOrEmpty(specificDestinationContentName) ?
                                                informationOutput.DestinationContentName : specificDestinationContentName;
            if (string.IsNullOrEmpty(destinationContentName))
                return "bulkdump.all";
            return destinationContentName;
        }
    }
}