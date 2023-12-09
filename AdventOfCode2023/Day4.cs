using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace AdventOfCode2023
{
   class Day4
   {
      /*
       * Locate all winning numbers for a given card
       * For each win, double the value from 1 to get total card value
       * Sum up all card winning points
       * 
       * Strategy: 
       *    1. Splice each card line into two parts - from : to |, then from | to end of line
       *    2. Split each line by spaces and parse out all the numbers into two lists, one for winning and other for given numbers
       *    3. Determine amount of given numbers are "winning" and double value (starting at 1 for 1 win) for each
       *    4. Sum the remaining value to total sum
       */
      public static int FirstPuzzle()
      {
         StreamReader reader = new StreamReader( Helper.GetInputDirectory( "day4input.txt" ) );

         int sumOfCardPoints = 0;
         string nextLine;
         while ( ( nextLine = reader.ReadLine() ) != null )
         {
            bool isFirstWin = true;
            int currentCardPointVal = 0;
            int beginning = nextLine.IndexOf( ':' ) + 2;
            int middle = nextLine.IndexOf( '|' );
            string winningNumStr = nextLine.Substring( beginning, middle - beginning );
            string givenNumStr = nextLine.Substring( middle + 2 );
            string[] winningNumList = Regex.Split( winningNumStr, @"\s{1,}" );
            string[] givenNumList = Regex.Split( givenNumStr, @"\s{1,}" );

            foreach ( string numStr in givenNumList )
            {
               int num;
               if ( int.TryParse( numStr, out num ) )
               {
                  if ( winningNumList.Contains( num.ToString() ) )
                  {
                     if ( isFirstWin )
                     {
                        currentCardPointVal = 1;
                        isFirstWin = false;
                     }
                     else
                     {
                        currentCardPointVal *= 2;
                     }
                  }
               }
            }

            sumOfCardPoints += currentCardPointVal;
         }

         return sumOfCardPoints;
      }

      /*
       * Locate all winning numbers for a given card
       * For each win, a copy of the next card is added sequentially (1 win per card)
       * Sum the total number of cards
       */
      public static int SecondPuzzle()
      {
         StreamReader reader = new StreamReader( Helper.GetInputDirectory( "day4input.txt" ) );

         int currentCardNumber = 1;
         Dictionary<int, int> copyCardList = new Dictionary<int, int>();
         string nextLine;
         while ( ( nextLine = reader.ReadLine() ) != null )
         {
            if ( !copyCardList.ContainsKey( currentCardNumber ) )
            {
               copyCardList.Add( currentCardNumber, 1 );
            }

            int beginning = nextLine.IndexOf( ':' ) + 2;
            int middle = nextLine.IndexOf( '|' );
            string winningNumStr = nextLine.Substring( beginning, middle - beginning );
            string givenNumStr = nextLine.Substring( middle + 2 );
            string[] winningNumList = Regex.Split( winningNumStr, @"\s{1,}" );
            string[] givenNumList = Regex.Split( givenNumStr, @"\s{1,}" );
            //TestPrintList( winningNumList );
            //TestPrintList( givenNumList );
            //Console.WriteLine();

            int cardNumIterator = currentCardNumber + 1;
            //Console.Write( $"{currentCardNumber}: " );
            foreach ( string numStr in givenNumList )
            {
               int num;
               if ( int.TryParse( numStr, out num ) )
               {
                  if ( winningNumList.Contains( num.ToString() ) )
                  {
                     //Console.Write( $"{num} " );
                     if ( copyCardList.ContainsKey( cardNumIterator ) )
                     {
                        copyCardList[ cardNumIterator ] += copyCardList[ currentCardNumber ];
                     }
                     else
                     {
                        copyCardList.Add( cardNumIterator, 2 );
                     }

                     cardNumIterator++;
                  }
               }
            }
            //Console.WriteLine();
            currentCardNumber++;
         }

         int totalCards = 0;
         foreach ( var cards in copyCardList )
         {
            totalCards += cards.Value;
         }

         return totalCards;
      }

      internal static void TestPrintList( string[] list )
      {
         foreach( string str in list )
         {
            Console.Write( $"{str} " );
         }
      }
   }
}
