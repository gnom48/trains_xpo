using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace trains.models
{
    /// <summary>
    /// Класс Station
    /// представляет собой модель данных с информацией о железнодорожных станциях
    /// </summary>
    public class Station: BaseXPObject
    {
        public Station(Session session) : base(session) { }
        public Station() : base() { }

        private string stationName;

        /// <summary>
        /// Название станции
        /// </summary>
        public string StationName
        {
            get
            {
                return stationName;
            }
            set
            {
                SetPropertyValue<string>(nameof(StationName), ref stationName, value);
            }
        }

        [Association("Station-Histories")]
        [Aggregated]
        public XPCollection<History> Histories => GetCollection<History>(nameof(Histories));

        [Association("Station-FromTrains")]
        [Aggregated]
        public XPCollection<Train> FromTrains => GetCollection<Train>(nameof(FromTrains));

        [Association("Station-ToTrains")]
        [Aggregated]
        public XPCollection<Train> ToTrains => GetCollection<Train>(nameof(ToTrains));
    }
}
