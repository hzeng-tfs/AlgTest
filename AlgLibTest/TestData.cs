using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgLibTest
{
   public class Sample
   {
      public int binOffset;
      public double[] spectrum;
      public double ctrd;
      public double fwhm;
      public string description;

      public Sample(string description, double[] spectrum, int binOffset, double ctrd, double fwhm)
      {
         this.description = description;
         this.spectrum = spectrum;
         this.binOffset = binOffset;
         this.ctrd = ctrd;
         this.fwhm = fwhm;
      }
   }

   public static class TestData
   {
      //from spectra file 20160628_145212.csv
      // according spectra file, the first count value (32) is at bin 475
      //public static int BeginFeKaFull = 475;
      //public static double[] PeakFeKaFull = new double[] {32, 44, 43, 50, 41, 53, 43, 50, 51, 67, 82, 90, 103, 138, 210, 311, 400, 523, 698, 886,
      //                                 1245, 1532, 1882, 2152, 2350, 2629, 2745, 2718, 2578, 2329, 2075, 1750, 1527, 1196, 876,
      //                                 633, 466, 286, 204, 135, 81, 38, 17, 28, 12, 10, 10, 13 };

      //public static int BeginFeKaPartial = 491;  //first count (400) is at bin 491
      //public static double[] PeakFeKaPartial = new double[] {400, 523, 698, 886,
      //                                 1245, 1532, 1882, 2152, 2350, 2629, 2745, 2718, 2578, 2329, 2075, 1750, 1527, 1196, 876,
      //                                 633, 466 };

      //public static int BeginBrKaFull = 816;
      //public static double[] PeakBrKaFull = new double[] {19, 11, 11, 19, 23, 22, 15, 25, 20, 27,
      //  28, 38, 34, 29, 44, 58, 55, 74, 104, 122, 145, 195, 227, 309, 378, 464, 476, 477, 613, 575, 616, 641, 580, 551, 522, 468, 439,
      //  359, 286, 256, 183, 129, 105, 95, 78, 46, 42, 31, 26, 17, 17, 17, 7, 20, 14, 18, 11, 18, 11, 15, 17, 14 };

      //public static int BeginBrKaPartial = 835;
      //public static double[] PeakBrKaPartial = new double[] {122, 145, 195, 227, 309, 378, 464, 476, 477, 613, 575, 616, 641, 580, 551, 522, 468, 439,
      //  359, 286, 256, 183, 129, 105};

      //public static int BeginPuLbRayPartial = 1206;
      //public static double[] PeakPuLbRayPartial = new double[] {51, 56, 80, 83, 88, 97, 118, 120, 151, 155, 196, 205, 233, 254, 303, 283, 320,
      //  318, 316, 297, 305, 268, 280, 239, 237, 184, 191, 138, 109, 76, 85, 56, 63, 40, 33, 19, 18, 20, 14, 20, 11};

      public static Sample FeKaFull = new Sample("FeKa full peak, strong",
         new double[] { 32, 44, 43, 50, 41, 53, 43, 50, 51, 67, 82, 90, 103, 138, 210, 311, 400, 523, 698, 886,
                        1245, 1532, 1882, 2152, 2350, 2629, 2745, 2718, 2578, 2329, 2075, 1750, 1527, 1196, 876,
                        633, 466, 286, 204, 135, 81, 38, 17, 28, 12, 10, 10, 13 },
         475, 501.91, 11.73);

      public static Sample FeKaPartial = new Sample("FeKa partial peak, strong",
         new double[] { 400, 523, 698, 886,
                        1245, 1532, 1882, 2152, 2350, 2629, 2745, 2718, 2578, 2329, 2075, 1750, 1527, 1196, 876,
                        633, 466 },
         491, 501.91, 11.73);

      public static Sample BrKaFull = new Sample("BrKa full peak, weak",
         new double[] { 19, 11, 11, 19, 23, 22, 15, 25, 20, 27, 28, 38, 34, 29, 44, 58, 55, 74, 104, 122, 145, 195, 227,
                        309, 378, 464, 476, 477, 613, 575, 616, 641, 580, 551, 522, 468, 439, 359, 286, 256, 183, 129,
                        105, 95, 78, 46, 42, 31, 26, 17, 17, 17, 7, 20, 14, 18, 11, 18, 11, 15, 17, 14 },
         816, 846.88, 15.17);

      public static Sample BrKaPartial = new Sample("BrKa Partial peak, weak",
         new double[] { 122, 145, 195, 227, 309, 378, 464, 476, 477, 613, 575, 616, 641, 580, 551, 522, 468, 439,
                        359, 286, 256, 183, 129, 105 },
         835, 846.88, 15.17);

      public static Sample PuLbRayPartial = new Sample("PuLbRay Partial peak, very weak",
         new double[] { 51, 56, 80, 83, 88, 97, 118, 120, 151, 155, 196, 205, 233, 254, 303, 283, 320, 318, 316, 297, 305,
                        268, 280, 239, 237, 184, 191, 138, 109, 76, 85, 56, 63, 40, 33, 19, 18, 20, 14, 20, 11 },
         1206, 1224.22, 16.32);

      //Can not do a successful Gaussian fitting on this peak
      public static Sample CrKaNoGaussianFitting = new Sample("CrKa peak, very weak, fitting return bad results",
         new double[] { 12, 8, 11, 8, 9, 18, 15, 13, 16, 21, 24, 16, 26, 32, 31, 39, 29, 52, 50, 30, 31, 37, 35, 35, 40, 39, 27, 35, 34, 42, 42, 52, 49, 67, },
         403, 0, 0);

   }
   //SpecTest(TestData.PeakFeKaFull, TestData.BeginFeKaFull, 501.91, 11.73);
   //SpecTest(TestData.PeakFeKaPartial, TestData.BeginFeKaPartial, 501.91, 11.73);
   //SpecTest(TestData.PeakBrKaFull, TestData.BeginBrKaFull, 846.88, 15.17);
   //SpecTest(TestData.PeakBrKaPartial, TestData.BeginBrKaPartial, 846.88, 15.17);
   //SpecTest(TestData.PeakPuLbRayPartial, TestData.BeginPuLbRayPartial, 1224.22, 16.32);
}
