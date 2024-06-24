using DevExpress.Xpo;
using System.Collections.Generic;

namespace trains.models
{
    /// <summary>
    /// Класс Invoice
    /// представляет собой модель данный с информацией о накладных листах
    /// </summary>
    public class Invoice: TrainsDbBaseXPObject
    {
        public Invoice(Session session) : base(session) { }

        /// <summary>
        /// Уникальный идентификатор накладного листа
        /// </summary>
        [Key(true)]
        public int InvoiceId { get; set; }

        /// <summary>
        /// Номер накладного листа
        /// </summary>
        public string InvoiceName { get; set; }

        [Association("Invoice-Cars")]
        public XPCollection<Car> Cars => GetCollection<Car>(nameof(Cars));
    }
}
