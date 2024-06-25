using DevExpress.Xpo;
using System;
using System.ComponentModel.DataAnnotations;
using AssociationAttribute = DevExpress.Xpo.AssociationAttribute;
using KeyAttribute = DevExpress.Xpo.KeyAttribute;

namespace trains.models
{
    /// <summary>
    /// Класс History
    /// представляет собой модель данный с информацией о истории операций и перемещения вагонов по станциям
    /// </summary>
    public class History: BaseXPObject
    {
        public History(Session session) : base(session) { }
        public History() { }

        private DateTime operationDateTime;

        /// <summary>
        /// Дата и время операции
        /// </summary>
        public DateTime OperationDateTime 
        { 
            get 
            {
                return operationDateTime;
            } 
            set 
            {
                SetPropertyValue<DateTime>(nameof(OperationDateTime), ref operationDateTime, value);
            }
        }

        private Car car;

        [Association("Car-Histories")]
        public Car Car
        {
            get
            {
                return car;
            }
            set
            {
                SetPropertyValue<Car>(nameof(Car), ref car, value);
            }
        }

        private Station station;

        [Association("Station-Histories")]
        public Station Station
        {
            get
            {
                return station;
            }
            set
            {
                SetPropertyValue<Station>(nameof(Station), ref station, value);
            }
        }

        private Operation operation;

        [Association("Operation-Histories")]
        public Operation Operation
        {
            get
            {
                return operation;
            }
            set
            {
                SetPropertyValue<Operation>(nameof(Operation), ref operation, value);
            }
        }
    }
}
