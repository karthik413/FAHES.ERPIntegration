using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FAHES.ERPIntegration.Inbound
{
    public class AppConfig
    {
        public string ApiUrl { get; set; }
        public string BearerToken { get; set; }
        public ConnectionStrings ConnectionStrings { get; set; }

        public string TenantId { get; set; }
        public string ClientId { get; set; }

        public string ClientSecret { get; set; }

        public string GrandType { get; set; }

        public string Scope { get; set; }

    }

    public class ConnectionStrings
    {
        public string DefaultConnection { get; set; }
    }
}
