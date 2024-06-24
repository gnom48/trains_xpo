using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using trains.models;

namespace trains.code
{
    /// <summary>
    /// Класс TrainsComparer 
    /// для сравнения объектов типа Train
    /// </summary>
    //class TrainsComparer : IEqualityComparer<Train>
    //{
    //    /// <summary>
    //    /// Метод сраненивает 2 объекта типа Train по значениям их атрибутов
    //    /// </summary>
    //    /// <param name="x">первый объект для сравнения</param>
    //    /// <param name="y">второй объект для сравнения</param>
    //    /// <returns>Если объекты равны то true, иначе false</returns>
    //    public bool Equals(Train x, Train y)
    //    {
    //        if (Object.ReferenceEquals(x, y)) return true;
    //        if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null))
    //            return false;
    //        return x.TrainNumber == y.TrainNumber && x.TrainIndexCombined == y.TrainIndexCombined;
    //    }

    //    /// <summary>
    //    /// Метод для получения хэша объекта
    //    /// </summary>
    //    /// <param name="obj">объект типа Train, от которого требуется получить хэш</param>
    //    /// <returns>хэш объекта</returns>
    //    public int GetHashCode(Train obj)
    //    {
    //        if (Object.ReferenceEquals(obj, null)) return 0;
    //        int hashTrainNumber = obj.TrainNumber.GetHashCode();
    //        int hashTrainIndexCombined = obj.TrainIndexCombined.GetHashCode();
    //        return hashTrainNumber ^ hashTrainIndexCombined;
    //    }
    //}
}
