using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commander.Core
{
    public interface ICommandHandler<T>
    {
        void Handle(T command);
    }
}
