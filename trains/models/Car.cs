using DevExpress.Xpo;
using System.Collections.Generic;

namespace trains.models
{
    /// <summary>
    /// Класс Car
    /// представляет собой модель данный с информацией о вагонах
    /// </summary>
    public class Car: TrainsDbBaseXPObject
    {
        public Car(Session session) : base(session) { }

        /// <summary>
        /// Номер вагона
        /// </summary>
        public string CarNumber
        {
            get
            {
                return GetPropertyValue<string>(nameof(CarNumber));
            }
            set
            {
                SetPropertyValue<string>(nameof(CarNumber), value);
            }
        }

        /*
        /// <summary>
        /// Идентификатор накладного листа 
        /// </summary>
        public int InvoiceId
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

        /// <summary>
        /// Идентификатор груза
        /// </summary>
        public int FreightId
        {
            get
            {
                return GetPropertyValue<string>(nameof(FreightName));
            }
            set
            {
                SetPropertyValue<string>(nameof(FreightName), value);
            }
        }*/

        /// <summary>
        /// Масса брутто
        /// </summary>
        public int GrossWeight
        {
            get
            {
                return GetPropertyValue<int>(nameof(GrossWeight));
            }
            set
            {
                SetPropertyValue<int>(nameof(GrossWeight), value);
            }
        }

        [Association("Car-TrainsCars")]
        public XPCollection<TrainsCars> TrainsCars => GetCollection<TrainsCars>(nameof(TrainsCars));

        [Association("Invoice-Cars")]
        public Invoice Invoice { get; set; }

        [Association("Freight-Cars")]
        public Freight Freight { get; set; }

        [Association("Car-Histories")]
        public XPCollection<History> Histories => GetCollection<History>(nameof(Histories));
    }
}
