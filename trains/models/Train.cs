using DevExpress.Xpo;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using System.Collections.Generic;

namespace trains.models
{
    /// <summary>
    /// Класс Station
    /// представляет собой модель данных с информацией о поездах-составах
    /// </summary>
    public class Train: BaseXPObject
    {
        public Train(Session session) : base(session) { }
        public Train() { }

        /// <summary>
        /// Номер поезда
        /// </summary>
        public string TrainNumber
        {
            get
            {
                return GetPropertyValue<string>(nameof(TrainNumber));
            }
            set
            {
                SetPropertyValue<string>(nameof(TrainNumber), value);
            }
        }

        /// <summary>
        /// Индекс состава
        /// </summary>
        public string TrainIndexCombined
        {
            get
            {
                return GetPropertyValue<string>(nameof(TrainIndexCombined));
            }
            set
            {
                SetPropertyValue<string>(nameof(TrainIndexCombined), value);
            }
        }

        private Station fromStation;

        [Association("Station-FromTrains")]
        public Station FromStation
        {
            get
            {
                return fromStation;
            }
            set
            {
                SetPropertyValue<Station>(nameof(Station), ref fromStation, value);
            }
        }

        private Station toStation;

        [Association("Station-ToTrains")]
        public Station ToStation
        {
            get
            {
                return toStation;
            }
            set
            {
                SetPropertyValue<Station>(nameof(Station), ref toStation, value);
            }
        }

        [Association("Train-TrainsCars")]
        public XPCollection<TrainsCars> TrainsCars_ => GetCollection<TrainsCars>(nameof(TrainsCars_));
    }
}
