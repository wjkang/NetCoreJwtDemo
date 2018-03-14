using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetCoreJwtDemo.Session
{
    public class ClaimsAbpSession : AbpSessionBase
    {
        protected IPrincipalAccessor PrincipalAccessor { get; }

        public ClaimsAbpSession(IPrincipalAccessor principalAccessor)
        {
            PrincipalAccessor = principalAccessor;
            Console.WriteLine("Session:" + this.GetHashCode().ToString());
        }

        public override long? UserId
        {
            get
            {
                var userIdClaim = PrincipalAccessor.Principal?.Claims.FirstOrDefault(c => c.Type == AbpClaimTypes.UserId);
                if (string.IsNullOrEmpty(userIdClaim?.Value))
                {
                    return null;
                }

                long userId;
                if (!long.TryParse(userIdClaim.Value, out userId))
                {
                    return null;
                }

                return userId;
            }
        }

        public override string UserName
        {
            get
            {
                var userNameClaim = PrincipalAccessor.Principal?.Claims.FirstOrDefault(c => c.Type == AbpClaimTypes.UserName);
                if (string.IsNullOrEmpty(userNameClaim?.Value))
                {
                    return null;
                }
                 return userNameClaim.Value;
            }
        }
    }
}
