using trains.models;
using System.Data.Entity;
using DevExpress.Xpo.DB;
using DevExpress.Xpo;

namespace trains
{
    public class TrainsDbContext
    {
        public static string connectionString = MSSqlConnectionProvider.GetConnectionString(@".\SQLEXPRESS", "trains_xpo");

        public static void Initialize() 
        {
            XpoDefault.DataLayer = XpoDefault.GetDataLayer(connectionString, AutoCreateOption.DatabaseAndSchema);
            XpoDefault.Session = null;
            XpoDefault.Session.CommitTransaction();
        }

        public static UnitOfWork GetUnitOfWork()
        {
            return new UnitOfWork();
        }
    }
}
