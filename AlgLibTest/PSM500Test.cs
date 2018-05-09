using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgLibTest
{
   class PSM500Test
   {
      public class func_2
      {
         private double[] _coeffs = new double[] {0, 0};
         public double Result(double x1)
         {
            return _coeffs[0]*x1 + _coeffs[1];
         }
         public string PrintCoeffs()
         {
            return alglib.ap.format(_coeffs, 4);
         }

         public func_2(double[] coeffs)
         {
            coeffs.CopyTo(_coeffs, 0);
         }
      }

      public static void Test()
      {
         // In       this example we demonstrate linear fitting by y = a0X0 + a1
         double[,] xy = new double[,]{
         //  x1,         y
            {29.5702,  43.15 },
            {41.4866,  47.20 },
            {39.6212,  47.62 },
            {43.2793,  47.10 },
            {43.1339,  46.75 },
            {37.5421,  46.45 },
            {40.5699,  46.95 },
            {39.9520,  47.04 },
            {39.4270,  45.31 },
            {44.0685,  47.04 },
            {45.7727,  48.47 },
            {34.1644,  44.77 },
            {31.4696,  43.77 },
            {56.8353,  51.26 },
            {56.9244,  51.70 },
            {57.3409,  52.41 },
            {59.3951,  52.55 },
         };

         double[] y = new double[xy.GetLength(0)];
         for (int i=0; i<y.Length; i++) {
            y[i] = xy[i, 1];
         }

         int errorCode;
         int nvars = 1;  //number of independent variables: X0
         int npoints = xy.GetLength(0);  //length of data set
         alglib.linearmodel model;
         alglib.lrreport report;
         double[] coefficients;

         alglib.lrbuild(xy, npoints, nvars, out errorCode, out model, out report);

         if (errorCode == 1) { // EXPECTED: 1 (if subroutine successfully finished)
            alglib.lrunpack(model, out coefficients, out nvars);  //Unpacks coefficients from linear model.

            func_2 fittedFunc = new func_2(coefficients);
            Console.WriteLine(" lab result   fitted result");
            for (int i = 0; i<xy.GetLength(0); i++) {
               Console.WriteLine("  {0,8:N3}    {1,8:N3}", xy[i, 1], fittedFunc.Result(xy[i, 0]));
            }
            Console.WriteLine("");
            //Console.Write("fitted result:");
            //for (int i = 0; i<xy.GetLength(0); i++) {
            //   Console.Write(" ", fittedFunc.Result(xy[i, 0]));
            //}
            //Console.Write("\n");

            double mean;
            double variance;
            double skewness;
            double kurtosis;
            alglib.samplemoments(y, out mean, out variance, out skewness, out kurtosis);
            double RSquared = 1 - report.rmserror*report.rmserror/(variance*(y.Length-1)/y.Length);  // N-1 and N

            double SEE = report.rmserror*Math.Sqrt((double)y.Length/(y.Length-2));

            Console.WriteLine("regression results: {0}", alglib.ap.format(coefficients, 4));

            Console.WriteLine("Standard Error      {0,8:N5}, {1,8:N5}", Math.Sqrt(report.c[0,0]), Math.Sqrt(report.c[1, 1]));
            Console.WriteLine("T statistic         {0,8:N5}, {1,8:N5}", coefficients[0]/Math.Sqrt(report.c[0, 0]), coefficients[1]/Math.Sqrt(report.c[1, 1]));

            Console.WriteLine("{0,8:N5} SEE", SEE);
            Console.WriteLine("{0,8:N5} RSquared", RSquared);
            Console.WriteLine("{0,8:N5} RMSError (root mean square error on a training set)", report.rmserror);
            Console.WriteLine("{0,8:N5} AvgError (average error on a training set)", report.avgerror);
            Console.WriteLine("{0,8:N5} AvgRelError (average relative error on a training set(excluding observations with zero function value)", report.avgrelerror);
            Console.WriteLine("{0,8:N5} CVRMSError (leave-one-out cross-validation estimate of generalization error)", report.cvrmserror);
            Console.WriteLine("{0,8:N5} CVAvgError (cross-validation estimate of average error)", report.cvavgerror);
            Console.WriteLine("{0,8:N5} CVAvgRelError (cross-validation estimate of average relative error)", report.cvavgrelerror);
            Console.WriteLine("covariation matrix  {0}", alglib.ap.format(report.c, 4));
         }
         else {
            Console.WriteLine("math function returns error code {0}", errorCode);
            Console.WriteLine("*-255, in case of unknown internal error");
            Console.WriteLine("*  -4, if internal SVD subroutine haven't converged");
            Console.WriteLine("*  -1, if incorrect parameters was passed(NPoints<NVars+2, NVars<1).");
         }


      }
   }
}
