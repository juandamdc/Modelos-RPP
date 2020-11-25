using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUI_Modelos_01
{
    class Skyline
    {
        private List<Tuple<double, double, double>> SkylineRepresentation { get; set; }
        private double Width { get; }
        private double Height { get; }

        public Skyline(double width, double height)
        {
            this.Width = width;
            this.Height = height;
        }

        private Tuple<List<Tuple<double, double>>, List<Tuple<double, double>>> Endpoints(List<Tuple<double, double, double>> skyline)
        {
            var left_points = new List<Tuple<double, double>>();
            var right_points = new List<Tuple<double, double>>();

            left_points.Add(new Tuple<double, double>(skyline[0].Item1, skyline[0].Item3));

            for (int i = 1; i < skyline.Count; i++)
                if (skyline[i - 1].Item3 > skyline[i].Item3)
                    left_points.Add(new Tuple<double, double>(skyline[i].Item1, skyline[i].Item3));

            for (int i = 0; i < skyline.Count - 1; i++)
                if (skyline[i].Item3 < skyline[i + 1].Item3)
                    right_points.Add(new Tuple<double, double>(skyline[i].Item2, skyline[i].Item3));

            right_points.Add(new Tuple<double, double>(skyline[skyline.Count - 1].Item2, skyline[skyline.Count - 1].Item3));

            return new Tuple<List<Tuple<double, double>>, List<Tuple<double, double>>>(left_points, right_points);
        }

        private List<Tuple<double, double, double>> UpdateStep1(List<Tuple<double, double, double>> skyline, Tuple<double, double> endpoint, MyRectangle rectangle)
        {
            var new_skyline = new List<Tuple<double, double, double>>();

            int i = 0;
            while (skyline[i].Item1 != endpoint.Item1 || skyline[i].Item3 != endpoint.Item2)
            {
                new_skyline.Add(skyline[i]);
                i++;
            }

            new_skyline.Add(new Tuple<double, double, double>(skyline[i].Item1, skyline[i].Item1 + rectangle.Width, skyline[i].Item3 + rectangle.Height));

            if (rectangle.Width < skyline[i].Item2 - skyline[i].Item1)
            {
                new_skyline.Add(new Tuple<double, double, double>(skyline[i].Item1 + rectangle.Width, skyline[i].Item2, skyline[i].Item3));
                i++;
            }
            else if (rectangle.Width > skyline[i].Item2 - skyline[i].Item1)
            {
                int j = 0;

                while (i + j < skyline.Count && new_skyline[i].Item2 > skyline[i + j].Item2)
                    j++;

                if (i + j < skyline.Count)
                    new_skyline.Add(new Tuple<double, double, double>(new_skyline[i].Item2, skyline[i + j].Item2, skyline[i + j].Item3));

                i += j + 1;
            }
            else
                i++;

            for (; i < skyline.Count; i++)
                new_skyline.Add(skyline[i]);


            skyline = new_skyline;
            new_skyline = new List<Tuple<double, double, double>>();

            for (int k = 0; k < skyline.Count;)
            {
                int j = 1;
                while (j + k < skyline.Count && skyline[k].Item3 == skyline[k + j].Item3)
                    j++;

                new_skyline.Add(new Tuple<double, double, double>(skyline[k].Item1, skyline[k + j - 1].Item2, skyline[k].Item3));
                k += j;
            }

            return new_skyline;
        }

        private List<Tuple<double, double, double>> UpdateStep2(List<Tuple<double, double, double>> skyline, List<MyRectangle> remainingRectangles)
        {
            if (skyline.Count == 1)
                return skyline;

            List<Tuple<double, double, double>> newSkyline = null;
            bool whileUpdate = true;

            while (whileUpdate && skyline.Count > 1)
            {
                whileUpdate = false;
                newSkyline = new List<Tuple<double, double, double>>();

                if (skyline[0].Item3 < skyline[1].Item3 && !Utils.CanPlaceAny(skyline, this.Width, this.Height, new Tuple<double, double>(skyline[0].Item1, skyline[0].Item3), remainingRectangles))
                {
                    newSkyline.Add(new Tuple<double, double, double>(skyline[0].Item1, skyline[1].Item2, skyline[1].Item3));
                    whileUpdate = true;
                }
                else
                    newSkyline.Add(skyline[0]);

                int i = 1;
                for (; i < skyline.Count - 1; i++)
                {
                    if (skyline[i].Item3 < skyline[i - 1].Item3 && skyline[i].Item3 < skyline[i + 1].Item3 && !Utils.CanPlaceAny(skyline, this.Width, this.Height, new Tuple<double, double>(skyline[i].Item1, skyline[i].Item3), remainingRectangles))
                    {
                        if (skyline[i - 1].Item3 == skyline[i + 1].Item3)
                        {
                            newSkyline[i - 1] = new Tuple<double, double, double>(skyline[i - 1].Item1, skyline[i + 1].Item2, skyline[i + 1].Item3);
                            i++;
                        }
                        else if (skyline[i - 1].Item3 < skyline[i + 1].Item3)
                            newSkyline[newSkyline.Count - 1] = new Tuple<double, double, double>(skyline[i - 1].Item1, skyline[i].Item2, skyline[i - 1].Item3);
                        else
                        {
                            newSkyline.Add(new Tuple<double, double, double>(skyline[i].Item1, skyline[i + 1].Item2, skyline[i + 1].Item3));
                            i++;
                        }

                        whileUpdate = true;
                    }
                    else
                        newSkyline.Add(skyline[i]);
                }

                if (i < skyline.Count)
                {
                    if (skyline[i].Item3 < skyline[i - 1].Item3 && !Utils.CanPlaceAny(skyline, this.Width, this.Height, new Tuple<double, double>(skyline[i].Item1, skyline[i].Item3), remainingRectangles))
                    {
                        newSkyline[newSkyline.Count - 1] = new Tuple<double, double, double>(skyline[i - 1].Item1, skyline[i].Item2, skyline[i - 1].Item3);
                        whileUpdate = true;
                    }
                    else
                        newSkyline.Add(skyline[i]);
                }

                skyline = newSkyline;
            }

            return newSkyline;
        }

        private Dictionary<int, dynamic> LeftPlacementEvaluation(List<Tuple<double, double, double>> skyline, Tuple<double, double> endpoint, double spread, MyRectangle rectangle, List<MyRectangle> rectangles)
        {
            var evaluation = new Dictionary<int, dynamic>();
            evaluation[1] = false;

            if (Utils.CanPlace(skyline, this.Width, this.Height, endpoint, rectangle))
            {
                evaluation[-2] = rectangle;
                evaluation[-1] = endpoint;
                evaluation[0] = UpdateStep1(skyline, endpoint, rectangle);
                evaluation[1] = Utils.CalcSpread(evaluation[0], spread);

                if (evaluation[1])
                {
                    evaluation[2] = false;
                    evaluation[3] = Utils.CalcLocalWaste(skyline, this.Width, this.Height, rectangle, rectangles, endpoint);
                    evaluation[4] = Utils.FitnessNumber(skyline, this.Height, rectangle, endpoint);
                }
            }

            return evaluation;
        }

        private Dictionary<int, dynamic> RightPlacementEvaluation(List<Tuple<double, double, double>> skyline, Tuple<double, double> endpoint, double spread, MyRectangle rectangle, List<MyRectangle> rectangles)
        {
            var skylineRevert = Utils.Revert(skyline, this.Width);
            var new_endpoint = new Tuple<double, double>(this.Width - endpoint.Item1, endpoint.Item2);

            var evaluation = LeftPlacementEvaluation(skylineRevert, new_endpoint, spread, rectangle, rectangles);
            if (evaluation[1])
            {
                evaluation[-1] = new Tuple<double, double>(this.Width - evaluation[-1].Item1 - rectangle.Width, evaluation[-1].Item2);
                evaluation[0] = Utils.Revert(evaluation[0], this.Width);
            }

            return evaluation;
        }

        private void UpdateSkyline(Dictionary<int, dynamic> placement, List<MyRectangle> remainigRectangles)
        {
            this.SkylineRepresentation = placement[0];

            if (remainigRectangles.Count > 0)
                this.SkylineRepresentation = UpdateStep2(this.SkylineRepresentation, remainigRectangles);
        }

        private Tuple<bool, List<MyRectangle>> RunHeuristic(List<MyRectangle> rectangles, double spread)
        {
            this.SkylineRepresentation = new List<Tuple<double, double, double>>() { new Tuple<double, double, double>(0, this.Width, 0) };
            var placedRectangles = new List<MyRectangle>();

            while (rectangles.Count > 0)
            {
                var bestPlacements = new Dictionary<Tuple<double, double>, Dictionary<int, dynamic>>();
                var temp= Endpoints(this.SkylineRepresentation);
                var leftEndpoints = temp.Item1;
                var rightEndpoints = temp.Item2;

                foreach (var endpoint in leftEndpoints)
                {
                    bestPlacements[endpoint] = Utils.WorstPlacement();
                    var cantPlacement = 0;

                    foreach (var rectangle in rectangles)
                    {
                        // the rectangle without rotation
                        var placement = LeftPlacementEvaluation(this.SkylineRepresentation, endpoint, spread, rectangle, rectangles);

                        if (placement[1])
                            cantPlacement++;

                        if (Utils.ComparePlacement(placement, bestPlacements[endpoint]) == 1)
                            bestPlacements[endpoint] = placement;

                        //// the rectangle with rotation
                        //var rotateRectangle = new MyRectangle(rectangle.Height, rectangle.Width);
                        //rotateRectangle.figura = rectangle.figura;
                        //rotateRectangle.Rotate = true;
                        //placement = LeftPlacementEvaluation(this.SkylineRepresentation, endpoint, spread, rotateRectangle, rectangles);

                        //if (placement[1])
                        //    cantPlacement++;

                        //if (Utils.ComparePlacement(placement, bestPlacements[endpoint]) == 1)
                        //    bestPlacements[endpoint] = placement;
                    }

                    if (cantPlacement == 1)
                        bestPlacements[endpoint][2] = true;
                }

                foreach (var endpoint in rightEndpoints)
                {
                    bestPlacements[endpoint] = Utils.WorstPlacement();
                    var cantPlacement = 0;

                    foreach (var rectangle in rectangles)
                    {
                        // the rectangle without rotation
                        var placement = RightPlacementEvaluation(this.SkylineRepresentation, endpoint, spread, rectangle, rectangles);

                        if (placement[1])
                            cantPlacement++;

                        if (Utils.ComparePlacement(placement, bestPlacements[endpoint]) == 1)
                            bestPlacements[endpoint] = placement;

                        //// the rectangle with rotation
                        //var rotateRectangle = new MyRectangle(rectangle.Height, rectangle.Width);
                        //rotateRectangle.figura = rectangle.figura;
                        //rotateRectangle.Rotate = true;
                        //placement = RightPlacementEvaluation(this.SkylineRepresentation, endpoint, spread, rotateRectangle, rectangles);

                        //if (placement[1])
                        //    cantPlacement++;

                        //if (Utils.ComparePlacement(placement, bestPlacements[endpoint]) == 1)
                        //    bestPlacements[endpoint] = placement;
                    }

                    if (cantPlacement == 1)
                        bestPlacements[endpoint][2] = true;
                }

                var bestPlacement = Utils.SelectBestPlacement(bestPlacements);

                if (Utils.ComparePlacement(bestPlacement, Utils.WorstPlacement()) == 1)
                {
                    if (bestPlacement[-2].Rotate)
                    {
                        var rectangleToEliminate = rectangles.Where(r => r.Width == bestPlacement[-2].Height && r.Height == bestPlacement[-2].Width).First();
                        rectangles.Remove(rectangleToEliminate);
                    }
                    else
                        rectangles.Remove(bestPlacement[-2]);

                    bestPlacement[-2].InferiorLeftPoint = bestPlacement[-1];
                    placedRectangles.Add(bestPlacement[-2]);
                    UpdateSkyline(bestPlacement, rectangles);
                }
                else
                    break;
            }

            if (rectangles.Count > 0)
                return new Tuple<bool, List<MyRectangle>>(false, placedRectangles);

            return new Tuple<bool, List<MyRectangle>>(true, placedRectangles);
        }

        public Tuple<bool, List<MyRectangle>> Run(List<MyRectangle> rectangles)
        {
            foreach (var spread in Utils.Spreads(this.Height, rectangles))
            {
                foreach (var rectangle in Utils.GetRectanglesOrdered(rectangles))
                {
                    var result = RunHeuristic(rectangle, spread);
                    if (result.Item1)
                        return result;
                }
            }

            return new Tuple<bool, List<MyRectangle>>(false, new List<MyRectangle>());
        }
    }

    static class Utils
    {
        public static List<List<MyRectangle>> GetRectanglesOrdered(List<MyRectangle> rectangles)
        {
            List<List<MyRectangle>> rectanglesToReturn = new List<List<MyRectangle>>();
            var rectanglesArray = new MyRectangle[rectangles.Count];

            MyRectangle.Sort_By_Area(rectangles);
            rectangles.CopyTo(rectanglesArray);
            rectanglesToReturn.Add(rectanglesArray.ToList());

            MyRectangle.Sort_By_Perimeter(rectangles);
            rectangles.CopyTo(rectanglesArray);
            rectanglesToReturn.Add(rectanglesArray.ToList());

            MyRectangle.Sort_By_Diag(rectangles);
            rectangles.CopyTo(rectanglesArray);
            rectanglesToReturn.Add(rectanglesArray.ToList());

            MyRectangle.Sort_By_Max(rectangles);
            rectangles.CopyTo(rectanglesArray);
            rectanglesToReturn.Add(rectanglesArray.ToList());

            return rectanglesToReturn;
        }

        public static List<double> Spreads(double skylineHeight, List<MyRectangle> rectangles)
        {
            double max_height = 0;

            foreach (var rectangle in rectangles)
            {
                if (rectangle.Max_Width_Height() > max_height)
                    max_height = rectangle.Max_Width_Height();
            }

            return new List<double>() { max_height, max_height + 1 / 3 * (skylineHeight - max_height), max_height + 2 / 3 * (skylineHeight - max_height), skylineHeight };
        }

        public static bool CanPlaceAny(List<Tuple<double, double, double>> skyline, double skylineWidth, double skylineHeight, Tuple<double, double> endpoint, List<MyRectangle> rectangles)
        {
            foreach (var rectangle in rectangles)
            {
                if (CanPlace(skyline, skylineWidth, skylineHeight, endpoint, rectangle) || CanPlace(skyline, skylineWidth, skylineHeight, endpoint, new MyRectangle(rectangle.Height, rectangle.Width) { figura=rectangle.figura}))
                    return true;
            }

            return false;
        }

        public static bool CanPlace(List<Tuple<double, double, double>> skyline, double skylineWidth, double skylineHeight, Tuple<double, double> endpoint, MyRectangle rectangle)
        {
            int i = 0;
            while (skyline[i].Item1 != endpoint.Item1 || skyline[i].Item3 != endpoint.Item2)
                i++;

            if (skyline[i].Item1 + rectangle.Width <= skylineWidth && skyline[i].Item3 + rectangle.Height <= skylineHeight)
            {
                int j = 1;
                while (i + j < skyline.Count && skyline[i + j].Item1 < skyline[i].Item1 + rectangle.Width)
                {
                    if (skyline[i + j].Item3 > skyline[i].Item3)
                        return false;

                    j++;
                }

                return true;
            }

            return false;
        }

        public static List<Tuple<double, double, double>> Revert(List<Tuple<double, double, double>> skylineRepresentation, double width)
        {
            var new_representation = new List<Tuple<double, double, double>>();
            foreach (var item in skylineRepresentation)
                new_representation.Add(new Tuple<double, double, double>(width - item.Item2, width - item.Item1, item.Item3));

            new_representation.Reverse();
            return new_representation;
        }

        public static bool CalcSpread(List<Tuple<double, double, double>> skyline, double spread)
        {
            double max = 0;
            double min = double.MaxValue;

            foreach (var item in skyline)
            {
                if (item.Item3 > max)
                    max = item.Item3;

                if (item.Item3 < min)
                    min = item.Item3;
            }

            return max - min < spread;
        }

        public static double CalcLocalWaste(List<Tuple<double, double, double>> skyline, double skylineWidth, double skylineHeight, MyRectangle rectangle, List<MyRectangle> remainingRectangles, Tuple<double, double> position)
        {
            double waste = 0;
            double min_width = double.MaxValue;
            double min_height = double.MaxValue;

            if (rectangle.Rotate)
            {
                var rectangleDif = remainingRectangles.Where(r => r.Width == rectangle.Height && r.Height == rectangle.Width).First();

                foreach (var rct in remainingRectangles)
                {
                    if (rct != rectangleDif)
                    {
                        if (rct.Width < min_width)
                            min_width = rct.Width;

                        if (rct.Height < min_height)
                            min_height = rct.Height;
                    }
                }
            }
            else
                foreach (var rct in remainingRectangles)
                {
                    if (rct != rectangle)
                    {
                        if (rct.Width < min_width)
                            min_width = rct.Width;

                        if (rct.Height < min_height)
                            min_height = rct.Height;
                    }
                }

            int i = 0;
            while (skyline[i].Item1 != position.Item1 || skyline[i].Item3 != position.Item2)
                i++;

            // type a
            int j = 1;
            while (i + j < skyline.Count && skyline[i + j].Item1 < skyline[i].Item1 + rectangle.Width)
            {
                if (skyline[i + j].Item2 < skyline[i].Item1 + rectangle.Width)
                    waste += (skyline[i + j].Item2 - skyline[i + j].Item1) * (skyline[i].Item3 - skyline[i + j].Item3);
                else
                    waste += (skyline[i].Item1 + rectangle.Width - skyline[i + j].Item1) * (skyline[i].Item3 - skyline[i + j].Item3);

                j++;
            }

            // type b
            bool cond;

            if (i != 0)
            {
                j = i - 1;
                while (j != 0 && skyline[j].Item3 < skyline[i].Item3 + rectangle.Height)
                    j--;

                if (j == i - 1)
                    cond = false;
                else
                {
                    cond = skyline[i].Item1 - skyline[j].Item1 < min_width;
                    j++;
                }

                if (cond)
                    while (j != i)
                    {
                        waste += (skyline[j].Item2 - skyline[j].Item1) * (skyline[i].Item3 + rectangle.Height - skyline[j].Item3);
                        j++;
                    }
            }

            // type c
            j = i;
            while (j < skyline.Count && skyline[j].Item3 < skyline[i].Item3 + rectangle.Height)
                j++;

            if (j == skyline.Count)
                cond = skylineWidth - (skyline[i].Item1 + rectangle.Width) < min_width;
            else
                cond = skyline[j].Item1 - (skyline[i].Item1 + rectangle.Width) < min_width;

            if (cond)
            {
                int k = 0;
                while (i + k < skyline.Count && skyline[i + k].Item2 < skyline[i].Item1 + rectangle.Width)
                    k++;

                if (i + k < skyline.Count)
                {
                    waste += (skyline[i + k].Item2 - (skyline[i].Item1 + rectangle.Width)) * (skyline[i].Item3 + rectangle.Height - skyline[i + k].Item3);
                    k++;
                }

                while (i + k < skyline.Count && k < j)
                {
                    waste += (skyline[i + k].Item2 - skyline[i + k].Item1) * (skyline[i].Item3 + rectangle.Height - skyline[i + k].Item3);
                    k++;
                }
            }

            // type d
            cond = (skylineHeight - (skyline[i].Item3 + rectangle.Height)) < min_height;

            if (cond)
                waste += rectangle.Width * (skylineHeight - (skyline[i].Item3 + rectangle.Height));

            return waste;
        }

        public static int FitnessNumber(List<Tuple<double, double, double>> skyline, double skylineHeight, MyRectangle rectangle, Tuple<double, double> position)
        {
            int fitnessNumber = 0;

            int i = 0;
            while (skyline[i].Item1 != position.Item1 || skyline[i].Item3 != position.Item2)
                i++;

            // S
            if (skyline[i].Item2 - skyline[i].Item1 == rectangle.Width)
                fitnessNumber++;

            // W
            if (i - 1 > 0 && skyline[i - 1].Item3 - skyline[i].Item3 == rectangle.Height)
                fitnessNumber++;

            // E
            if (i + 1 < skyline.Count && skyline[i].Item1 + rectangle.Width == skyline[i+1].Item2 && skyline[i + 1].Item3 - skyline[i].Item3 == rectangle.Height)
                fitnessNumber++;

            // N
            if (skyline[i].Item3 + rectangle.Height == skylineHeight)
                fitnessNumber++;

            return fitnessNumber;
        }

        public static Dictionary<int, dynamic> WorstPlacement()
        {
            var worstPlacement = new Dictionary<int, dynamic>();
            worstPlacement[1] = false;

            return worstPlacement;
        }

        public static int ComparePlacement(Dictionary<int, dynamic> p1, Dictionary<int, dynamic> p2)
        {
            if (p1[1] && p2[1])
            {
                if (p1[2] == p2[2])
                {
                    if (p1[3] == p2[3])
                    {
                        if (p1[4] > p2[4])
                            return 1;

                        return -1;
                    }

                    if (p1[3] < p2[3])
                        return 1;

                    return -1;
                }

                if (p1[2])
                    return 1;

                return -1;
            }

            if (p1[1])
                return 1;

            return -1;
        }

        public static Dictionary<int, dynamic> SelectBestPlacement(Dictionary<Tuple<double, double>, Dictionary<int, dynamic>> placements)
        {
            var bestPlacement = Utils.WorstPlacement();

            foreach (var key in placements.Keys)
                if (ComparePlacement(placements[key], bestPlacement) == 1)
                    bestPlacement = placements[key];

            return bestPlacement;
        }
    }
}
