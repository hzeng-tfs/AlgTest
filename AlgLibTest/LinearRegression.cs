using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgLibTest
{
   class LinearRegression
   {
      public class func_4
      {
         private double[] _coeffs = new double[] {0, 0, 0, 0};
         public double Result(double x1, double x2, double x3)
         {
            return _coeffs[0]*x1 + _coeffs[1]*x2 + _coeffs[2]*x3 + _coeffs[3];
         }
         public string PrintCoeffs()
         {
            return alglib.ap.format(_coeffs, 4);
         }

         public func_4(double[] coeffs)
         {
            coeffs.CopyTo(_coeffs, 0);
         }
      }


      // pct noise level
      public static double AddNoise(double input, double pct = 0.05)
      {
         Random rand = new Random();
         double d = rand.NextDouble();
         if (d > 0.5)
            return input*(1 + (d-0.5)/0.5*pct);
         else
            return input*(1 - d/0.5*pct);
      }

      public static void LinearRegressionTest()
      {
         // In       this example we demonstrate linear fitting by y = a0X0 + a1X1 + a2X2 + a3
         double[,] xy = new double[,]{
         //  x1,     x2,    x3,    y
            { 1,     2,     3,     0},
            { 2,     3,     4,     0 },
            { 1,     1,     1,     0 },
            { 3.4,   5.6,   7.8,   0 },
            { 1.2,   2,     3,     0 },
            { 5,     2,     5,     0 },
            { 12,    2,     13,    0 },
            { 10,    12,    5,     0 },
            { 21,    12,    15,     0 },
            { 12,    21,    15,     0 },
            { 10,    50,   30,     0 }
         };
         double[] realvalue = new double[xy.GetLength(0)];
         func_4 func = new func_4(new double[] {1.1, 2.1, 3.45, 2.4});

         StringBuilder sb = new StringBuilder();
         sb.Append("real value   :");
         for (int i = 0; i<xy.GetLength(0); i++) {
            realvalue[i] = func.Result(xy[i, 0], xy[i, 1], xy[i, 2]);
            sb.AppendFormat("{0,8:N3} ", realvalue[i]);
         }
         sb.Append("\n");

         double[] noiseLevels = new double[] {0, 0.02, 0.05, 0.1};
         foreach (double noiseLevel in noiseLevels) {
            //add noise
            for (int i = 0; i<xy.GetLength(0); i++) {
               xy[i, 3] = AddNoise(realvalue[i], noiseLevel);
               //add bad data
               if (i % 10 == 6)
                  xy[i, 3] = AddNoise(realvalue[i], 0.25);
            }


            Console.Write("\nNoise Level up to {0:P} and bad data @ every 7th out of 10\n", noiseLevel);
            Console.Write(sb.ToString());
            Console.Write("value+noise  :");
            for (int i = 0; i<xy.GetLength(0); i++) {
               Console.Write("{0,8:N3} ", xy[i, 3]);
            }
            Console.Write("\n");

            int errorCode;
            int nvars = 3;  //number of independent variables
            int npoints = xy.GetLength(0);
            alglib.linearmodel model;
            alglib.lrreport report;
            double[] coefficients;

            alglib.lrbuild(xy, npoints, nvars, out errorCode, out model, out report);

            if (errorCode == 1) { // EXPECTED: 1 (if subroutine successfully finished)
               alglib.lrunpack(model, out coefficients, out nvars);  //Unpacks coefficients from linear model.
               func_4 fittedFunc = new func_4(coefficients);
               Console.Write("fitted result:");
               for (int i = 0; i<xy.GetLength(0); i++) {
                  Console.Write("{0,8:N3} ", fittedFunc.Result(xy[i, 0], xy[i, 1], xy[i, 2]));
               }
               Console.Write("\n");

               Console.WriteLine("expecting         : {0}", func.PrintCoeffs());
               Console.WriteLine("regression results: {0}", alglib.ap.format(coefficients, 4));

               //Console.WriteLine("covariation matrix {0}", alglib.ap.format(report.c, 4)); 
               Console.WriteLine("{0,8:N5} RMSError (root mean square error on a training set)", report.rmserror);
               Console.WriteLine("{0,8:N5} AvgError (average error on a training set)", report.avgerror);
               Console.WriteLine("{0,8:N5} AvgRelError (average relative error on a training set(excluding observations with zero function value)", report.avgrelerror);
               Console.WriteLine("{0,8:N5} CVRMSError (leave-one-out cross-validation estimate of generalization error)", report.cvrmserror);
               Console.WriteLine("{0,8:N5} CVAvgError (cross-validation estimate of average error)", report.cvavgerror);
               Console.WriteLine("{0,8:N5} CVAvgRelError (cross-validation estimate of average relative error)", report.cvavgrelerror);
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
}
