using System;
using System.Text;

namespace Simulator.Internals
{
    /// <summary>
    /// Two dimensional array class
    /// </summary>
    public sealed class Matrix
    {
        /// <summary>
        /// Delegate to print the matrix (to the console/log/etc.). Assumes that the each argument line is printed with the line break
        /// added automatically (so equivalent to e.g. Console.Out.WriteLine()). 
        /// </summary>
        /// <param name="arg">One line to print</param>
        public delegate void PrintingFunction(string arg);

        /// <summary>
        /// Wrapped main array
        /// </summary>
        public double[][] Array { get; private set; }

        /// <summary>
        /// Row count
        /// </summary>
        public int RowCount { get; private set; }

        /// <summary>
        /// Column count
        /// </summary>
        public int ColumnCount { get; private set; }

        /// <summary>
        /// Creates a new two-dimensional matrix of doubles.
        /// </summary>
        /// <param name="rows">X-dimension, defaults to 10</param>
        /// <param name="columns">Y-dimension, defaults to 10</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when either row or column count is 0</exception>
        public Matrix(int rows = 10, int columns = 10)
        {
            if (rows == 0)
                throw new ArgumentOutOfRangeException("rows");
            if (columns == 0)
                throw new ArgumentOutOfRangeException("columns");

            RowCount = Math.Abs(rows);
            ColumnCount = Math.Abs(columns);

        	Array = new double[RowCount][];
        	var upperBound = Array.GetUpperBound(0) + 1;
			for(var i = 0; i < upperBound; ++i)
			{
				Array[i] = new double[ColumnCount];
			}
        }

        /// <summary>
        /// Transposes the matrix
        /// </summary>
        /// <returns>New, transposed instance of Matrix class</returns>
        public Matrix Transpose()
        {
            var newMatrix = new Matrix(ColumnCount, RowCount);

            for (int row = 0; row < RowCount; ++row)
            {
                for (int column = 0; column < ColumnCount; ++column)
                {
                    newMatrix[column, row] = Array[row][column];
                }
            }

            return newMatrix;
        }

        /// <summary>
        /// Overloaded array access operator
        /// </summary>
        /// <param name="row">first index</param>
        /// <param name="column">second index</param>
        /// <returns>An array element at [row, column] index</returns>
        public double this[int row, int column]
        {
           get
           {
               return Array[row][column];
           }
           set
           {
               Array[row][column] = value;
           }
        }

        /// <summary>
        /// Overloaded multiplication operator for matrices multiplication
        /// </summary>
        /// <param name="matrix1">First matrix</param>
        /// <param name="matrix2">Second matrix</param>
        /// <exception cref="ArgumentException">Thrown when the dimensions don't match for matrix multiplication</exception>
        /// <returns>New instance of Matrix class=</returns>
        public static Matrix operator * (Matrix matrix1, Matrix matrix2)
        {
            if(matrix1 == null)
                throw new ArgumentNullException("matrix1");
            if(matrix2 == null)
                throw new ArgumentNullException("matrix2");

            if(matrix1.ColumnCount != matrix2.RowCount)
            {
				var message = String.Format("Wrong array dimensions, array 1 column count: {0}, array 2 row count: {1}", matrix1.ColumnCount, matrix2.RowCount);
                throw new ArgumentException(message);
            }

            var result = new Matrix(matrix1.RowCount, matrix2.ColumnCount);

            for (int row = 0; row < result.RowCount; ++row)
            {
                for (int column = 0; column < result.ColumnCount; ++column)
                {
                    result[row, column] = 0;
                    for (int i = 0; i < matrix1.ColumnCount; i++)
                    {
                        result[row, column] = result[row, column] + matrix1[row, i] * matrix2[i, column];
                    }
                }
            }
            return result;
        }

		public static Matrix operator + (Matrix firstMatrix, Matrix secondMatrix)
		{
			if((secondMatrix.RowCount != firstMatrix.RowCount) || (secondMatrix.ColumnCount != firstMatrix.ColumnCount))
				throw new ArgumentException("Matrices are not of the same size");

			var resultMatrix = new Matrix(firstMatrix.RowCount, firstMatrix.ColumnCount);
			for(var row = 0; row < resultMatrix.RowCount; ++row)
			{
				for(var column=0; column < resultMatrix.ColumnCount; ++column)
				{
					resultMatrix[row, column] = firstMatrix[row, column] + secondMatrix[row, column];
				}
			}
			return resultMatrix;
		}

        /// <summary>
        /// Overloaded multiplication operator for matrix and variable multiplication
        /// </summary>
        /// <param name="variable">A variable</param>
        /// <param name="matrix">Matrix</param>
        /// <returns>A new instance of Matrix class</returns>
        public static Matrix operator * (double variable, Matrix matrix)
        {
            if(matrix == null)
                throw new ArgumentNullException("matrix");

            var result = new Matrix(matrix.RowCount, matrix.ColumnCount);

            for (int row = 0; row < result.RowCount; ++row)
            {
                for (int column = 0; column < result.ColumnCount; ++column)
                {
                    result[row, column] = matrix[row, column]*variable;
                }
            }
            return result;
        }

        /// <summary>
        /// Prints the matrix using the specified function pointer
        /// </summary>
        /// <param name="print">Method used to print the rows of the matrix</param>
        public void Print(PrintingFunction print)
        {
            var stringBuilder = new StringBuilder();
            for(int row = 0; row < RowCount; ++row)
            {
                for(int column = 0; column < ColumnCount; ++column)
                {
					stringBuilder.Append(String.Format("[{0},{1}] ", row, column));
                    stringBuilder.Append(Array[row][column]);
                    stringBuilder.Append(" ");
                }
                print.Invoke(stringBuilder.ToString().Trim());
                stringBuilder.Clear();
            }
        }
    }

}
