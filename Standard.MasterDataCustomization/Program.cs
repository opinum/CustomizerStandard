using MasterDataCustomization;
using System.Collections.Generic;

namespace Standard.MasterDataCustomization
{
    class Program
    {
        static void Main(string[] args)
        {
            new MasterDataCustomizer()
                .WithArguments(args)
                //Default values if no argument is provided (running in test/debug mode)

                .WithDefaultOpisenseApiUrl("https://api.opinum.com/")
                .WithDefaultOpisenseIdentityUrl("https://identity.opinum.com/")

                .WithDefaultAdditionalProperties(new { Id = 227077 })

                //Authentication infos
                .WithClientId("__PUT_YOUR_CLIENT_ID_HERE__")
                .WithClientSecret("__PUT_YOUR_CLIENT_SECRET_HERE__")
                .WithUsername("__PUT_YOUR_USER_NAME_HERE__")
                .WithPassword("__PUT_YOUR_USER_PASSWORD_HERE__")
                .WithScopes("opisense-api")

                //Allow to bypass whatif mode and to force the execution
                .WithWhatifBypass(false)

                //What  to execute
                .WithCustomizationMethod(StandardVariablesManager.UpdateStandardVariablesConfiguration)

                //Fire!
                .Customize().Wait();
        }
    }
}
