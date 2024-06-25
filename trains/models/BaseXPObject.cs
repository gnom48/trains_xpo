using DevExpress.Xpo;

namespace trains.models
{
    /// <summary>
    /// Класс BaseXPObject
    /// является базовый объектом для всех моделей в проекте. 
    /// Абстрагирует модели от платформы XPO
    /// </summary>
    [NonPersistent]
    public class BaseXPObject: XPObject
    {
        protected BaseXPObject(Session session): base(session) { }

        protected BaseXPObject() { }
    }
}
