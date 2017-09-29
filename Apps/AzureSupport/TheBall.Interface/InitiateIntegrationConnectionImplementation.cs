using System.Threading.Tasks;
using TheBall.CORE;

namespace TheBall.Interface
{
    public class InitiateIntegrationConnectionImplementation
    {
        private static IContainerOwner Owner
        {
            get { return InformationContext.CurrentOwner; }
        }
        public static Connection GetTarget_Connection(string description)
        {
            Connection connection = new Connection();
            connection.SetLocationAsOwnerContent(Owner, connection.ID);
            connection.IsActiveParty = true;
            connection.Description = description;
            return connection;
        }

        public static AuthenticatedAsActiveDevice GetTarget_DeviceForConnection(string description, string targetBallHostName, string targetGroupId, Connection connection)
        {
            CreateAuthenticatedAsActiveDeviceParameters parameters = new CreateAuthenticatedAsActiveDeviceParameters
            {
                AuthenticationDeviceDescription = description,
                TargetBallHostName = targetBallHostName,
                TargetGroupID = targetGroupId,
                Owner = Owner,
            };
            var operResult = CreateAuthenticatedAsActiveDevice.Execute(parameters);
            connection.DeviceID = operResult.CreatedAuthenticatedAsActiveDevice.ID;
            return operResult.CreatedAuthenticatedAsActiveDevice;
        }

        public static async Task ExecuteMethod_StoreConnectionAsync(Connection connection)
        {
            await connection.StoreInformationAsync();
        }

        public static async Task ExecuteMethod_NegotiateDeviceConnectionAsync(AuthenticatedAsActiveDevice deviceForConnection)
        {
            PerformNegotiationAndValidateAuthenticationAsActiveDeviceParameters parameters =
                new PerformNegotiationAndValidateAuthenticationAsActiveDeviceParameters
                    {
                        AuthenticatedAsActiveDeviceID = deviceForConnection.ID,
                        Owner = Owner
                    };
            await PerformNegotiationAndValidateAuthenticationAsActiveDevice.ExecuteAsync(parameters);
        }

    }
}