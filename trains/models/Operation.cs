using DevExpress.Xpo;
using System.Collections.Generic;

namespace trains.models
{
    /// <summary>
    /// Класс Operation
    /// представляет собой модель данный с информацией об операциях над вагонами
    /// </summary>
    public class Operation: BaseXPObject
    {
        public Operation(Session session) : base(session) { }
        public Operation() { }

        private string operationName;

        /// <summary>
        /// Наименование операции
        /// </summary>
        public string OperationName 
        {
            get
            {
                return operationName;
            }
            set
            {
                SetPropertyValue<string>(nameof(OperationName), ref operationName, value);
            }
        }

        [Association("Operation-Histories")]
        public XPCollection<History> Histories => GetCollection<History>(nameof(Histories));
    }
}