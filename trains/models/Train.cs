using DevExpress.Xpo;
using System.Collections.Generic;

namespace trains.models
{
    /// <summary>
    /// Класс Station
    /// представляет собой модель данных с информацией о поездах-составах
    /// </summary>
    public class Train: TrainsDbBaseXPObject
    {
        public Train(Session session) : base(session) { }

        /// <summary>
        /// Номер поезда
        /// </summary>
        public string TrainNumber { get; set; }

        /// <summary>
        /// Индекс состава
        /// </summary>
        public string TrainIndexCombined { get; set; }

        /// <summary>
        /// Идентификатор станции отправления
        /// </summary>
        public int FromStationId { get; set; }

        /// <summary>
        /// Идентификатор станции прибытия
        /// </summary>
        public int ToStationId { get; set; }

        [Association("Station-FromTrains")]
        public Station FromStation { get; set; }

        [Association("Station-ToTrains")]
        public Station ToStation { get; set; }

        [Association("Train-TrainsCars")]
        public XPCollection<TrainsCars> TrainsCars => GetCollection<TrainsCars>(nameof(TrainsCars));
    }
}
