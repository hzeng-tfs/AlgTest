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


      public static void Test()
      {
         double[] realCoeffs = new double[] {10, 300, 120, 15};
         double[,] realX = new double[121,1];
         int step = 0;
         for(int i=0; i<121; i++) {
            realX[i, 0] = step;
            step += 2;
         }

         double[] realY = new double[realX.GetLength(0)];
         for (int i=0; i<realY.Length; i++) {
            double result = 0;
            GaussianFunc(realCoeffs, new double[] { realX[i,0] }, ref result, null);
            realY[i] = result;
         }

         //double[] c = new double[]{1, 250, 100, 10};  //initial guess
         double[] c = new double[]{0, 10, 100, 10};  //initial guess

         //
         // In this example we demonstrate exponential fitting
         // by f(x) = exp(-c*x^2)
         // using function value and gradient (with respect to c).
         //
         //double[,] x = new double[,]{{-1},{-0.8},{-0.6},{-0.4},{-0.2},{0},{0.2},{0.4},{0.6},{0.8},{1.0}};
         //double[] y = new double[]{0.223130,0.382893,0.582748,0.786628,0.941765,1.000000,0.941765,0.786628,0.582748,0.382893,0.223130};
         //double[] c = new double[]{0.3};  //initial guess
         alglib.lsfitstate state;

         //using function values and gradient
         alglib.lsfitcreatefg(realX, realY, c, true, out state);

         //set Stopping conditions for nonlinear least squares fitting.
         double epsx = 0.000001;  //step improvement
         int maxits = 0;   //maximum number of iterations; 0=unlimited
         alglib.lsfitsetcond(state, epsx, maxits);

         alglib.lsfitfit(state, GaussianFunc, GaussianFuncGrad, null, null);

         //get results from fitting
         //info: completion code, 2 is good
         //c: solution
         //rep: report
         int info;
         alglib.lsfitreport rep;
         alglib.lsfitresults(state, out info, out c, out rep);
         System.Console.WriteLine("{0}", info); // EXPECTED: 2
         System.Console.WriteLine("real    coefficients: {0}", alglib.ap.format(realCoeffs, 1)); 
         System.Console.WriteLine("fitting coefficients: {0}", alglib.ap.format(c, 1)); // EXPECTED: [1.5]

      }




   }
}
