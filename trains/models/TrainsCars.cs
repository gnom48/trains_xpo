using DevExpress.Xpo;

namespace trains.models
{
    /// <summary>
    /// Класс TrainsCars
    /// представляет собой модель данных с информацией о состветствии между поездами-составами и вагонами
    /// </summary>
    public class TrainsCars: TrainsDbBaseXPObject
    {
        public TrainsCars(Session session): base(session) {}

        /// <summary>
        /// Идентификатор поезда
        /// </summary>
        [Key(true)]
        public int TrainId { get; set; }

        /// <summary>
        /// Идентификатор вагона
        /// </summary>
        [Key(true)]
        public int CarId { get; set; }

        /// <summary>
        /// Порядковый номер вагона в составе
        /// </summary>
        public int CarPositionInTrain { get; set; }

        [Association("Train-TrainsCars")]
        public Train Train { get; set; }

        [Association("Car-TrainsCars")]
        public Car Car { get; set; }
    }
}
