using DevExpress.Xpo;
using System.Collections.Generic;

namespace trains.models
{
    /// <summary>
    /// Класс Car
    /// представляет собой модель данный с информацией о вагонах
    /// </summary>
    public class Car: BaseXPObject
    {
        public Car(Session session) : base(session) { }
        public Car() { }

        private string carNumber;

        /// <summary>
        /// Номер вагона
        /// </summary>
        public string CarNumber
        {
            get
            {
                return carNumber;
            }
            set
            {
                SetPropertyValue<string>(nameof(CarNumber), ref carNumber, value);
            }
        }

        private int grossWeight;

        /// <summary>
        /// Масса брутто
        /// </summary>
        public int GrossWeight
        {
            get
            {
                return grossWeight;
            }
            set
            {
                SetPropertyValue<int>(nameof(GrossWeight), ref grossWeight, value);
            }
        }

        [Association("Car-TrainsCars")]
        public XPCollection<TrainsCars> TrainsCars_ => GetCollection<TrainsCars>(nameof(TrainsCars_));

        private Invoice invoice;

        [Association("Invoice-Cars")]
        public Invoice Invoice
        {
            get
            {
                return invoice;
            }
            set
            {
                SetPropertyValue<Invoice>(nameof(Invoice), ref invoice, value);
            }
        }

        private Freight freight;

        [Association("Freight-Cars")]
        public Freight Freight
        {
            get
            {
                return freight;
            }
            set
            {
                SetPropertyValue<Freight>(nameof(Freight), ref freight, value);
            }
        }

        [Association("Car-Histories")]
        public XPCollection<History> Histories => GetCollection<History>(nameof(Histories));
    }
}
