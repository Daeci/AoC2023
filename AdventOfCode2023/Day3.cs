using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace AdventOfCode2023
{
   public class Day3
   {
      /*
       * Locate all numbers that have at least one digit adjacent (horizontally, vertically, and diagonally) to a symbol (periods don't count)
       * Capture the entire number with the digit(s)
       * Calculate the sum
       */
      public static int FirstPuzzle()
      {
         StreamReader reader = new StreamReader( Helper.GetInputDirectory( "day3input.txt" ) );

         int numberSum = 0;
         List<string> grid = new List<string>();
         List<KeyValuePair<int, int>> symbolsList;
         grid.Add( reader.ReadLine() );
         grid.Add( reader.ReadLine() );
         do
         {
            /*
             * Map out the coordinates of each non-period symbol
             * Slice out every full number
             * Check each number if any digit is adjacent to any of the symbols mapped out already
             * Add appropriate number to sum
             */
            symbolsList = new List<KeyValuePair<int, int>>();
            Dictionary<KeyValuePair<int, int>, int> numbersWithIndexList = new Dictionary<KeyValuePair<int, int>, int>(); // index, number
            for( int i = 0; i < grid.Count; i++ )
            {
               foreach( Match match in Regex.Matches( grid[ i ], @"[^.\d\s]" ) ) // match every non-period, non-digit, non-whitespace
               {
                  symbolsList.Add( new KeyValuePair<int, int>( i, match.Index ) ); // coords of all non-period symbols in the two lines
               }

               foreach( Match match in Regex.Matches( grid[ i ], @"(?:\D|^|)(\d+)(?:\D|$)" ) ) // match every number that either has a symbol in front or behind it,
                                                                                               // or is at the start or end of a line,
                                                                                               // and last resort find zero-length match (ex: .664.598..) => [.664.][598.]
               {
                  numbersWithIndexList.Add( new KeyValuePair<int, int>( match.Groups[ 1 ].Index, int.Parse( match.Groups[ 1 ].Value ) ), i );
               }
            }

            foreach( var number in numbersWithIndexList )
            {
               if( IsAdjacent( number, grid, symbolsList ) )
               {
                  //Console.WriteLine( $"Adjacent number: {number.Key.Value}" );
                  numberSum += number.Key.Value;
               }
            }
            
            grid[ 0 ] = grid[ 1 ];
         } while( ( grid[ 1 ] = reader.ReadLine() ) != null );

         // perform last cycle with last row only to look for horizontal adjacencies
         symbolsList = new List<KeyValuePair<int, int>>();
         foreach( Match match in Regex.Matches( grid[ 0 ], @"[^.\d\s]" ) ) // match every non-period, non-digit, non-whitespace
         {
            symbolsList.Add( new KeyValuePair<int, int>( 0, match.Index ) ); // coords of all non-period symbols in the two lines
         }

         Dictionary<int, int> firstRowNumberAndIndex = new Dictionary<int, int>();
         foreach( Match match in Regex.Matches( grid[ 0 ], @"(?:\D|^)(\d+)(?:\D|$)" ) )
         {
            firstRowNumberAndIndex.Add( match.Groups[ 1 ].Index, int.Parse( match.Groups[ 1 ].Value ) );
         }

         foreach( var number in firstRowNumberAndIndex )
         {
            if( IsAdjacent( new KeyValuePair<KeyValuePair<int, int>, int>( number, 0 ), grid, symbolsList ) )
            {
               //Console.WriteLine( $"Adjacent number: {number.Value}" );
               numberSum += number.Value;
            }
         }

         return numberSum;
      }

      /*
       * Locate all '*' symbols with only two adjacent numbers
       * Multiply the two values together and sum all the results
       * 
       * Strategy:
       *    1. Locate all '*' symbols and record kvp(index, line)
       *    2. Start with 3 lines and keep track of all numbers adjacent to a discovered '*' symbol.
       *    3. Iterate through list of discovered star symbols and pick out the ones with only 2 numbers adjacent and add to the total gearSum their multiplied values
       *    4. Repeat process checking only line 2 until EOF
       */
      public static int SecondPuzzle()
      {
         StreamReader reader = new StreamReader( Helper.GetInputDirectory( "day3input.txt" ) );

         int gearSum = 0;
         bool isFirstRun = true;
         List<string> grid = new List<string>();
         grid.Add( reader.ReadLine() );
         grid.Add( reader.ReadLine() );
         grid.Add( reader.ReadLine() );
         do
         {
            List<StarLocation> starSymbolLocations = new List<StarLocation>();
            List<NumberLocation> numberLocations = new List<NumberLocation>();
            if( isFirstRun )
            {
               for( int j = grid[ 0 ].IndexOf( '*' ); j > -1; j = grid[ 0 ].IndexOf( '*', j + 1 ) )
               {
                  starSymbolLocations.Add( new StarLocation( 0, j ) );
               }

               for( int j = grid[ 1 ].IndexOf( '*' ); j > -1; j = grid[ 1 ].IndexOf( '*', j + 1 ) )
               {
                  starSymbolLocations.Add( new StarLocation( 1, j ) );
               }
               isFirstRun = false;
            }
            else
            {
               for( int j = grid[ 1 ].IndexOf( '*' ); j > -1; j = grid[ 1 ].IndexOf( '*', j + 1 ) )
               {
                  starSymbolLocations.Add( new StarLocation( 1, j ) );
               }
            }
            for( int i = 0; i < grid.Count; i++ )
            {
               foreach( Match match in Regex.Matches( grid[ i ], @"(?:\D|^|)(\d+)(?:\D|$)" ) )
               {
                  numberLocations.Add( new NumberLocation( i, match.Groups[ 1 ].Index, int.Parse( match.Groups[ 1 ].Value ) ) );
               }
            }

            foreach( StarLocation star in starSymbolLocations )
            {
               foreach( NumberLocation number in numberLocations )
               {
                  if( IsAdjacentToStar( number, star ) )
                  {
                     star.AdjacentNums.Add( number.Number );
                  }
               }
            }

            foreach( StarLocation star in starSymbolLocations )
            {
               if( star.AdjacentNums.Count == 2 )
               {
                  gearSum += star.AdjacentNums[ 0 ] * star.AdjacentNums[ 1 ];
               }
            }
            grid[ 0 ] = grid[ 1 ];
            grid[ 1 ] = grid[ 2 ];
         } while( ( grid[ 2 ] = reader.ReadLine() ) != null );

         return gearSum;
      }

      internal static bool IsAdjacent( KeyValuePair<KeyValuePair<int, int>, int> number, List<string> grid, List<KeyValuePair<int, int>> symbolsList )
      {
         /*
          * Look for first line adjacent using first and second line symbols
          * Look for second line adjacent using only first line symbols because we're throwing away the first line after and don't want duplicate results
          * 
          * number => number.Key.Key = index | number.Key.Value = number | number.Value = grid line 0 or 1
          */
         //Console.WriteLine( $"Checking number {number.Key.Value}" );
         if( grid.Count > 1 ) // really only gonna be 1 or 2
         {
            List<int> firstRowSymbols = ( from kvp in symbolsList where kvp.Key == 0 select kvp.Value ).ToList() ?? new List<int>();
            List<int> secondRowSymbols = ( from kvp in symbolsList where kvp.Key == 1 select kvp.Value ).ToList() ?? new List<int>();
            if( number.Value == 0 )
            {
               int numEndIndex = number.Key.Key + number.Key.Value.ToString().Length - 1;
               foreach( int firstRowSymbolIndex in firstRowSymbols )
               {
                  if( number.Key.Key == 0 )
                  {
                     if( firstRowSymbolIndex == numEndIndex + 1 ) { return true; }
                  }
                  else if( numEndIndex + 1 == grid[ 0 ].Length )
                  {
                     if( firstRowSymbolIndex == number.Key.Key - 1 ) { return true; }
                  }
                  else
                  {
                     if( ( firstRowSymbolIndex == numEndIndex + 1 ) || (firstRowSymbolIndex == number.Key.Key - 1 ) ) { return true; }
                  }
               }

               foreach( int secondRowSymbolIndex in secondRowSymbols )
               {
                  if( number.Key.Key == 0 )
                  {
                     if( secondRowSymbolIndex >= number.Key.Key && secondRowSymbolIndex <= numEndIndex + 1 ) { return true; }
                  }
                  else if(numEndIndex + 1 == grid[ 0 ].Length )
                  {
                     if( secondRowSymbolIndex >= number.Key.Key - 1 && secondRowSymbolIndex <= numEndIndex ) { return true; }
                  }
                  else
                  {
                     if( secondRowSymbolIndex >= number.Key.Key - 1 && secondRowSymbolIndex <= numEndIndex + 1 ) { return true; }
                  }
               }

               return false;
            }
            else
            {
               int numEndIndex = number.Key.Key + number.Key.Value.ToString().Length - 1;
               foreach( int firstRowSymbolIndex in firstRowSymbols )
               {
                  if( number.Key.Key == 0 )
                  {
                     if( firstRowSymbolIndex >= number.Key.Key && firstRowSymbolIndex <= numEndIndex + 1 ) { return true; }
                  }
                  else if( numEndIndex + 1 == grid[ 0 ].Length )
                  {
                     if( firstRowSymbolIndex >= number.Key.Key - 1 && firstRowSymbolIndex <= numEndIndex ) { return true; }
                  }
                  else
                  {
                     if( firstRowSymbolIndex >= number.Key.Key - 1 && firstRowSymbolIndex <= numEndIndex + 1 ) { return true; }
                  }
               }

               return false;
            }
         }
         else
         {
            int numEndIndex = number.Key.Key + number.Key.Value.ToString().Length - 1;
            List<int> firstRowSymbols = ( from kvp in symbolsList where kvp.Key == 0 select kvp.Value ).ToList() ?? new List<int>();
            foreach( int firstRowSymbolIndex in firstRowSymbols )
            {
               if( number.Key.Key == 0 )
               {
                  if( firstRowSymbolIndex == numEndIndex + 1 ) { return true; }
               }
               else if( numEndIndex + 1 == grid[ 0 ].Length )
               {
                  if( firstRowSymbolIndex == number.Key.Key - 1 ) { return true; }
               }
               else
               {
                  if( ( firstRowSymbolIndex == numEndIndex + 1 ) || (firstRowSymbolIndex == number.Key.Key - 1 ) ) { return true; }
               }
            }

            return false;
         }
      }

      internal static bool IsAdjacentToStar( NumberLocation number, StarLocation star )
      {
         int numMaxIndex = number.Index + number.Number.ToString().Length - 1;
         if( star.Line == 1 ) 
         {
            if( number.Line != 1 ) // top or bottom lines (0 & 2)
            {
               if( star.Index >= number.Index - 1 && star.Index <= numMaxIndex + 1 ) { return true; }
            }
            else
            {
               if( star.Index == number.Index - 1 || star.Index == numMaxIndex + 1 ) { return true; }
            }

            return false;
         }

         if( star.Line == 0 ) // only in first run through (in hindsight completely unnecessary as input file doesn't include a * in first row, but anyways)
         {
            if( Math.Abs( number.Line - star.Line ) == 2 ) // case of first runthrough where checking number in 3rd row vs star in 1st
            {
               return false;
            }

            if( number.Line == 0 )
            {
               if( star.Index == number.Index - 1 || star.Index == numMaxIndex + 1 ) { return true; }
            }
            else // 1
            {
               if( star.Index >= number.Index - 1 && star.Index <= numMaxIndex + 1 ) { return true; }
            }

            return false;
         }

         return false;
      }
   }

   internal class StarLocation
   {
      public int Line { get; init; }
      public int Index { get; init; }
      public List<int> AdjacentNums { get; set; } = new List<int>();

      public StarLocation( int line, int index )
      {
         Line = line;
         Index = index;
      }
   }

   internal class NumberLocation
   {
      public int Line { get; init; }
      public int Index { get; init; }
      public int Number { get; init; }

      public NumberLocation( int line, int index, int number )
      {
         Line = line;
         Index = index;
         Number = number;
      }
   }
}