using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SongCollector
{
    public class NameAddEvent : EventArgs
    {
        public string Name {  get; private set; }
        public NameAddEvent(string name)
        {
            this.Name = name;
        }
    }
}
