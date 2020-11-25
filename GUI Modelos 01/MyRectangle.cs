using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUI_Modelos_01
{
    public class MyRectangle
    {
        public double Width { get; set; }
        public double Height { get; set; }
        public bool Rotate { get; set; }

        public FiguraDibujable figura {get; set;}

        public Tuple<double, double> InferiorLeftPoint { get; set; }

        public MyRectangle(double width, double height, FiguraDibujable figura=null)
        {
            this.Width = width;
            this.Height = height;
            this.Rotate = false;
            this.InferiorLeftPoint = new Tuple<double, double>(-1.0, -1.0);
            this.figura = figura;
        }

        public double Area()
        {
            return this.Width * this.Height;
        }

        public double Perimeter()
        {
            return 2 * (this.Width + this.Height);
        }

        public double Max_Width_Height()
        {
            return Math.Max(this.Width, this.Height);
        }

        public double Diag_Width_Height()
        {
            return this.Width + this.Height + Math.Sqrt(Math.Pow(this.Height, 2) + Math.Pow(this.Width, 2));
        }

        private static int Comparer_By_Area(MyRectangle rct1, MyRectangle rct2)
        {
            var area_rct1 = rct1.Area();
            var area_rct2 = rct2.Area();

            if (area_rct1 > area_rct2)
                return 1;
            else if (area_rct1 < area_rct2)
                return -1;
            else
                return 0;
        }

        private static int Comparer_By_Perimeter(MyRectangle rct1, MyRectangle rct2)
        {
            var perimeter_rct1 = rct1.Perimeter();
            var perimeter_rct2 = rct2.Perimeter();

            if (perimeter_rct1 > perimeter_rct2)
                return 1;
            else if (perimeter_rct1 < perimeter_rct2)
                return -1;
            else
                return 0;
        }

        private static int Comparer_By_Max(MyRectangle rct1, MyRectangle rct2)
        {
            var max_rct1 = rct1.Max_Width_Height();
            var max_rct2 = rct2.Max_Width_Height();

            if (max_rct1 > max_rct2)
                return 1;
            else if (max_rct1 < max_rct2)
                return -1;
            else
                return 0;
        }

        private static int Comparer_By_Diag(MyRectangle rct1, MyRectangle rct2)
        {
            var diag_rct1 = rct1.Diag_Width_Height();
            var diag_rct2 = rct2.Diag_Width_Height();

            if (diag_rct1 > diag_rct2)
                return 1;
            else if (diag_rct1 < diag_rct2)
                return -1;
            else
                return 0;
        }

        public static void Sort_By_Area(List<MyRectangle> lst)
        {
            lst.Sort(MyRectangle.Comparer_By_Area);
            lst.Reverse();
        }

        public static void Sort_By_Perimeter(List<MyRectangle> lst)
        {
            lst.Sort(MyRectangle.Comparer_By_Perimeter);
            lst.Reverse();
        }

        public static void Sort_By_Max(List<MyRectangle> lst)
        {
            lst.Sort(MyRectangle.Comparer_By_Max);
            lst.Reverse();
        }

        public static void Sort_By_Diag(List<MyRectangle> lst)
        {
            lst.Sort(MyRectangle.Comparer_By_Diag);
            lst.Reverse();
        }

        public override string ToString()
        {
            return $"Width: {this.Width.ToString()}, Height: {this.Height}, Inferior Left Point: {this.InferiorLeftPoint}, Rotate: {this.Rotate}";
        }
    }
}
