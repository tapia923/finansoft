using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Web;

namespace PlanillajeColectivos.Tools
{
    public class CustomSecurityTokenManager: ClientCredentialsSecurityTokenManager
    {
        public CustomSecurityTokenManager(CustomCredentials cred)
        : base(cred)
        { }

        public override System.IdentityModel.Selectors.SecurityTokenSerializer CreateSecurityTokenSerializer(System.IdentityModel.Selectors.SecurityTokenVersion version)
        {
            return new CustomTokenSerializer(System.ServiceModel.Security.SecurityVersion.WSSecurity11);
        }
    }
}