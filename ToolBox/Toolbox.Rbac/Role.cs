using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toolbox.Rbac
{
    public class Role
    {
        public Role(string name)
        {
            this.Name = name;
            this.Actions = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);
        }
        public string Name { get; private set; }

        public ICollection<string> Actions { get; private set; }
    }

}
