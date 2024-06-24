using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace trains.models
{
    public class TrainsDbBaseXPObject: XPBaseObject
    {
        protected TrainsDbBaseXPObject(Session session): base(session) { }
    }
}
