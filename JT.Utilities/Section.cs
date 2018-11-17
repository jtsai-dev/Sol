using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JT.Infrastructure
{
    //List<object> data = new List<object>()
    //{
    //    new { X = int.MinValue, Y = 2 },
    //    new { X = 4, Y = 6 },
    //    new { X = 8, Y = int.MaxValue }
    //};
    //var search = new { X = 0, Y = 10 };
    //var temp4 = Section.GetCrossSection<int>(
    //        ComparerModel<int>.ConvertToListComparerModel(data, "X", "Y"),
    //        ComparerModel<int>.ConvertToComparerModel(search, "X", "Y"));
    //var temp5 = Section.GetConbimeSection<int>(ComparerModel<int>.ConvertToListComparerModel(data, "X", "Y"));

    public class Section
    {
        /// <summary>
        /// get the cross of the sections
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Souce"></param>
        /// <param name="Search"></param>
        /// <returns></returns>
        public static List<ComparerModel<T>> GetCrossSection<T>(List<ComparerModel<T>> Souce, ComparerModel<T> Search) where T : struct, IComparable
        {
            List<ComparerModel<T>> result = new List<ComparerModel<T>>();
            Souce.ForEach(o =>
            {
                //var temp = o.X <= Search.Y && o.Y >= Search.X;
                //if(o.X <= search.Y && o.Y >= search.X)
                if (o.X.CompareTo(Search.Y) <= 0 && o.Y.CompareTo(Search.X) >= 0)//had cross
                {
                    //get the max start
                    var x = Search.X.CompareTo(o.X) == 1 ? //search.X > o.X ?
                            Search.X : o.X;
                    //get the min end
                    var y = Search.Y.CompareTo(o.Y) == 1 ? //search.Y > o.Y ?
                            o.Y : Search.Y;
                    result.Add(new ComparerModel<T>() { X = x, Y = y, });
                }
            });

            return result;
        }

        /// <summary>
        /// conbime the sections
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Souce"></param>
        /// <param name="Search"></param>
        /// <returns></returns>
        public static List<ComparerModel<T>> GetConbimeSection<T>(List<ComparerModel<T>> Souce) where T : struct, IComparable
        {
            Souce.ForEach(o =>
            {
                Souce.Where(p => p != o).ToList()
                     //q.X <= o.Y && q.Y >= o.X
                     .FindAll(q => q.X.CompareTo(o.Y) <= 0 && q.Y.CompareTo(o.X) >= 0)//get the cross
                     .ForEach(r =>
                     {
                         //get the min start
                         o.X = r.X.CompareTo(o.X) == 1 ? //r.X > o.X ?
                               o.X : r.X;
                         //get the max end
                         o.Y = r.Y.CompareTo(o.Y) == 1 ? //r.Y > o.Y ?
                               r.Y : o.Y;
                         //set the cross as same
                         if (Souce.Contains(r))
                         {
                             r.X = o.X;
                             r.Y = o.Y;
                         }
                     });
            });
            var result = Souce.GroupBy(m => new { m.X, m.Y })
                              .Select(n => new ComparerModel<T>() { X = n.Key.X, Y = n.Key.Y })
                              .ToList();

            return result;
        }

        //private static void GetCrossSection()
        //{
        //    List<Point> data = new List<Point>()
        //    {
        //        new Point() { X = int.MinValue, Y = 2 },
        //        new Point() { X = 4, Y = 6 },
        //        new Point() { X = 8, Y = int.MaxValue }
        //    };
        //    Point search = new Point() { X = 0, Y = 10 };

        //    List<Point> result = new List<Point>();
        //    data.ForEach(o =>
        //    {
        //        if (o.X <= search.Y && o.Y >= search.X)//had cross
        //        {
        //            var x = search.X > o.X ? search.X : o.X;//get the max start
        //            var y = search.Y > o.Y ? o.Y : search.Y;//get the min end
        //            result.Add(new Point() { X = x, Y = y, });
        //        }
        //    });

        //    foreach (var item in result)
        //    {
        //        Console.WriteLine(item.ToString());
        //    }
        //}
        //private static void GetConbimeSection()
        //{
        //    List<Point> data = new List<Point>()
        //    {
        //        new Point() { X = int.MinValue, Y = 2 },
        //        new Point() { X = 4, Y = 6 },
        //        new Point() { X = 8, Y = int.MaxValue }
        //    };

        //    data.ForEach(o =>
        //    {
        //        data.Where(p => p != o).ToList().FindAll(q => q.X <= o.Y && q.Y >= o.X)//get the cross
        //            .ForEach(r =>
        //            {
        //                o.X = r.X > o.X ? o.X : r.X;//get the min start
        //                o.Y = r.Y > o.Y ? r.Y : o.Y;//get the max end
        //                //set the cross as same
        //                if (data.Contains(r))
        //                {
        //                    r.X = o.X;
        //                    r.Y = o.Y;
        //                }
        //            });
        //    });
        //    data = data.GroupBy(m => new { m.X, m.Y })
        //        .Select(n => new Point() { X = n.Key.X, Y = n.Key.Y })
        //        .ToList();

        //    foreach (var item in data)
        //    {
        //        Console.WriteLine(item.ToString());
        //    }
        //}
        //private class Point
        //{
        //    public int X { get; set; }
        //    public int Y { get; set; }
        //    public override string ToString()
        //    {
        //        return X + " : " + Y;
        //    }
        //}
    }

    /// <summary>
    /// The model for comparer, use to support the function GetCrossSection<T>() and GetConbimeSection()
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ComparerModel<T> where T : struct, IComparable//, IComparable
    {
        public T X { get; set; }
        public T Y { get; set; }

        /// <summary>
        /// Convert to the model which could be comparer
        /// </summary>
        /// <param name="model"></param>
        /// <param name="propertyX">The proppertyName of the model, it's type should be same as T</param>
        /// <param name="propertyY">The proppertyName of the model, it's type should be same as T</param>
        /// <returns></returns>
        public static ComparerModel<T> ConvertToComparerModel(object model, string propertyX, string propertyY)
        {
            Type t = model.GetType();

            var x = t.GetProperty(propertyX).GetValue(model, null);
            var y = t.GetProperty(propertyY).GetValue(model, null);

            var comparerModel = new ComparerModel<T>();
            comparerModel.X = (T)x;
            comparerModel.Y = (T)y;

            return comparerModel;
        }

        /// <summary>
        /// Convert to the list of model which could be comparer
        /// </summary>
        /// <param name="listModel"></param>
        /// <param name="propertyX">The proppertyName of the model, it's type should be same as T</param>
        /// <param name="propertyY">The proppertyName of the model, it's type should be same as T</param>
        /// <returns></returns>
        public static List<ComparerModel<T>> ConvertToListComparerModel(List<object> listModel, string propertyX, string propertyY)
        {
            return listModel.Select(p => ConvertToComparerModel(p, propertyX, propertyY)).ToList();
        }

        //public override string ToString()
        //{
        //    return string.Format("{0} : {1}", this.X, this.Y);
        //}
    }
}
