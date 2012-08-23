using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

namespace Simulator.Internals.Interfaces.Implementations
{
    /// <summary>
    /// A class for performing the main simulation
    /// </summary>
    internal sealed class MainSimulation : ISimulation
    {
        private Configuration config;
        private Matrix d;
    	private Point[] nodes;

		private double[] residVector;

		private bool simulationReady;
        /// <summary>
        /// Creates the D-matrix
        /// </summary>
        /// <param name="e">E</param>
        /// <param name="nu">nu</param>
        /// <returns>A new instance of Matrix - D</returns>
        private static Matrix DMatrix(double e, double nu)
        {
            var d = new Matrix(4, 4);

            d[0, 0] = 1 - nu;
            d[0, 1] = nu;
            d[0, 2] = nu;
            d[0, 3] = 0;

            d[1, 0] = nu;
            d[1, 1] = 1 - nu;
            d[1, 2] = nu;
            d[1, 3] = 0;

            d[2, 0] = nu;
            d[2, 1] = nu;
            d[2, 2] = 1 - nu;
            d[2, 3] = 0;

            d[3, 0] = 0;
            d[3, 1] = 0;
            d[3, 2] = 0;
            d[3, 3] = (1 - nu) / 2;

            var coefficient = e/((1 + nu)*(1 - 2*nu));
            if(double.IsInfinity(coefficient))
                throw new DivideByZeroException("Coefficient is infinity, division by zero occurred.");
            d = coefficient * d;

            return d;
        }

        /// <summary>
        /// Prepares the B-Matrix along with the area and the transposed B-matrix
        /// </summary>
        /// <param name="a">Point a</param>
        /// <param name="b">Point b</param>
        /// <param name="c">Point c</param>
        /// <param name="area">The triangle area</param>
        /// <returns>The B-Matrix</returns>
        private static Matrix BMatrix(Point a, Point b, Point c, out double area)
        {
            var bmat = new Matrix(4, 6);
            double xs = (a.X.Value + b.X.Value + c.X.Value) / 3.0;
            double ys = (a.X.Value + b.X.Value + c.X.Value) / 3.0;
            double na = ((ys - b.Y.Value) * (c.X.Value - b.X.Value) - (xs - b.X.Value) * (c.Y.Value - b.Y.Value)) / ((a.Y.Value - b.Y.Value) * (c.X.Value - b.X.Value) - (a.X.Value - b.X.Value) * (c.Y.Value - b.Y.Value));
            double nb = ((ys - c.Y.Value) * (a.X.Value - c.X.Value) - (xs - c.X.Value) * (a.Y.Value - c.Y.Value)) / ((b.Y.Value - c.Y.Value) * (a.X.Value - c.X.Value) - (b.X.Value - c.X.Value) * (a.Y.Value - c.Y.Value));
            double nc = ((ys - a.Y.Value) * (b.X.Value - a.X.Value) - (xs - a.X.Value) * (b.Y.Value - a.Y.Value)) / ((c.Y.Value - a.Y.Value) * (b.X.Value - a.X.Value) - (c.X.Value - a.X.Value) * (b.Y.Value - a.Y.Value));
            double nax = -(c.Y.Value - b.Y.Value) / ((a.Y.Value - b.Y.Value) * (c.X.Value - b.X.Value) - (a.X.Value - b.X.Value) * (c.Y.Value - b.Y.Value));
            double nay = (c.X.Value - b.X.Value) / ((a.Y.Value - b.Y.Value) * (c.X.Value - b.X.Value) - (a.X.Value - b.X.Value) * (c.Y.Value - b.Y.Value));
            double nbx = -(a.Y.Value - c.Y.Value) / ((b.Y.Value - c.Y.Value) * (a.X.Value - c.X.Value) - (b.X.Value - c.X.Value) * (a.Y.Value - c.Y.Value));
            double nby = (a.X.Value - c.X.Value) / ((b.Y.Value - c.Y.Value) * (a.X.Value - c.X.Value) - (b.X.Value - c.X.Value) * (a.Y.Value - c.Y.Value));
            double ncx = -(b.Y.Value - a.Y.Value) / ((c.Y.Value - a.Y.Value) * (b.X.Value - a.X.Value) - (c.X.Value - a.X.Value) * (b.Y.Value - a.Y.Value));
            double ncy = (b.X.Value - a.X.Value) / ((c.Y.Value - a.Y.Value) * (b.X.Value - a.X.Value) - (c.X.Value - a.X.Value) * (b.Y.Value - a.Y.Value));
            area = (1.0 / 2.0) * Math.Abs((b.X.Value - a.X.Value) * (c.Y.Value - a.Y.Value) - (c.X.Value - a.X.Value) * (b.Y.Value - a.Y.Value));
            bmat[0,0] = nax;
            bmat[0,2] = nbx;
            bmat[0,4] = ncx;
            bmat[1,1] = nay;
            bmat[1,3] = nby;
            bmat[1,5] = ncy;
            bmat[2,0] = na / xs;
            bmat[2,2] = nb / xs;
            bmat[2,4] = nc / xs;
            bmat[3,0] = nay;
            bmat[3,1] = nax;
            bmat[3,2] = nby;
            bmat[3,3] = nbx;
            bmat[3,4] = ncy;
            bmat[3,5] = ncx;

            return bmat;
        }

