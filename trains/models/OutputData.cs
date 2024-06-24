using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace trains.models
{
    /// <summary>
    /// Класс OutputData
    /// представляет собой модель для вывода результатов запроса
    /// </summary>
    public class OutputData
    {
        /// <summary>
        /// Наименование груза, по которому совершена группировка
        /// </summary>
        public string GroupFreight { get; set; }

        /// <summary>
        /// Промежуточная общая масса брутто груза в группе
        /// </summary>
        public int TotalGroupWeight { get; set; }

        /// <summary>
        /// Количество записей (вагонов) в группе
        /// </summary>
        public int TotalGroupCount { get; set; }

        /// <summary>
        /// Полная информация о каждой записе (вагоне) в группе
        /// </summary>
        public List<SelectResult> RecordsInGroup { get; set; }
    }
}
