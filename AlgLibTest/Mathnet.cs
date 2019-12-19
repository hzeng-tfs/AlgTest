using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.Optimization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


//testing of Math.Net | Numerics | Optimization | LevenbergMarquardtMinimizer.cs
namespace AlgLibTest
{
   class MathnetGaussianFitting
   {
      public static string ToTokenSeparatedString<TSource>(IEnumerable<TSource> source, string token = ",")
      {
         //return source == null ? "" : string.Join(token, source);
         return source == null ? "" : string.Join(token, source.Select(d => $"{d,7:F2}"));
      }

      /// this callback calculates Gaussian function f(c,x) = c0 + c1*EXP[-(x-c2)^2/2*c3^2]
      /// c0, c1, c2 and c3 are the background, amplitude, centre and width of the Gaussians.
      /// where x is a position on X-axis and c is adjustable parameter
      //public static void GaussianFunc(double[] c, double[] x, ref double func, object obj)
      //{
      //   double arg = (x[0] - c[2]) / c[3];
      //   double ex = System.Math.Exp(-arg * arg / 2);
      //   func = c[1] * ex + c[0];
      //}
      private static Vector<double> GaussianFunc(Vector<double> c, Vector<double> x)
      {
         var y = CreateVector.Dense<double>(x.Count);
         for (int i = 0; i < x.Count; i++) {
            double arg = (x[i] - c[2]) / c[3];
            double ex = System.Math.Exp(-arg * arg / 2);
            y[i] = c[1] * ex + c[0];
         }
         return y;
      }

      /// Note: x is an array but has only one element
      /// this callback calculates Gaussian function f(c,x) = c0 + c1*EXP[-(x-c2)^2/2*c3^2] and gradient G={df/dc[i]}
      /// where x is a position on X-axis and c is adjustable parameter.
      /// IMPORTANT: gradient is calculated with respect to C, not to X
      //public static void GaussianFuncGrad(double[] c, double[] x, ref double func, double[] grad, object obj)
      //{
      //   //optimised for better calculation performance
      //   double arg = (x[0] - c[2]) / c[3];
      //   double ex = System.Math.Exp(-arg * arg / 2);
      //   double fac = c[1] * ex * arg;

      //   func = c[1] * ex + c[0];
      //   grad[0] = 1;                    // dy/dc0, derivative of y with respect to c0.
      //   grad[1] = ex;                   // dy/dc1, derivative of y with respect to c1.
      //   grad[2] = fac / c[3];             // dy/dc2, derivative of y with respect to c2.
      //   grad[3] = fac * arg / c[3];         // dy/dc3, derivative of y with respect to c3.
      //}
      private static Matrix<double> GaussianPrime(Vector<double> c, Vector<double> x)
      {
         var prime = Matrix<double>.Build.Dense(x.Count, c.Count);
         for (int i = 0; i < x.Count; i++) {
            //optimised for better calculation performance
            double arg = (x[i] - c[2]) / c[3];
            double ex = System.Math.Exp(-arg * arg / 2);
            double fac = c[1] * ex * arg;

            //func = c[1] * ex + c[0];
            prime[i, 0] = 1;                    // dy/dc0, derivative of y with respect to c0.
            prime[i, 1] = ex;                   // dy/dc1, derivative of y with respect to c1.
            prime[i, 2] = fac / c[3];           // dy/dc2, derivative of y with respect to c2.
            prime[i, 3] = fac * arg / c[3];     // dy/dc3, derivative of y with respect to c3.
         }
         return prime;
      }

      //public static void SpecTest(double[] realY, int begin, double ctrd, double fwhm)
      public static void SpecTest(Sample sample)
      {
         Console.WriteLine($"------  Testing with real spectrum: {sample.description}, PeakWidth={sample.spectrum.Length} ------");

         //prepare data
         double[] realX = new double[sample.spectrum.Length];
         double max = double.MinValue;
         int maxPos = 0;
         for (int i = 0; i < realX.Length; i++) {
            realX[i] = sample.binOffset + i + 0.5;
            if (sample.spectrum[i] > max) {  //find max value and position for initial guess.
               max = sample.spectrum[i];
               maxPos = (int)realX[i];
            }
         }

         Vector<double> XValues = new DenseVector(realX);
         Vector<double> YValues = new DenseVector(sample.spectrum);
         Vector<double> initGuess = new DenseVector(new double[] { 0, max, maxPos, 8 });
         Console.WriteLine("initial guess is: {0}", ToTokenSeparatedString<double>(initGuess, ", "));

         var watch = System.Diagnostics.Stopwatch.StartNew();
         var obj = ObjectiveFunction.NonlinearModel(GaussianFunc, GaussianPrime, XValues, YValues);
         var solver = new LevenbergMarquardtMinimizer();
         var result = solver.FindMinimum(obj, initGuess);
         watch.Stop();

         Console.WriteLine($"Algorithm takes: {watch.ElapsedTicks} ticks {watch.ElapsedTicks*1000.0d/System.Diagnostics.Stopwatch.Frequency:F3} ms; " +
                           $"exit={result.ReasonForExit}; iterations={result.Iterations}");
         Console.WriteLine("results are     : {0}", ToTokenSeparatedString<double>(result.MinimizingPoint, ", "));
         Console.WriteLine("StdErrors are   : {0}", ToTokenSeparatedString<double>(result.StandardErrors, ", "));
         //Console.WriteLine("MinimizedValues : {0}", ToTokenSeparatedString<double>(result.MinimizedValues, ", "));
         Console.WriteLine("chi-square      : {0}", chisquare(YValues, result.MinimizedValues));

         const double ratio = 2.35482;    //  fwhm = 2*sqrt(2*ln(2))*a3 = 2.35482*a3
         Console.WriteLine("Math.NET  result: Ctrd = {0,8:N2}, FWHM = {1,8:N2}", result.MinimizingPoint[2], result.MinimizingPoint[3] * ratio);
         Console.WriteLine("WinISA NR result: Ctrd = {0,8:N2}, FWHM = {1,8:N2}", sample.ctrd, sample.fwhm);
      }

      public static void SpecTest()
      {
         SpecTest(TestData.FeKbFull);

         SpecTest(TestData.FeKbPartial);

         SpecTest(TestData.BrKaFull);

         SpecTest(TestData.BrKaPartial);

         SpecTest(TestData.PuLbRayPartial);

         SpecTest(TestData.CrKaNoGaussianFitting);
      }


      // Note:
      // - Math.NET doesn't provide chi-square on LevenbergMarquardtMinimizer
      // - need to calculated by ourselves
      //    * https://docs.microsoft.com/en-us/archive/msdn-magazine/2017/march/test-run-chi-squared-goodness-of-fit-using-csharp
      //    * possible helper: result.MinimizedValues

      public static double chisquare(Vector<double> observed, Vector<double> expected)
      {
         double sum = 0.0;
         double integral = 0.0;
         if (observed.Count != 0 && observed.Count == expected.Count) {
            for (int i=0; i<observed.Count; i++) {
               if (expected[i] != 0) {
                  sum += ((observed[i] - expected[i]) *(observed[i] - expected[i])) / expected[i];
                  integral += observed[i];
               }
            }
            //sum = Math.Sqrt(sum / integral);
         }
         return sum;
      }
   }

   class MathNetRegression
   {

   }

}
