namespace Simulator.Internals
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class Coefficients
    {
        /// <summary>
        /// Poisson's coefficient
        /// </summary>
        public double Nu { get; set; }


        /// <summary>
        /// Elasticity coefficient [Pa]
        /// </summary>
        public double E { get; set; }


        /// <summary>
        /// Angle of cornea dilation [radians]
        /// </summary>
        public double Gamma { get; set; }

        /// <summary>
        /// Pressure exerted on cornea (negative value from the outside, positive value from the inside) [Pa]
        /// </summary>
        public double P { get; set; }
    }
}
