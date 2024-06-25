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
        public static string connectionString = MSSqlConnectionProvider.GetConnectionString(@".\SQLEXPRESS", "trains_xpo");

        public static IDataLayer dataLayer;

        /// <summary>
        /// Создает новую базу данных, если не существует, иначе ничего не делает
        /// </summary>
        public static void CreateDatabaseIfNotExists()
        {
            dataLayer = XpoDefault.GetDataLayer(connectionString, AutoCreateOption.DatabaseAndSchema);
            
            using (var uow = new UnitOfWork(dataLayer))
            {
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

            var uniqueCarsNums = root.Rows.Select(x => x.CarNumber).OrderBy(x => x).ToList();
            uniqueCarsNums = uniqueCarsNums.Distinct().ToList();
            var uniqueTrainsNums = root.Rows.Select(x => $"{x.TrainNumber}_{x.TrainIndexCombined}").OrderBy(x => x).ToList();
            uniqueTrainsNums = uniqueTrainsNums.Distinct().ToList();
            var uniqueTrainsCarsNums = root.Rows.Select(x => $"{x.CarNumber}_{x.TrainNumber}_{x.TrainIndexCombined}").ToList();
            uniqueTrainsCarsNums = uniqueTrainsCarsNums.Distinct().ToList();


            var freights = new List<Freight>();
            var operations = new List<Operation>();
            var stations = new List<Station>();
            var invoices = new List<Invoice>();

            // заполнение таблиц-справочников
            foreach (var r in root.Rows)
            {
                freights.Add(new Freight { FreightName = r.FreightEtsngName });
                operations.Add(new Operation { OperationName = r.LastOperationName });
                stations.Add(new Station { StationName = r.LastStationName });
                stations.Add(new Station { StationName = r.FromStationName });
                stations.Add(new Station { StationName = r.ToStationName });
                invoices.Add(new Invoice { InvoiceName = r.InvoiceNum });
            }

            // удаление дублирующихся записей из справочников
            freights = freights.GroupBy(x => x.FreightName).Select(g => g.First()).ToList();
            operations = operations.GroupBy(x => x.OperationName).Select(g => g.First()).ToList();
            invoices = invoices.GroupBy(x => x.InvoiceName).Select(g => g.First()).ToList();
            stations = stations.GroupBy(x => x.StationName).Select(g => g.First()).ToList();

            using (var uow = new UnitOfWork(dataLayer))
            {
                freights.ForEach(x => { var tmp = new Freight(uow); tmp.FreightName = x.FreightName; });
                operations.ForEach(x => { var tmp = new Operation(uow); tmp.OperationName = x.OperationName; });
                invoices.ForEach(x => { var tmp = new Invoice(uow); tmp.InvoiceName = x.InvoiceName; });
                stations.ForEach(x => { var tmp = new Station(uow); tmp.StationName = x.StationName; });
                uow.CommitChanges();

                // заполнение зависимых таблиц
                foreach (var r in root.Rows)
                {
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
        }

        /// <summary>
        /// Очищает данные в таблицах и обнуляет начальные значения идентификаторов
        /// </summary>
        public static void DeleteData()
        {
            using (var uow = new UnitOfWork(dataLayer))
            {
                uow.ExecuteNonQuery("TRUNCATE TABLE dbo.TrainsCars");
                uow.ExecuteNonQuery("TRUNCATE TABLE dbo.Trains");
                uow.ExecuteNonQuery("TRUNCATE TABLE dbo.Cars");
                uow.ExecuteNonQuery("TRUNCATE TABLE dbo.Histories");
            }
        }
    }
}
