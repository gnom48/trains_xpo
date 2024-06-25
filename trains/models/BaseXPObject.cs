using DevExpress.Xpo;

namespace trains.models
{
    [NonPersistent]
    public class BaseXPObject: XPObject
    {
        protected BaseXPObject(Session session): base(session) { }
        public BaseXPObject() { }
    }
}
