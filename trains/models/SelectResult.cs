using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace trains.models
{
    /// <summary>
    /// Класс SelectResult
    /// является результатом объединения и выборки данных
    /// </summary>
    public class SelectResult
    {
        /// <summary>
        /// Номер поезда
        /// </summary>
        public string TrainNumber { get; set; }

        /// <summary>
        /// Индекс состава
        /// </summary>
        public string TrainIndexCombined { get; set; }

        /// <summary>
        /// Дата и время последней зафиксированной операции
        /// </summary>
        public DateTime LastOperationDateTime { get; set; }

        /// <summary>
        /// Название последней зафиксированной операции
        /// </summary>
        public string OperationName { get; set; }
        
        /// <summary>
        /// Название последней зафиксированной станции
        /// </summary>
        public string StationName { get; set; }

        /// <summary>
        /// Номер накладной
        /// </summary>
        public string InvoiceName { get; set; }

        /// <summary>
        /// Позиция вагона в составе
        /// </summary>
        public int CarPositionInTrain { get; set; }

        /// <summary>
        /// Номер вагона
        /// </summary>
        public string CarNumber { get; set; }

        /// <summary>
        /// Наименование груза
        /// </summary>
        public string FreightName { get; set; }


        /// <summary>
        /// Масса брутто груза
        /// </summary>
        public int GrossWeight { get; set; }
    }
}
