using DevExpress.Xpo.DB;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using trains.models;
using DevExpress.Data.Filtering;

namespace trains
{
    /// <summary>
    /// Класс DbHelper
    /// предоставляет методы для работы с базой данных
    /// </summary>
    public class DbHelper
    {
        public static string connectionString = MSSqlConnectionProvider.GetConnectionString(@".\SQLEXPRESS", "trains_xpo_1");

        public static IDataLayer dataLayer;

        private static Stopwatch stopwatch = new Stopwatch();

        /// <summary>
        /// Создает новую базу данных, если не существует, иначе ничего не делает
        /// </summary>
        public static void CreateDatabaseIfNotExists()
        {
            dataLayer = XpoDefault.GetDataLayer(connectionString, AutoCreateOption.DatabaseAndSchema);
            
            using (var uow = new UnitOfWork(dataLayer))
            {
                // обновление связей и структуры таблиц если они не соответствуют последнему состоянию
                uow.UpdateSchema(
                    typeof(Invoice), 
                    typeof(Freight), 
                    typeof(Operation), 
                    typeof(Station), 
                    typeof(Car),
                    typeof(Train),
                    typeof(History),
                    typeof(TrainsCars));
            }
        }

        /// <summary>
        /// Заполняет таблицы базы данных информацией из входного файла
        /// </summary>
        /// <param name="filename">путь до файла с данными в формате XML (необязательный, по умолчанию тестовые данные)</param>
        public static void LoadDataFromXml(string filename = @"\resources\Data.xml")
        {
            stopwatch.Reset();
            stopwatch.Start();

            // построчное чтение файла
            StringBuilder xml = new StringBuilder();
            using (var fs = new StreamReader(Directory.GetCurrentDirectory() + filename))
            {
                while (!fs.EndOfStream)
                {
                    xml.Append(fs.ReadLine());
                }
            }

            // десериализация в объект Root
            Root root;
            XmlSerializer serializer = new XmlSerializer(typeof(Root));
            using (StringReader reader = new StringReader(xml.ToString()))
            {
                root = (Root)serializer.Deserialize(reader);
                root.Rows = root.Rows.OrderBy(x => x.TrainNumber).ThenBy(x => x.TrainIndexCombined).ThenBy(x => x.CarNumber).ThenByDescending(x => DateTime.Parse(x.WhenLastOperation)).ToList();
            }

            // отбор уникальных вхождений объектов для исключения занесения в базу данных дублирующих записей
            var uniqueCarsNums = root.Rows.Select(x => x.CarNumber).OrderBy(x => x).ToList();
            uniqueCarsNums = uniqueCarsNums.Distinct().ToList();
            var uniqueTrainsNums = root.Rows.Select(x => $"{x.TrainNumber}_{x.TrainIndexCombined}").OrderBy(x => x).ToList();
            uniqueTrainsNums = uniqueTrainsNums.Distinct().ToList();
            var uniqueTrainsCarsNums = root.Rows.Select(x => $"{x.CarNumber}_{x.TrainNumber}_{x.TrainIndexCombined}").ToList();
            uniqueTrainsCarsNums = uniqueTrainsCarsNums.Distinct().ToList();

            var freights = new List<string>();
            var operations = new List<string>();
            var stations = new List<string>();
            var invoices = new List<string>();

            // заполнение таблиц-справочников
            foreach (var r in root.Rows)
            {
                freights.Add(r.FreightEtsngName);
                operations.Add(r.LastOperationName);
                stations.Add(r.LastStationName);
                stations.Add(r.FromStationName);
                stations.Add(r.ToStationName);
                invoices.Add(r.InvoiceNum);
            }

            // удаление дублирующихся записей из справочников
            freights = freights.Distinct().ToList();
            operations = operations.Distinct().ToList();
            invoices = invoices.Distinct().ToList();
            stations = stations.Distinct().ToList();

            using (var uow = new UnitOfWork(dataLayer))
            {
                // запись справочников в базу данных
                freights.ForEach(x => new Freight(uow) { FreightName = x });
                operations.ForEach(x => new Operation(uow) { OperationName = x });
                invoices.ForEach(x => new Invoice(uow) { InvoiceName = x });
                stations.ForEach(x => new Station(uow) { StationName = x });
                uow.CommitChanges();

                // заполнение зависимых таблиц
                foreach (var r in root.Rows)
                {
                    // проверка на не дублирующиеся записи
                    if (uniqueCarsNums.Contains(r.CarNumber))
                    {
                        new Car(uow)
                        {
                            CarNumber = r.CarNumber.ToString(),
                            GrossWeight = r.FreightTotalWeightKg,
                            Invoice = uow.Query<Invoice>().Where(x => x.InvoiceName == r.InvoiceNum).FirstOrDefault(),
                            Freight = uow.Query<Freight>().Where(x => x.FreightName == r.FreightEtsngName).FirstOrDefault(),
                        };
                        uniqueCarsNums.Remove(r.CarNumber);
                    }

                    // проверка на не дублирующиеся записи
                    if (uniqueTrainsNums.Contains($"{r.TrainNumber}_{r.TrainIndexCombined}"))
                    {
                        new Train(uow)
                        {
                            TrainIndexCombined = r.TrainIndexCombined,
                            TrainNumber = r.TrainNumber.ToString(),
                            FromStation = uow.Query<Station>().Where(x => x.StationName == r.FromStationName).FirstOrDefault(),
                            ToStation = uow.Query<Station>().Where(x => x.StationName == r.ToStationName).FirstOrDefault(),
                        };
                        uniqueTrainsNums.Remove($"{r.TrainNumber}_{r.TrainIndexCombined}");
                    }
                }

                uow.CommitChanges();

                // заполнение смежной таблицы
                foreach (var r in root.Rows)
                {
                    // проверка на не дублирующиеся записи
                    if (uniqueTrainsCarsNums.Contains($"{r.CarNumber}_{r.TrainNumber}_{r.TrainIndexCombined}"))
                    {
                        new TrainsCars(uow)
                        {
                            Car = uow.Query<Car>().Where(x => x.CarNumber == r.CarNumber.ToString()).FirstOrDefault(),
                            Train = uow.Query<Train>().Where(x => x.TrainNumber == r.TrainNumber.ToString() && x.TrainIndexCombined == r.TrainIndexCombined).FirstOrDefault(),
                            CarPositionInTrain = r.PositionInTrain
                        };
                        uniqueTrainsCarsNums.Remove($"{r.CarNumber}_{r.TrainNumber}_{r.TrainIndexCombined}");
                    }
                }

                uow.CommitChanges();

                // заполнение таблицы истории операций с вагонами
                var tmp_carss = uow.Query<Car>().ToList();
                var tmp_operations = uow.Query<Operation>().ToList();
                var tmp_stations = uow.Query<Station>().ToList();
                foreach (var r in root.Rows)
                {
                    new History(uow)
                    {
                        Car = tmp_carss.Where(x => x.CarNumber == r.CarNumber.ToString()).First(),
                        Operation = tmp_operations.Where(x => x.OperationName == r.LastOperationName).First(),
                        Station = tmp_stations.Where(x => x.StationName == r.LastStationName).First(),
                        OperationDateTime = DateTime.Parse(r.WhenLastOperation)
                    };
                }

                uow.CommitChanges();
            }

            stopwatch.Stop();
            Console.WriteLine($"Импорт данных занял {stopwatch.ElapsedMilliseconds} мс");
        }

