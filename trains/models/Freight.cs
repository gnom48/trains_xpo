using DevExpress.Xpo;
using System.Collections.Generic;

namespace trains.models
{
    /// <summary>
    /// Класс Freight
    /// представляет собой модель данный с информацией о грузах
    /// </summary>
    public class Freight: TrainsDbBaseXPObject
    {
        public Freight(Session session) : base(session) { }

        /// <summary>
        /// Наименование груза
        /// </summary>
        public string FreightName
        {
            get
            {
                return GetPropertyValue<string>(nameof(FreightName));
            }
            set
            {
                SetPropertyValue<string>(nameof(FreightName), value);
            }
        }

        [Association("Freight-Cars")]
        public XPCollection<Car> Cars => GetCollection<Car>(nameof(Cars));
    }
}
