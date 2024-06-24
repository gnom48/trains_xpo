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
    public class History: TrainsDbBaseXPObject
    {
        public History(Session session) : base(session) { }

        /// <summary>
        /// Идентификатор вагона
        /// </summary>
        public int CarId { get; set; }

        /// <summary>
        /// Идентификатор зафиксированной станции
        /// </summary>
        public int StationId { get; set; }

        /// <summary>
        /// Идентификатор операции
        /// </summary>
        public int OperationId { get; set; }

        /// <summary>
        /// Дата и время операции
        /// </summary>
        public DateTime OperationDateTime { get; set; }

        [Association("Car-Histories")]
        public Car Car { get; set; }

        [Association("Station-Histories")]
        public Station Station { get; set; }

        [Association("Operation-Histories")]
        public Operation Operation { get; set; }
    }
}
