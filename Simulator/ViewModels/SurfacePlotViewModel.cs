using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using Caliburn.Micro;

namespace Simulator.ViewModels
{
	public class SurfacePlotViewModel : PropertyChangedBase
	{
		public double MinX { get; set; }
		public double MinY { get; set; }
		public double MaxX { get; set; }
		public double MaxY { get; set; }
		public int Rows { get; set; }
		public int Columns { get; set; }

		public Func<double, double, double> Function { get; set; }
		public Point3D[,] Data { get; set; }
		public double[,] ColorValues { get; set; }

		public Model3DGroup Lights
		{
			get 
			{ 
				var group = new Model3DGroup();
				group.Children.Add(new AmbientLight(Colors.Gray));
				group.Children.Add(new PointLight(Colors.Red, new Point3D(0, -1000, 0)));
				group.Children.Add(new PointLight(Colors.Blue, new Point3D(0, 0, 1000)));
				group.Children.Add(new PointLight(Colors.Green, new Point3D(1000, 1000, 0)));

				return group;
			}
		}

		public Brush SurfaceBrush
		{
			get { return Brushes.White; }
		}


		public SurfacePlotViewModel()
		{
			MinX = 0;
			MaxX = 3;
			MinY = 0;
			MaxY = 3;
			Rows = 91;
			Columns = 91;

			Function = (x, y) => Math.Sin(x * y) * 0.5;
			UpdateModel();
		}

		private void UpdateModel()
		{
			Data = CreateDataArray(Function);

			ColorValues = null;

			NotifyOfPropertyChange(() => Data);
			NotifyOfPropertyChange(() => ColorValues);
			NotifyOfPropertyChange(() => SurfaceBrush);
		}

		public Point3D[,] CreateDataArray(Func<double, double, double> f)
		{
			var data = new Point3D[Rows, Columns];
			for (int i = 0; i < Rows; i++)
				for (int j = 0; j < Columns; j++)
				{
					var pt = GetPointFromIndex(i, j);
					data[i, j] = new Point3D(pt.X, pt.Y, f(pt.X, pt.Y));
				}
			return data;
		}

		public Point GetPointFromIndex(int i, int j)
		{
			double x = MinX + (double)j / (Columns - 1) * (MaxX - MinX);
			double y = MinY + (double)i / (Rows - 1) * (MaxY - MinY);
			return new Point(x, y);
		}

		public double[,] FindGradientY(Point3D[,] data)
		{
			int n = data.GetUpperBound(0) + 1;
			int m = data.GetUpperBound(0) + 1;
			var K = new double[n, m];
			for (int i = 0; i < n; i++)
				for (int j = 0; j < m; j++)
				{
					// Finite difference approximation
					var p10 = data[i + 1 < n ? i + 1 : i, j - 1 > 0 ? j - 1 : j];
					var p00 = data[i - 1 > 0 ? i - 1 : i, j - 1 > 0 ? j - 1 : j];
					var p11 = data[i + 1 < n ? i + 1 : i, j + 1 < m ? j + 1 : j];
					var p01 = data[i - 1 > 0 ? i - 1 : i, j + 1 < m ? j + 1 : j];

					//double dx = p01.X - p00.X;
					//double dz = p01.Z - p00.Z;
					//double Fx = dz / dx;

					double dy = p10.Y - p00.Y;
					double dz = p10.Z - p00.Z;

					K[i, j] = dz / dy;
				}
			return K;
		}
	}
}
