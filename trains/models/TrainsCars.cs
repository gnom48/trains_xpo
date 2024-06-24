using DevExpress.Xpo;

namespace trains.models
{
    /// <summary>
    /// Класс TrainsCars
    /// представляет собой модель данных с информацией о состветствии между поездами-составами и вагонами
    /// </summary>
    public class TrainsCars: TrainsDbBaseXPObject
    {
        public TrainsCars(Session session): base(session) { }

        private int carPositionInTrain;

        /// <summary>
        /// Порядковый номер вагона в составе
        /// </summary>
        public int CarPositionInTrain
        {
            get
            {
                return carPositionInTrain;
            }
            set
            {
                SetPropertyValue<int>(nameof(CarPositionInTrain), ref carPositionInTrain, value);
            }
        }

        private Train train;

        [Association("Train-TrainsCars")]
        public Train Train
        {
            get
            {
                return train;
            }
            set
            {
                SetPropertyValue<Train>(nameof(Train), ref train, value);
            }
        }

        private Car car;

        [Association("Car-TrainsCars")]
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
    }
}
