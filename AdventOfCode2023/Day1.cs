using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace AdventOfCode2023
{
   public static class Day1
   {
      /*
       * Read each line and parse first and last digit
       * If only 1 digit, repeat (ex: a2b = 22)
       * Sum up all numbers found in each line
       */
      public static int FirstPuzzle()
      {
         int calibrationVal = 0;

         StreamReader reader = new StreamReader( Helper.GetInputDirectory( "day1input.txt" ) );

         string line;
         string num = "";
         while( ( line = reader.ReadLine() ) != null )
         {
            for( int i = 0; i < line.Length; i++ )
            {
               if( char.IsDigit( line[i] ) )
               {
                  num = char.ToString( line[i] );
                  for( int j = line.Length - 1; j >= 0; j-- )
                  {
                     if( char.IsDigit( line[j] ) )
                     {
                        num += char.ToString( line[j] );
                        break;
                     }
                  }
                  break;
               }
            }

            calibrationVal += int.Parse( num );
         }

         return calibrationVal;
      }

      /*
       * Same as first but now need to treat number words as actual numbers (two = 2)
       */
      public static int SecondPuzzle()
      {
         int calibrationVal = 0;

         StreamReader reader = new StreamReader( Helper.GetInputDirectory( "day1input.txt" ) );

         string line;
         while( ( line = reader.ReadLine() ) != null )
         {
            string newLine = Transmute( line );
            string num = "" + newLine[0] + newLine[^1];
            //Console.WriteLine( num );
            calibrationVal += int.Parse( num );
         }

         return calibrationVal;
      }

      internal static string Transmute( string line )
      {
         Dictionary<string, int> numbers = new Dictionary<string, int>() {
            { "one", 1 },
            { "two", 2 },
            { "three", 3 },
            { "four", 4 },
            { "five", 5 },
            { "six", 6 },
            { "seven", 7 },
            { "eight", 8 },
            { "nine", 9 }
         };

         SortedDictionary<int, int> matches = new SortedDictionary<int, int>();

         foreach( var number in numbers )
         {
            foreach( Match match in Regex.Matches( line, @$"{number.Key}" ) )
            {
               matches.Add( match.Index, number.Value );
            }
         }

         foreach( Match match in Regex.Matches( line, "\\d" ) )
         {
            matches.Add( match.Index, int.Parse( match.Value ) );
         }

         string newLine = "";
         foreach( KeyValuePair<int, int> kvp in matches )
         {
            newLine += kvp.Value;
         }
         
         return newLine;
      }
   }
}
