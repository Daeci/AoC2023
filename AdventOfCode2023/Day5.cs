using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2023
{
   class Day5
   {
      /*
       * Destination, Source, Range
       */
      public static long FirstPuzzle()
      {
         StreamReader reader = new StreamReader( Helper.GetInputDirectory( "day5input.txt" ) );

         string seeds = reader.ReadLine();
         List<long> seedsList = seeds.Substring( seeds.IndexOf( ':' ) + 2 ).Split( ' ' ).Select( long.Parse ).ToList();
         List<long> processedVals = new List<long>();
         string line;
         bool startCapturingData = false;
         while( ( line = reader.ReadLine() ) != null )
         {
            if( startCapturingData )
            {
               List<Converter> convertingList = new List<Converter>();
               // capture all the data
               do
               {
                  long[] lineParse = line.Split( ' ' ).Select( long.Parse ).ToArray();
                  convertingList.Add( new Converter( lineParse[ 0 ], lineParse[ 1 ], lineParse[ 2 ] ) );
                  line = reader.ReadLine();
               } while( line != null && line.Any( char.IsDigit ) );

               // do the data process for this set of conversions
               if( processedVals.Count == 0 ) { processedVals = ProcessConversions( seedsList, convertingList ); }
               else { processedVals = ProcessConversions( processedVals, convertingList ); }

               startCapturingData = false;
            }
            else
            {
               if( line.Contains( ':' ) )
               {
                  startCapturingData = true;
               }
            }
         }

         long minLocation = processedVals[ 0 ];
         foreach( long processedVal in processedVals )
         {
            if( processedVal < minLocation )
            {
               minLocation = processedVal;
            }
         }

         return minLocation;
      }

      public static long SecondPuzzle()
      {
         StreamReader reader = new StreamReader( Helper.GetInputDirectory( "day5input.txt" ) );

         string seeds = reader.ReadLine();
         long[] seedsInitialList = seeds.Substring( seeds.IndexOf( ':' ) + 2 ).Split( ' ' ).Select( long.Parse ).ToArray();
         List<long> seedsList = new List<long>();
         for( int i = 0; i < seedsInitialList.Length; i+=2 )
         {
            for( long j = seedsInitialList[ i ]; j < seedsInitialList[ i ] + seedsInitialList[ i + 1 ] - 1; j++ )
            {
               seedsList.Add( j );
            }
         }
         List<long> processedVals = new List<long>();
         string line;
         bool startCapturingData = false;
         while( ( line = reader.ReadLine() ) != null )
         {
            if( startCapturingData )
            {
               List<Converter> convertingList = new List<Converter>();
               // capture all the data
               do
               {
                  long[] lineParse = line.Split( ' ' ).Select( long.Parse ).ToArray();
                  convertingList.Add( new Converter( lineParse[ 0 ], lineParse[ 1 ], lineParse[ 2 ] ) );
                  line = reader.ReadLine();
               } while( line != null && line.Any( char.IsDigit ) );

               // do the data process for this set of conversions
               if( processedVals.Count == 0 ) { processedVals = ProcessConversions( seedsList, convertingList ); }
               else { processedVals = ProcessConversions( processedVals, convertingList ); }

               startCapturingData = false;
            }
            else
            {
               if( line.Contains( ':' ) )
               {
                  startCapturingData = true;
               }
            }
         }

         long minLocation = processedVals[ 0 ];
         foreach( long processedVal in processedVals )
         {
            if( processedVal < minLocation )
            {
               minLocation = processedVal;
            }
         }

         return minLocation;
      }

      internal static List<long> ProcessConversions( List<long> inputVals, List<Converter> conversionVals )
      {
         List<long> outputVals = new List<long>();
         foreach( long inputVal in inputVals )
         {
            bool isConverted = false;
            foreach( Converter conv in conversionVals )
            {
               long maxVal = conv.Source + conv.Range - 1;
               if( inputVal >= conv.Source && inputVal <= maxVal )
               {
                  outputVals.Add( inputVal + ( conv.Destination - conv.Source ) );
                  isConverted = true;
                  break;
               }
            }

            if( !isConverted )
            {
               outputVals.Add( inputVal );
            }
         }

         return outputVals;
      }
   }

   internal class Converter
   {
      public long Destination { get; init; }
      public long Source { get; init; }
      public long Range { get; init; }

      public Converter( long destination, long source, long range )
      {
         Destination = destination;
         Source = source;
         Range = range;
      }
   }
}