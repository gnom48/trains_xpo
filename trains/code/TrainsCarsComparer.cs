using System;
using System.Collections.Generic;
using trains.models;

namespace trains.code
{
    /// <summary>
    /// Класс TrainsCarsComparer 
    /// для сравнения объектов типа TrainsCars
    /// </summary>
    class TrainsCarsComparer : IEqualityComparer<TrainsCars>
    {
        /// <summary>
        /// Метод сраненивает 2 объекта типа TrainsCars по значениям их атрибутов
        /// </summary>
        /// <param name="x">первый объект для сравнения</param>
        /// <param name="y">второй объект для сравнения</param>
        /// <returns>Если объекты равны то true, иначе false</returns>
        public bool Equals(TrainsCars x, TrainsCars y)
        {
            if (Object.ReferenceEquals(x, y)) return true;
            if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null))
                return false;
            return x.CarId == y.CarId && x.TrainId == y.TrainId && x.CarPositionInTrain == y.CarPositionInTrain;
        }

        /// <summary>
        /// Метод для получения хэша объекта
        /// </summary>
        /// <param name="obj">объект типа TrainsCars, от которого требуется получить хэш</param>
        /// <returns>хэш объекта</returns>
        public int GetHashCode(TrainsCars obj)
        {
            if (Object.ReferenceEquals(obj, null)) return 0;
            int hashTrain = obj.TrainId.GetHashCode();
            int hashCar = obj.CarId.GetHashCode();
            int hashPos = obj.CarPositionInTrain.GetHashCode(); 
            return hashTrain ^ hashCar ^ hashPos;
        }
    }
}
