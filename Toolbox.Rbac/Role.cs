using System;
using System.Collections.Generic;

namespace Toolbox.Rbac
{
    public class Role
    {
        public Role(string name)
        {
            Name = name;
            Actions = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);
        }
        public string Name { get; private set; }

        public ICollection<string> Actions { get; private set; }
    }

}