        /// <summary>
        /// Очищает данные в таблицах и обнуляет начальные значения идентификаторов
        /// </summary>
        public static void DeleteData()
        {
            stopwatch.Reset();
            stopwatch.Start();
            using (var uow = new UnitOfWork(dataLayer))
            {
                uow.ExecuteNonQuery("delete from dbo.TrainsCars;");
                uow.ExecuteNonQuery("DBCC CHECKIDENT ('TrainsCars', RESEED, 0);");
                uow.ExecuteNonQuery("delete from dbo.History");
                uow.ExecuteNonQuery("DBCC CHECKIDENT ('History', RESEED, 0);");
                uow.ExecuteNonQuery("delete from dbo.Train");
                uow.ExecuteNonQuery("DBCC CHECKIDENT ('Train', RESEED, 0);");
                uow.ExecuteNonQuery("delete from dbo.Car");
                uow.ExecuteNonQuery("DBCC CHECKIDENT ('Car', RESEED, 0);");
                uow.ExecuteNonQuery("delete from dbo.Invoice");
                uow.ExecuteNonQuery("DBCC CHECKIDENT ('Invoice', RESEED, 0);");
                uow.ExecuteNonQuery("delete from dbo.Freight");
                uow.ExecuteNonQuery("DBCC CHECKIDENT ('Freight', RESEED, 0);");
                uow.ExecuteNonQuery("delete from dbo.Station");
                uow.ExecuteNonQuery("DBCC CHECKIDENT ('Station', RESEED, 0);");
                uow.ExecuteNonQuery("delete from dbo.Operation");
                uow.ExecuteNonQuery("DBCC CHECKIDENT ('Operation', RESEED, 0);");
                uow.CommitChanges();
            }
            stopwatch.Stop();
            Console.WriteLine($"Очистка данных заняла {stopwatch.ElapsedMilliseconds} мс");
        }
    }
}
