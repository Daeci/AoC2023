using System.Collections.Generic;
using System.IO;

namespace AdventOfCode2023
{
   public class Day2
   {
      /*
       * Read in each group of results in a game line separated by ';'
       * Determine which game lines are possible if the game bag contained 12 red cubes, 13 green cubes, 14 blue cubes
       * Sum up all the possible game line ids (example: game 1 - yes, game 2 - no, game 3 - yes = 4)
       */
      public static int FirstPuzzle()
      {
         int gameIdSum = 0;
         StreamReader reader = new StreamReader( Helper.GetInputDirectory( "day2input.txt" ) );

         string line;
         int gameNum = 1;
         while( ( line = reader.ReadLine() ) != null )
         {
            List<string> gameResults = new List<string>();
            List<int> indexes = new List<int>();
            int tempIndex = line.IndexOf( ':' );
            for( int i = line.IndexOf( ';' ); i > -1; i = line.IndexOf( ';', i + 1 ) )
            {
               gameResults.Add( line.Substring( tempIndex + 1, i - tempIndex + 1 ) );
               tempIndex = i;
            }
            gameResults.Add( line.Substring( tempIndex + 1 ) );

            if( IsValidGameResult( gameResults ) )
            {
               gameIdSum += gameNum;
            }

            gameNum ++;
         }

         return gameIdSum;
      }

      /*
       * Determine the fewest number of red, green, and blue balls possible for a game
       * Includes all game results from line of input
       * Then take the power of the set of cubes (multiple all three values together)
       * Sum it all up
       */
      public static int SecondPuzzle()
      {
         int gamePowerSum = 0;
         StreamReader reader = new StreamReader( Helper.GetInputDirectory( "day2input.txt" ) );

         string line;
         while( ( line = reader.ReadLine() ) != null )
         {
            List<string> gameResults = new List<string>();
            List<int> indexes = new List<int>();
            int tempIndex = line.IndexOf( ':' );
            for( int i = line.IndexOf( ';' ); i > -1; i = line.IndexOf( ';', i + 1 ) )
            {
               gameResults.Add( line.Substring( tempIndex + 1, i - tempIndex + 1 ) );
               tempIndex = i;
            }
            gameResults.Add( line.Substring( tempIndex + 1 ) );

            int maxRed = 0;
            int maxBlue = 0;
            int maxGreen = 0;
            foreach( string gameResult in gameResults )
            {
               GameResult game = GetGameResult( gameResult );
               if( game.RedBall > maxRed )
               {
                  maxRed = game.RedBall;
               }

               if( game.BlueBall > maxBlue )
               {
                  maxBlue = game.BlueBall;
               }

               if( game.GreenBall > maxGreen )
               {
                  maxGreen = game.GreenBall;
               }
            }

            gamePowerSum += maxRed * maxBlue * maxGreen;
         }

         return gamePowerSum;
      }

      internal static bool IsValidGameResult( List<string> gameResults )
      {
         foreach( string gameResult in gameResults )
         {
            GameResult game = GetGameResult( gameResult );
            //Console.WriteLine( $"Red: {game.RedBall}, Blue: {game.BlueBall}, Green: {game.GreenBall}" );
            if( !game.IsValidGame() )
            {
               return false;
            }
         }

         return true;
      }

      internal static GameResult GetGameResult( string gameResult )
      {
         List<string> ballResults = new List<string>();
         int tempIndex = 0;
         for( int i = gameResult.IndexOf( ',' ); i > -1; i = gameResult.IndexOf( ',', i + 1 ) )
         {
            ballResults.Add( gameResult.Substring( tempIndex + 1, i - tempIndex + 1 ) );
            tempIndex = i;
         }
         ballResults.Add( gameResult.Substring( tempIndex + 1 ) );

         int redBallAmount = 0;
         int blueBallAmount = 0;
         int greenBallAmount = 0;
         foreach( string ballResult in ballResults )
         {
            string fixedBallResult = ballResult.Replace( " ", "" );
            string numStr = "";
            for( int i = 0; char.IsDigit( fixedBallResult[ i ] ); i++ )
            {
               numStr += fixedBallResult[i];
            }

            if( fixedBallResult.Contains( "red" ) )
            {
               redBallAmount = int.Parse( numStr );
            }
            else if( fixedBallResult.Contains( "blue" ) )
            {
               blueBallAmount = int.Parse( numStr );
            }
            else if( fixedBallResult.Contains( "green" ) )
            {
               greenBallAmount = int.Parse( numStr );
            }
         }

         return new GameResult( redBallAmount, blueBallAmount, greenBallAmount );
      }
   }

   internal class GameResult
   {
      public int RedBall { get; init; }
      public int BlueBall { get; init; }
      public int GreenBall { get; init; }

      public GameResult( int redBallNum, int blueBallNum, int greenBallNum )
      {
         RedBall = redBallNum;
         BlueBall = blueBallNum;
         GreenBall = greenBallNum;
      }

      public bool IsValidGame()
      {
         return RedBall <= 12 && GreenBall <= 13 && BlueBall <= 14;
      }
   }
}
