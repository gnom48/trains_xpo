using DevExpress.Xpo;
using System.Collections.Generic;

namespace trains.models
{
    /// <summary>
    /// Класс Operation
    /// представляет собой модель данный с информацией об операциях над вагонами
    /// </summary>
    public class Operation: TrainsDbBaseXPObject
    {
        public Operation(Session session) : base(session) { }

        /// <summary>
        /// Наименование операции
        /// </summary>
        public string OperationName 
        {
            get
            {
                return GetPropertyValue<string>(nameof(OperationName));
            }
            set
            {
                SetPropertyValue<string>(nameof(OperationName), value);
            }
        }

        [Association("Operation-Histories")]
        public XPCollection<History> Histories => GetCollection<History>(nameof(Histories));
    }
}