        /// <summary>
        /// Computes the K matrix for the element
        /// </summary>
        /// <returns>K-Matrix</returns>
        private Matrix KMatrix(FiniteElement element)
        {
            double area;

            var bMatrix = BMatrix(element.A, element.B, element.C, out area);
        	var bt = bMatrix.Transpose();
        	var btD = bt*d;
        	var k = btD*bMatrix;
			k = area * k;
            return k;
        }

        private Matrix StiffnessMatrix()
        {
            var stiffnessMatrix = new Matrix(2*nodes.Length, 2*nodes.Length);
        	
        	Action<IEnumerable<FiniteElement>> calculateStiffness = elementList =>
        	                            		{
													foreach (var element in elementList)
													{
														var k = KMatrix(element);
														var elementNodes = new[]
				                   							{
																element.A,
																element.B,
																element.C
				                   							};

														for (var i = 0; i < 3; ++i)
														{
															var rowPointIndex = Array.FindIndex(nodes, point => point.Equals(elementNodes[i]));

															for (var j = 0; j < 3; ++j)
															{
																var columnPointIndex = Array.FindIndex(nodes, point => point.Equals(elementNodes[j]));

																for (var ii = 0; ii < 2; ii++)
																{
																	var row = 2 * rowPointIndex + ii;

																	for (var jj = 0; jj < 2; jj++)
																	{
																		var column = 2 * columnPointIndex + jj;

																		lock (stiffnessMatrix)
																		{
																			stiffnessMatrix[row, column] = stiffnessMatrix[row, column] +  k[2*i+ii, 2*j+jj];
																		}
																	}
																}
															}
														}
													}
        	           							};

            
			//At least for now, maybe later I'll move the parallelism further up, maybe to linear equation solving.
			//At around 300 elements, the parallel and sequential version go head to head performance wise. When the element count is
			//higher than 300, the parallel version gets faster, up to three times at element count of 1100. Below the 300-element mark,
			//sequential version is quicker.
			if(config.Elements.Count > 300)
			{
        		var sublists = config.Elements.Sublist(Environment.ProcessorCount);

        		var options = new ParallelOptions {MaxDegreeOfParallelism = Environment.ProcessorCount};

				Parallel.ForEach(sublists, options, calculateStiffness);
			}
			else
			{
				calculateStiffness.Invoke(config.Elements);
			}
        	
		
			return stiffnessMatrix;
        }

        private void ModifyStiffnessMatrix(Matrix stiffnessMatrix)
        {
            var fixedNodes = Array.FindAll(nodes, node => node.X.IsFixed || node.Y.IsFixed);

            int row = 0;
            foreach(var node in fixedNodes)
            {
                var nodeIndex = Array.FindIndex(nodes, point => point.Equals(node));

                if(node.X.IsFixed && node.Y.IsFixed)
                {
                    for(var i = 0; i < 2; ++i)
                    {
                        row = 2*nodeIndex + i;
                        residVector[row] = 0;
                    }
                }
                else if (node.X.IsFixed)
                {
                    row = 2 * nodeIndex;
                    residVector[row] = 0;
                }
                else if (node.Y.IsFixed)
                {
                    row = 2*nodeIndex + 1;
                    residVector[row] = 0;
                }

                for(var i = 0; i < nodes.Length; ++i)
                {
                    stiffnessMatrix[row, i] = 0;
                }

                stiffnessMatrix[row, row] = 1;
            }
        }
		
        private double[] ComputeResidue()
        {
            var resid = new double[2*nodes.Length];

            var loadedNodes = Array.FindAll(nodes, node => node.Load != null);

            foreach(var node in loadedNodes)
            {
                var nodeIndex = Array.FindIndex(nodes, point => point.Equals(node));
                resid[2*nodeIndex] = resid[2*nodeIndex] + node.Load.X;
                resid[2*nodeIndex + 1] = resid[2*nodeIndex + 1] + node.Load.Y;
            }

            return resid;
        }

