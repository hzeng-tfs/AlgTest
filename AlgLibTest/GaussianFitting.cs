using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgLibTest
{
   class GaussianFitting
   {
      /// this callback calculates Gaussian function f(c,x) = c0 + c1*EXP[-(x-c2)^2/2*c3^2]
      /// c0, c1, c2 and c3 are the background, amplitude, centre and width of the Gaussians.
      /// where x is a position on X-axis and c is adjustable parameter
      public static void GaussianFunc(double[] c, double[] x, ref double func, object obj)
      {
         double arg = (x[0]-c[2])/c[3];
         double ex  = System.Math.Exp(-arg*arg/2);
         func = c[1]*ex + c[0];
      }

      /// Note: x is an array but has only one element
      /// this callback calculates Gaussian function f(c,x) = c0 + c1*EXP[-(x-c2)^2/2*c3^2] and gradient G={df/dc[i]}
      /// where x is a position on X-axis and c is adjustable parameter.
      /// IMPORTANT: gradient is calculated with respect to C, not to X
      public static void GaussianFuncGrad(double[] c, double[] x, ref double func, double[] grad, object obj)
      {
         //optimised for better calculation performance
         double arg  = (x[0]-c[2])/c[3];
         double ex   = System.Math.Exp(-arg*arg/2);
         double fac  = c[1]*ex*arg;

         func    = c[1]*ex + c[0];
         grad[0] = 1;                    // dy/dc0, derivative of y with respect to c0.
         grad[1] = ex;                   // dy/dc1, derivative of y with respect to c1.
         grad[2] = fac/c[3];             // dy/dc2, derivative of y with respect to c2.
         grad[3] = fac*arg/c[3];         // dy/dc3, derivative of y with respect to c3.
      }

      // adds noise to original value, up to +/- given precentage of original value
      public static double AddNoise(double input, double pct = 0.05)
      {
         Random rand = new Random();
         double d = rand.NextDouble();
         if (d > 0.5)
            return input*(1 + (d-0.5)/0.5*pct);
         else
            return input*(1 - d/0.5*pct);
      }

      private static void PrintResults(double[] c, int errorCode, alglib.lsfitreport report)
      {
         if (errorCode == 2) { // fitting successfully
            Console.WriteLine("fitting coefficients: {0}", alglib.ap.format(c, 1)); // EXPECTED: [1.5]
            Console.WriteLine();
            Console.WriteLine("{0,8:N5} R2          non-adjusted coefficient of determination (non-weighted)", report.r2);
            Console.WriteLine("{0,8:N5} RMSError    rms error on the(X, Y).", report.rmserror);
            Console.WriteLine("{0,8:N5} AvgError    average error on the(X, Y).", report.avgerror);
            Console.WriteLine("{0,8:N5} AvgRelError average relative error on the non-zero Y", report.avgrelerror);
            Console.WriteLine("{0,8:N5} MaxError    maximum error NON-WEIGHTED ERRORS ARE CALCULATED", report.maxerror);
            Console.WriteLine("{0,8:N5} WRMSError   weighted rms error on the(X, Y).", report.wrmserror);

         }
         else {
            Console.WriteLine("fitting returns error code {0}", errorCode);
            Console.WriteLine("-8 optimizer   detected  NAN/INF  in  the  target function and/or gradient");
            Console.WriteLine("-7 gradient verification failed.");
            Console.WriteLine("-3 inconsistent constraints");
            Console.WriteLine(" 2 relative step is no more than EpsX.");
            Console.WriteLine(" 5 MaxIts steps was taken");
            Console.WriteLine(" 7 stopping conditions are too stringent, further improvement is impossible");
         }
         Console.WriteLine();
      }


      public static void Test()
      {
         // prepare data
         double[] realCoeffs = new double[] {10, 300, 120, 15};
         double[,] realX = new double[121,1];
         int step = 0;
         for(int i=0; i<realCoeffs[2]+1; i++) {
            realX[i, 0] = step;
            step += 2;
         }

         double[] noiseLevels = new double[] {0.02, 0.05, 0.1};
         foreach (double noiseLevel in noiseLevels) {
            double max = double.MinValue;
            int maxPos = 0;
            double[] realY = new double[realX.GetLength(0)];
            for (int i = 0; i<realY.Length; i++) {
               double result = 0;
               GaussianFunc(realCoeffs, new double[] { realX[i, 0] }, ref result, null);
               realY[i] = AddNoise(result, noiseLevel);

               if (i % 10 == 5)
                  realY[i] = AddNoise(result, noiseLevel*3);       //add bad data

               if (realY[i] > max) {  //find max value and position for initial guess.
                  max = realY[i];
                  maxPos = (int)realX[i, 0];
               }
            }

            //double[] c = new double[]{0, 100, 0, 5};  //bad initial guess gives bad results
            double[] c = new double[]{0, max, maxPos, 5};  //initial guess, also the final results
            Console.WriteLine("Noise Level up to {0:P} and bad data @ every 6th out of 10", noiseLevel);
            Console.WriteLine("initial guess is    : {0} {{background, amplitude, centre and width}}", alglib.ap.format(c, 1));
            Console.WriteLine("real    coefficients: {0}", alglib.ap.format(realCoeffs, 1));

            alglib.lsfitstate state;
            alglib.lsfitcreatefg(realX, realY, c, true, out state);    //using function values and gradient

            //set Stopping conditions for nonlinear least squares fitting.
            double epsx = 0.000001;  //step improvement
            int maxits = 0;   //maximum number of iterations; 0=unlimited
            alglib.lsfitsetcond(state, epsx, maxits);

            alglib.lsfitfit(state, GaussianFunc, GaussianFuncGrad, null, null);

            //get results from fitting
            int errorCode;  //info: completion code, 2 is good
            alglib.lsfitreport report;
            alglib.lsfitresults(state, out errorCode, out c, out report);
            PrintResults(c, errorCode, report);
         }
      }

      //from spectra file 20160628_145212.csv
      // according spectra file, the first count value (32) is at bin 475
      private static int _beginFeKaFull = 475;
      private static double[] _peakFeKaFull = new double[] {32, 44, 43, 50, 41, 53, 43, 50, 51, 67, 82, 90, 103, 138, 210, 311, 400, 523, 698, 886,
                                       1245, 1532, 1882, 2152, 2350, 2629, 2745, 2718, 2578, 2329, 2075, 1750, 1527, 1196, 876,
                                       633, 466, 286, 204, 135, 81, 38, 17, 28, 12, 10, 10, 13 };

      private static int _beginFeKaPartial = 491;  //first count (400) is at bin 491
      private static double[] _peakFeKaPartial = new double[] {400, 523, 698, 886,
                                       1245, 1532, 1882, 2152, 2350, 2629, 2745, 2718, 2578, 2329, 2075, 1750, 1527, 1196, 876,
                                       633, 466 };

      private static int _beginBrKaFull = 816;
      private static double[] _peakBrKaFull = new double[] {19, 11, 11, 19, 23, 22, 15, 25, 20, 27,
        28, 38, 34, 29, 44, 58, 55, 74, 104, 122, 145, 195, 227, 309, 378, 464, 476, 477, 613, 575, 616, 641, 580, 551, 522, 468, 439,
        359, 286, 256, 183, 129, 105, 95, 78, 46, 42, 31, 26, 17, 17, 17, 7, 20, 14, 18, 11, 18, 11, 15, 17, 14 };

      private static int _beginBrKaPartial = 835;
      private static double[] _peakBrKaPartial = new double[] {122, 145, 195, 227, 309, 378, 464, 476, 477, 613, 575, 616, 641, 580, 551, 522, 468, 439,
        359, 286, 256, 183, 129, 105};

      private static int _beginPuLbRayPartial = 1206;
      private static double[] _peakPuLbRayPartial = new double[] {51, 56, 80, 83, 88, 97, 118, 120, 151, 155, 196, 205, 233, 254, 303, 283, 320,
        318, 316, 297, 305, 268, 280, 239, 237, 184, 191, 138, 109, 76, 85, 56, 63, 40, 33, 19, 18, 20, 14, 20, 11};



      public static void SpecTest(double[] realY, int begin, double ctrd, double fwhm)
      {
         //FeKb on above spectra file
         //double[] realY = new double[] {32, 44, 43, 50, 41, 53, 43, 50, 51, 67, 82, 90, 103, 138, 210, 311, 400, 523, 698, 886,
         //                               1245, 1532, 1882, 2152, 2350, 2629, 2745, 2718, 2578, 2329, 2075, 1750, 1527, 1196, 876,
         //                               633, 466, 286, 204, 135, 81, 38, 17, 28, 12, 10, 10, 13 };
         double[,] realX = new double[realY.Length, 1];
         double max = double.MinValue;
         int maxPos = 0;
         for (int i=0; i<realX.Length; i++) {
            realX[i,0] = begin + i;
            if (realY[i] > max) {  //find max value and position for initial guess.
               max = realY[i];
               maxPos = (int)realX[i,0];
            }
         }

         //double[] c = new double[]{0, 100, 0, 5};  //bad initial guess gives bad results
         double[] c = new double[]{0, max, maxPos, 8};  //initial guess, also the final results
         Console.WriteLine("initial guess is    : {0}", alglib.ap.format(c, 1));

         alglib.lsfitstate state;
         alglib.lsfitcreatefg(realX, realY, c, true, out state);    //using function values and gradient

         //set Stopping conditions for nonlinear least squares fitting.
         double epsx = 0.000001;  //step improvement
         int maxits = 0;   //maximum number of iterations; 0=unlimited
         alglib.lsfitsetcond(state, epsx, maxits);

         alglib.lsfitfit(state, GaussianFunc, GaussianFuncGrad, null, null);

         //get results from fitting
         int errorCode;  //info: completion code, 2 is good
         alglib.lsfitreport report;
         alglib.lsfitresults(state, out errorCode, out c, out report);

         PrintResults(c, errorCode, report);

         const double ratio = 2.35482;    //  fwhm = 2*sqrt(2*ln(2))*a3 = 2.35482*a3
         Console.WriteLine("AlgLib    result: Ctrd = {0,8:N2}, FWHM = {1,8:N2}", c[2], c[3]*ratio);
         Console.WriteLine("WinISA NR result: Ctrd = {0,8:N2}, FWHM = {1,8:N2}", ctrd, fwhm);



      }

      public static void SpecTest()
      {
         Console.WriteLine("-----------  Testing with real spectrum: FeKa full peak, strong, PeakWidth={0}-----------------", _peakBrKaFull.Length);
         SpecTest(_peakFeKaFull, _beginFeKaFull, 501.91, 11.73);

         Console.WriteLine("\n--------------  Testing with real spectrum: FeKa partial peak, strong, PeakWidth={0}-----------------", _peakFeKaPartial.Length);
         SpecTest(_peakFeKaPartial, _beginFeKaPartial, 501.91, 11.73);

         Console.WriteLine("\n--------------  Testing with real spectrum: BrKa full peak, weak, PeakWidth={0}-----------------", _peakBrKaFull.Length);
         SpecTest(_peakBrKaFull, _beginBrKaFull, 846.88, 15.17);

         Console.WriteLine("\n--------------  Testing with real spectrum: BrKa Partial peak, weak, PeakWidth={0}-----------------", _peakBrKaPartial.Length);
         SpecTest(_peakBrKaPartial, _beginBrKaPartial, 846.88, 15.17);

         Console.WriteLine("\n--------------  Testing with real spectrum: PuLbRay Partial peak, very weak, PeakWidth={0}-----------------", _peakPuLbRayPartial.Length);
         SpecTest(_peakPuLbRayPartial, _beginPuLbRayPartial, 1224.22, 16.32);
      }

   }
}
