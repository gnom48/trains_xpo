using System.Collections.Generic;
using System.Xml.Serialization;

namespace trains.models
{
    /// <summary>
    /// Класс Row
    /// представляет собой модель 1 записи из входных данных XML
    /// </summary>
    [XmlRoot(ElementName = "row")]
    public class Row
    {
        /// <summary>
        /// Номер поезда
        /// </summary>
        [XmlElement(ElementName = "TrainNumber")]
        public int TrainNumber { get; set; }

        /// <summary>
        /// Индекс состава
        /// </summary>
        [XmlElement(ElementName = "TrainIndexCombined")]
        public string TrainIndexCombined { get; set; }

        /// <summary>
        /// Станция отправления
        /// </summary>
        [XmlElement(ElementName = "FromStationName")]
        public string FromStationName { get; set; }

        /// <summary>
        /// Станция прибытия
        /// </summary>
        [XmlElement(ElementName = "ToStationName")]
        public string ToStationName { get; set; }

        /// <summary>
        /// Текущая (последняя зафиксированная) станция
        /// </summary>
        [XmlElement(ElementName = "LastStationName")]
        public string LastStationName { get; set; }

        /// <summary>
        /// Дата и время последней зафиксированной операции
        /// </summary>
        [XmlElement(ElementName = "WhenLastOperation")]
        public string WhenLastOperation { get; set; }

        /// <summary>
        /// Название последней зафиксированной операции
        /// </summary>
        [XmlElement(ElementName = "LastOperationName")]
        public string LastOperationName { get; set; }

        /// <summary>
        /// Номер накладной
        /// </summary>
        [XmlElement(ElementName = "InvoiceNum")]
        public string InvoiceNum { get; set; }

        /// <summary>
        /// Позиция вагона в составе
        /// </summary>
        [XmlElement(ElementName = "PositionInTrain")]
        public int PositionInTrain { get; set; }

        /// <summary>
        /// Номер вагона
        /// </summary>
        [XmlElement(ElementName = "CarNumber")]
        public int CarNumber { get; set; }

        /// <summary>
        /// Наименование груза
        /// </summary>
        [XmlElement(ElementName = "FreightEtsngName")]
        public string FreightEtsngName { get; set; }

        /// <summary>
        /// Масса брутто груза
        /// </summary>
        [XmlElement(ElementName = "FreightTotalWeightKg")]
        public int FreightTotalWeightKg { get; set; }
    }


    /// <summary>
    /// Класс Root
    /// представляет собой модель входных данных в формате XML
    /// для их программного представления
    /// </summary>
    [XmlRoot(ElementName = "Root")]
    public class Root
    {
        /// <summary>
        /// Записи в исходных данных
        /// </summary>
        [XmlElement(ElementName = "row")]
        public List<Row> Rows { get; set; }
    }
}
