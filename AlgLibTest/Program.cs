using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgLibTest
{
   class Program
   {
      static void Main(string[] args)
      {
         Console.WriteLine("---------  Start of Linear Regression Test  --------");
         Console.WriteLine("---  Y = c0*X0 + c1*X1 + c2*X2 + c4  --------");
         LinearRegression.LinearRegressionTest();
         Console.WriteLine("---------  End of Test --------\n");

         Console.WriteLine("---------  Start of Gaussian Fitting Test  --------");
         GaussianFitting.Test();
         GaussianFitting.SpecTest();
         Console.WriteLine("---------  End of Test --------\n");


         Console.WriteLine("---------  Start of PSM500 data Test  --------");
         PSM500Test.Test();
         Console.WriteLine("---------  End of Test --------\n");

         Console.ReadLine();

      }


   }

}