        private double[] ComputeGauss(Matrix stiffnessMatrix)
        {
            var resultVector = new double[2*nodes.Length];

	        int n = 2*nodes.Length;
            for (var k = 0; k < n - 1; k++)
            {
                for(var i = k+1; i < n; i++)
                {
                    double m = stiffnessMatrix[i,k]/stiffnessMatrix[k,k];
                    for (var j = k + 1; j < n; j++)
                    {
                        stiffnessMatrix[i, j] = stiffnessMatrix[i, j] - m*stiffnessMatrix[k, j];
                    }

                    residVector[i] = residVector[i] - m*residVector[k];
                    
                }
            }

            resultVector[n - 1] = residVector[n - 1]/stiffnessMatrix[n - 1, n - 1];

            for (var i = n - 2; i >= 0; i--)
            {
                resultVector[i] = residVector[i];

                for(var j = i+1; j<n;j++)
                {
                    resultVector[i] = resultVector[i] - stiffnessMatrix[i, j]*resultVector[j];
                }

                resultVector[i] = resultVector[i]/stiffnessMatrix[i, i];

            }

            return resultVector;
        }


		/// <summary>
		/// Generates the node list from finite elements' nodes.
		/// </summary>
		/// <returns>Dictionary of pairs: ordinal number of the node, Point object of the node with coordinates</returns>
		public Point[] GenerateNodeList()
		{
			var points = new List<Point>();
    		foreach(var element in config.Elements)
    		{
    			points.Add(element.A);
				points.Add(element.B);
				points.Add(element.C);
    		}

			var duplicatePoints = points.GroupBy(e => e, new PointComparer()).Where(e => e.Count() > 1).ToList();

			foreach (var items in duplicatePoints)
			{
				var load = new Force();
			    double? fixedX = null;
                double? fixedY = null;
			    var itemlist = items.ToList();
				foreach (var item in itemlist)
				{
				    if (item.Load != null)
				    {
				        load.X += item.Load.X;
				        load.Y += item.Load.Y;
				    }

				    if (item.X.IsFixed && fixedX.HasValue && item.X.Value != fixedX.Value)
				    {
				        throw new InvalidOperationException("Element configuration error: Two or more duplicate nodes have different values fixed on X node.");
				    }

                    if(item.Y.IsFixed && fixedY.HasValue && item.Y.Value != fixedY.Value)
                    {
                        throw new InvalidOperationException("Element configuration error: Two or more duplicate nodes have different values fixed on Y node.");
                    }

			        if (item.X.IsFixed)
                        fixedX = item.X.Value;

                    if (item.Y.IsFixed)
                        fixedY = item.Y.Value;
				}


				foreach (var item in items)
				{
                    if(load.X != 0 || load.Y != 0)
						item.Load = load;

                    if(fixedY.HasValue)
                    {
                        item.Y.IsFixed = true;
                        item.Y.Value = fixedY.Value;
                    }

                    if(fixedX.HasValue)
                    {
                        item.X.IsFixed = true;
                        item.X.Value = fixedX.Value;
                    }
				}
				
                
			}

			var distinctPoints = points.Distinct(new PointComparer()).ToArray();

			if (!distinctPoints.Any()) return null;

			return distinctPoints;
		}

		#region ISimulation members

		/// <summary>
		/// Runs the simulation
		/// </summary>
		public double[] Run()
		{
            if (simulationReady == false) return null;

			var stiffness = StiffnessMatrix();
		    residVector = ComputeResidue();
            ModifyStiffnessMatrix(stiffness);

		    var result = ComputeGauss(stiffness);

		    return result;
		}

		private void CreateDebugFile(Matrix matrix)
		{
			using (var textWriter = new StreamWriter("C:\\matrixDump.txt"))
			{
				matrix.Print(textWriter.WriteLine);
				textWriter.Flush();
				textWriter.Close();
			}
		}

		private void WriteResults(double[] results)
		{
			using (var textWriter = new StreamWriter("C:\\results.txt")) 
			{
				for (int i = 0; i < results.Length; ++i)
				{
					textWriter.WriteLine(String.Format("u[{0}] = {1}", i, results[i]));
				}
				textWriter.Flush();
				textWriter.Close();
			}
		}

        public void LoadConfiguration(Configuration configuration)
        {
            if (configuration.Coefficients == null)
                throw new ArgumentNullException("configuration.Coefficients");

            if (configuration.Elements == null)
            {
                throw new ArgumentException("Element list is null.");
            }
            config = configuration;
            d = DMatrix(config.Coefficients.E, config.Coefficients.Nu);
            nodes = GenerateNodeList();
            simulationReady = true;
        }

		public Point[] GetNodeList()
		{
			return nodes;
		}

        public int ElementCount()
        {
            return config.Elements.Count;
        }
		#endregion
	}
}
