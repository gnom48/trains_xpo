using DevExpress.Xpo;
using System.Collections.Generic;

namespace trains.models
{
    /// <summary>
    /// Класс Freight
    /// представляет собой модель данный с информацией о грузах
    /// </summary>
    public class Freight: BaseXPObject
    {
        public Freight(Session session) : base(session) { }
        public Freight(): base() { }

        private string freightName;

        /// <summary>
        /// Наименование груза
        /// </summary>
        public string FreightName
        {
            get
            {
                return freightName;
            }
            set
            {
                SetPropertyValue<string>(nameof(FreightName), ref freightName, value);
            }
        }

        [Association("Freight-Cars")]
        public XPCollection<Car> Cars => GetCollection<Car>(nameof(Cars));
    }
}
