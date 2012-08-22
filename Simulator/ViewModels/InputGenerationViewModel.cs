using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using Caliburn.Micro;
using Miscellaneous;

namespace Simulator.ViewModels
{
	public class InputGenerationViewModel : Screen, IDataErrorInfo
	{
		private readonly HashSet<string> invalidEntries = new HashSet<string>();
		private double outerRadius;
		private double innerRadius;
		private double elasticityCoefficient;
		private double poissonCoefficient;
		private double gammaAngle;
		private double pressure;
		private string filename;
		private uint finiteElementCount;

		public double OuterRadius
		{
			get { return outerRadius; }
			set
			{
				if (value.Equals(outerRadius)) return;
				outerRadius = value;
				NotifyOfPropertyChange(() => OuterRadius);
			}
		}

		public double InnerRadius
		{
			get { return innerRadius; }
			set
			{
				if (value.Equals(innerRadius)) return;
				innerRadius = value;
				NotifyOfPropertyChange(() => InnerRadius);
			}
		}

		public double ElasticityCoefficient
		{
			get { return elasticityCoefficient; }
			set
			{
				if (value.Equals(elasticityCoefficient)) return;
				elasticityCoefficient = value;
				NotifyOfPropertyChange(() => ElasticityCoefficient);
			}
		}

		public double PoissonCoefficient
		{
			get { return poissonCoefficient; }
			set
			{
				if (value.Equals(poissonCoefficient)) return;
				poissonCoefficient = value;
				NotifyOfPropertyChange(() => PoissonCoefficient);
			}
		}

		public double GammaAngle
		{
			get { return gammaAngle; }
			set
			{
				if (value.Equals(gammaAngle)) return;
				gammaAngle = value;
				NotifyOfPropertyChange(() => GammaAngle);
			}
		}

		public double Pressure
		{
			get { return pressure; }
			set
			{
				if (value.Equals(pressure)) return;
				pressure = value;
				NotifyOfPropertyChange(() => Pressure);
			}
		}

		public string Filename
		{
			get { return filename; }
			set
			{
				if (value == filename) return;
				filename = value;
				NotifyOfPropertyChange(() => Filename);
			}
		}

		public uint FiniteElementCount
		{
			get { return finiteElementCount; }
			set
			{
				if (value == finiteElementCount) return;
				finiteElementCount = value;
				NotifyOfPropertyChange(() => FiniteElementCount);
			}
		}

		public bool CanGenerate { get { return !invalidEntries.Any(); } }

		public void Generate()
		{
			TryClose(true);
		}

		public InputGenerationViewModel()
		{
			OuterRadius = 2e-3;
			InnerRadius = 1e-3;
			FiniteElementCount = 10;
			ElasticityCoefficient = 1000;
		}

		public string Error
		{
			get { throw new System.NotImplementedException(); }
		}

		public string this[string columnName]
		{
			get
			{
				var message = string.Empty;
				if(columnName == Property.GetName(() => OuterRadius))
				{
					if (OuterRadius <= 0)
					{
						message = "Outer radius is negative or zero";
					}
					else if (OuterRadius <= InnerRadius)
					{
						message = "Outer radius is smaller or equal to inner radius";
					}
				}
				else if(columnName == Property.GetName(() => InnerRadius))
				{
					if (InnerRadius <= 0)
					{
						message = "Inner radius is negative or zero";
					}
					else if (InnerRadius >= OuterRadius)
					{
						message = "Inner radius is greater than or equal to outer radius";
					}
				}
				else if(columnName == Property.GetName(() => FiniteElementCount))
				{
					if (FiniteElementCount < 6)
						message = "Finite element count has to be 6 or more";
				}
				else if(columnName == Property.GetName(() => ElasticityCoefficient))
				{
					if (ElasticityCoefficient <= 0)
						message = "Elasticity coefficient should be greater than zero";
				}
				else if(columnName == Property.GetName(() => Filename))
				{
					if (!string.IsNullOrEmpty(Filename))
					{
						var invalidChars = Path.GetInvalidFileNameChars();
						var filenameCharArray = Filename.ToCharArray();

						if (filenameCharArray.Any(character => invalidChars.Any(invalidCharacter => character == invalidCharacter)))
						{
							message = "Invalid characters found in filename.";
						}
					}
				}
				
				if(string.IsNullOrEmpty(message))
				{
					invalidEntries.Remove(columnName);
				}
				else
				{
					invalidEntries.Add(columnName);
				}

				NotifyOfPropertyChange(() => CanGenerate);
				return message;
			}
		}
	}
}
