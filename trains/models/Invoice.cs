using DevExpress.Xpo;
using System.Collections.Generic;

namespace trains.models
{
    /// <summary>
    /// Класс Invoice
    /// представляет собой модель данный с информацией о накладных листах
    /// </summary>
    public class Invoice: BaseXPObject
    {
        public Invoice(Session session) : base(session) { }
        public Invoice() : base() { }

        private string invoiceName;

        /// <summary>
        /// Номер накладного листа
        /// </summary>
        public string InvoiceName
        {
            get
            {
                return invoiceName;
            }
            set
            {
                SetPropertyValue<string>(nameof(InvoiceName), ref invoiceName, value);
            }
        }

        [Association("Invoice-Cars")]
        [Aggregated]
        public XPCollection<Car> Cars => GetCollection<Car>(nameof(Cars));
    }
}
