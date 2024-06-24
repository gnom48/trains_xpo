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
        public XPCollection<Car> Cars => GetCollection<Car>(nameof(Cars));
    }
}
