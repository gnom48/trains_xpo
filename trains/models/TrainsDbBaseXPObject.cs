using DevExpress.Xpo;

namespace trains.models
{
    [NonPersistent]
    public class TrainsDbBaseXPObject: XPObject
    {
        protected TrainsDbBaseXPObject(Session session): base(session) { }
    }
}
