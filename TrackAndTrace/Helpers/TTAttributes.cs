using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackAndTrace.Helpers
{
    [System.AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public sealed class AuthorizeAttribute : Attribute
    {
        // See the attribute guidelines at 
        //  http://go.microsoft.com/fwlink/?LinkId=85236
        readonly string positionalString;

        public AuthorizeAttribute(params object[] role)
        {
            if (role.Any(r => r.GetType().BaseType != typeof(Enum)))
                throw new ArgumentException("Tip role nije validan!");
        }

    }
}
