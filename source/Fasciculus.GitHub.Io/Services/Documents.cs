using System.Collections.Generic;

namespace Fasciculus.GitHub.Services
{
    public class Documents : List<string>
    {
        public Documents()
        {
            AddGlobals();
        }

        private void AddGlobals()
        {
            Add("/");
            Add("/Privacy");
        }
    }
}
