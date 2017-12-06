using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pirates;

namespace MyBot
{
    /// <summary>
    /// This is an example for a bot.
    /// </summary>
    public class TutorialBot : IPirateBot
    {
        
        //comes on work with me
        /// <summary>
        /// Makes the bot run a single turn.
        /// </summary>
        /// <param name="game">The current game state</param>
        
        Pirate collector;
        Pirate tailGuard;
        List<Pirate> bodyGuards;

        int offsetX;
        int offsetY;

        public void DoTurn(PirateGame game)
        {

            collector = game.GetAllMyPirates().ToList()[6];

            tailGuard = game.GetAllMyPirates().ToList()[7];

            bodyGuards = new List<Pirate>();
            bodyGuards.Add(game.GetAllMyPirates().ToList()[5]);
            bodyGuards.Add(game.GetAllMyPirates().ToList()[4]);

            offsetX = 300;
            offsetY = 300;

            // Get one of my pirates.
            //Pirate pirate = game.GetMyLivingPirates()[0];
            List<Pirate> defenders = game.GetAllMyPirates().ToList();
            defenders.OrderBy(Pirate => Pirate.Location.Distance(game.GetEnemyCapsule().Location));
            defenders.RemoveRange(4, 4);

            foreach (Pirate pirate in defenders)
            {
                if (pirate.IsAlive())
                {

                    if (!PushCarrier(pirate, game))
                    {
                        // Enemy capsule defenders work

                        Location start;
                        if ((defenders[0].Equals(pirate) || defenders[1].Equals(pirate)))
                        {
                            if (defendFrom(game) == null)
                            {
                                start = generateStart(game, 0);
                                pirate.Sail(start);
                            }

                            else pirate.Sail(defendFrom(game));
                        }
                        else if (defenders[2].Equals(pirate) || defenders[3].Equals(pirate))
                        {
                            if (defendFrom(game) == null)
                            {
                                start = generateStart(game, 400);
                                pirate.Sail(start);
                            }

                            else pirate.Sail(defendFrom(game));

                        }

                        /*else
                        {
                            if (pirate.Capsule == null)
                            {
                                Capsule capsule = game.GetMyCapsule();
                                pirate.Sail(capsule);
                            }
                            else
                            {
                                Mothership mothership = game.GetMyMothership();
                                // Go towards the mothership.
                                pirate.Sail(mothership);
                            }
                            
                        }*/
                    }
                }
            }

            Formation(game);


        }

        private Location generateStart(PirateGame game, int range)
        {
            int row = 0;
            int col = 0;
            if (game.GetEnemyMothership().Location.Col > game.GetMyMothership().Location.Col)
                col = game.GetEnemyMothership().Location.Col - 1001 + range;
            else col = game.GetEnemyMothership().Location.Col + 1001 - range;

            if (game.GetEnemyCapsule().InitialLocation.Row > game.GetMyCapsule().InitialLocation.Row)
                row = game.GetEnemyMothership().Location.Row + 1001 - range;
            else
                row = game.GetEnemyMothership().Location.Row - 1001 + range;
            return new Location(row, col);
        }

        private Pirate defendFrom(PirateGame game)
        {
            List<Pirate> enemiesByDistanceFromEnemyBase = game.GetEnemyLivingPirates().ToList();
            enemiesByDistanceFromEnemyBase.OrderBy(Pirate => Pirate.Location.Distance(game.GetEnemyMothership().Location));
            List<Pirate> potentialThreat = new List<Pirate>();
            potentialThreat.Add(null);
            foreach (Pirate pirate in enemiesByDistanceFromEnemyBase)
            {
                if (pirate.Distance(game.GetEnemyMothership()) < 2000)
                {
                    if (potentialThreat[0] == null) ;
                    potentialThreat[0] = pirate;
                }
            }
            if (potentialThreat != null)
                return potentialThreat[0];
            else return null;

        }

        /// <summary>
        /// Makes the pirate try to push an enemy pirate. Returns true if it did.
        /// </summary>
        /// <param name="pirate">The pushing pirate.</param>
        /// <param name="game">The current game state.</param>
        /// <returns>true if the pirate pushed.</returns>
        private bool TryPush(Pirate pirate, PirateGame game)
        {
            // Go over all enemies.
            foreach (Pirate enemy in game.GetEnemyLivingPirates())
            {
                // Check if the pirate can push the enemy.
                if (pirate.CanPush(enemy))
                {
                    // Push enemy!
                    pirate.Push(enemy, enemy.InitialLocation);

                    // Print a message.
                    System.Console.WriteLine("pirate " + pirate + " pushes " + enemy + " towards " + enemy.InitialLocation);

                    // Did push.
                    return true;
                }
            }

            // Didn't push.
            return false;
        }

        private bool PushCarrier(Pirate pirate, PirateGame game)
        {
            // Go over all enemies.
            foreach (Pirate enemy in game.GetEnemyLivingPirates())
            {
                // Check if the pirate can push the enemy.
                if (pirate.CanPush(enemy) && enemy.HasCapsule())
                {
                    // Push enemy!
                    pirate.Push(enemy, enemy.InitialLocation);

                    // Print a message.
                    System.Console.WriteLine("pirate " + pirate + " pushes " + enemy + " towards " + enemy.InitialLocation);

                    // Did push.
                    return true;
                }
            }

            // Didn't push.
            return false;
        }


        void Formation(PirateGame game)
        {

            if (collector.IsAlive())
            {
                if (collector.Capsule == null)
                {
                    if (!TryPush(collector, game))
                    {
                        collector.Sail(game.GetMyCapsule());
                    }

                }
                else
                {
                    if (!TryPush(collector, game))
                    {
                        collector.Sail(game.GetMyMothership().Location);
                    }

                }
            }
            else
            {
                Pirate temp = collector;
                for (int i = 0; i < 2; i++)
                {

                    if (bodyGuards[i].IsAlive() && !collector.IsAlive())
                    {
                        collector = bodyGuards[i];
                        bodyGuards[i] = temp;
                        break;
                    }
                }

            }
            if (bodyGuards[0].IsAlive())
            {
                if (!TryPush(bodyGuards[0], game))

                    bodyGuards[0].Sail(new Location(collector.Location.Row + offsetY, collector.Location.Col - offsetX));

            }
            if (bodyGuards[1].IsAlive())
            {
                if (!TryPush(bodyGuards[1], game))

                    bodyGuards[1].Sail(new Location(collector.Location.Row + offsetY, collector.Location.Col + offsetX));
            }
            if (tailGuard.IsAlive())
            {
                if (!TryPush(tailGuard, game))

                    tailGuard.Sail(new Location(collector.Location.Row + offsetY, collector.Location.Col));
            }

        }
    }
}