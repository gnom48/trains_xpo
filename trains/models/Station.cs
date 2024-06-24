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
    public class Station: TrainsDbBaseXPObject
    {
        public Station(Session session) : base(session) { }

        /// <summary>
        /// Название станции
        /// </summary>
        public string StationName { get; set; }

        [Association("Station-Histories")]
        public XPCollection<History> Histories => GetCollection<History>(nameof(Histories));

        [Association("Station-FromTrains")]
        public XPCollection<Train> FromTrains => GetCollection<Train>(nameof(FromTrains));

        [Association("Station-ToTrains")]
        public XPCollection<Train> ToTrains => GetCollection<Train>(nameof(ToTrains));
    }
}
