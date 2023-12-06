using System;
using System.IO;

namespace AdventOfCode2023
{
   public static class Helper
   {
      public static string GetInputDirectory( string fileName )
      {
         return Path.Combine( AppDomain.CurrentDomain.BaseDirectory, "Inputs", fileName );
      }
   }
}
