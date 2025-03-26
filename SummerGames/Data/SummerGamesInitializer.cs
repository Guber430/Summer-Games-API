using Microsoft.EntityFrameworkCore;
using SummerGames.Models;
using System.Diagnostics;

namespace SummerGames.Data
{
    public static class SummerGamesInitializer
    {
        /// <summary>
        /// Prepares the Database and seeds data as required
        /// </summary>
        /// <param name="serviceProvider">DI Container</param>
        /// <param name="DeleteDatabase">Delete the database and start from scratch</param>
        /// <param name="UseMigrations">Use Migrations or EnsureCreated</param>
        /// <param name="SeedSampleData">Add optional sample data</param>
        public static void Initialize(IServiceProvider serviceProvider,
            bool DeleteDatabase = false, bool UseMigrations = true, bool SeedSampleData = true)
        {
            using (var context = new SummerGamesContext(
                serviceProvider.GetRequiredService<DbContextOptions<SummerGamesContext>>()))
            {
                //Refresh the database as per the parameter options
                #region Prepare the Database
                try
                {
                    //Note: .CanConnect() will return false if the database is not there!
                    if (DeleteDatabase || !context.Database.CanConnect())
                    {
                        context.Database.EnsureDeleted(); //Delete the existing version 
                        if (UseMigrations)
                        {
                            context.Database.Migrate(); //Create the Database and apply all migrations
                        }
                        else
                        {
                            context.Database.EnsureCreated(); //Create and update the database as per the Model
                        }
                        //Now create any additional database objects such as Triggers or Views
                        //--------------------------------------------------------------------
                        //Sport Table Triggers for Concurrency
                        string sqlCmd = @"
                            CREATE TRIGGER SetSportTimestampOnUpdate
                            AFTER UPDATE ON Sports
                            BEGIN
                                UPDATE Sports
                                SET RowVersion = randomblob(8)
                                WHERE rowid = NEW.rowid;
                            END;
                        ";
                        context.Database.ExecuteSqlRaw(sqlCmd);

                        sqlCmd = @"
                            CREATE TRIGGER SetSportTimestampOnInsert
                            AFTER INSERT ON Sports
                            BEGIN
                                UPDATE Sports
                                SET RowVersion = randomblob(8)
                                WHERE rowid = NEW.rowid;
                            END
                        ";
                        context.Database.ExecuteSqlRaw(sqlCmd);

                        //Athlete Table Triggers for Concurrency
                        sqlCmd = @"
                            CREATE TRIGGER SetAthleteTimestampOnUpdate
                            AFTER UPDATE ON Athletes
                            BEGIN
                                UPDATE Athletes
                                SET RowVersion = randomblob(8)
                                WHERE rowid = NEW.rowid;
                            END;
                        ";
                        context.Database.ExecuteSqlRaw(sqlCmd);

                        sqlCmd = @"
                            CREATE TRIGGER SetAthleteTimestampOnInsert
                            AFTER INSERT ON Athletes
                            BEGIN
                                UPDATE Athletes
                                SET RowVersion = randomblob(8)
                                WHERE rowid = NEW.rowid;
                            END
                        ";
                        context.Database.ExecuteSqlRaw(sqlCmd);
                    }
                    else //The database is already created
                    {
                        if (UseMigrations)
                        {
                            //Apply all migrations
                            context.Database.Migrate();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.GetBaseException().Message);
                }
                #endregion
                //Seed data needed for production and during development
                #region Seed Required Data
                try
                {

                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.GetBaseException().Message);
                }
                #endregion

                //Seed meaningless data as sample data during development
                #region Seed Sample Data
                if (SeedSampleData)
                {
                    //To randomly generate data
                    Random random = new Random();

                    if (!context.Contingents.Any())
                    {
                        var contingents = new List<Contingent>
                    {
                        new Contingent { Code = "ON", Name = "Ontario"},
                        new Contingent { Code = "PE", Name = "Prince Edward Island"},
                        new Contingent { Code = "NB", Name = "New Brunswick"},
                        new Contingent { Code = "BC", Name = "British Columbia"},
                        new Contingent { Code = "NL", Name = "Newfoundland and Labrador"},
                        new Contingent { Code = "SK", Name = "Saskatchewan"},
                        new Contingent { Code = "NS", Name = "Nova Scotia"},
                        new Contingent { Code = "MB", Name = "Manitoba"},
                        new Contingent { Code = "QC", Name = "Quebec"},
                        new Contingent { Code = "YT", Name = "Yukon"},
                        new Contingent { Code = "NU", Name = "Nunavut"},
                        new Contingent { Code = "NT", Name = "Northwest Territories"},
                        new Contingent { Code = "AB", Name = "Alberta"}
                    };
                        context.Contingents.AddRange(contingents);
                        context.SaveChanges();
                    }

                    //Add the Sports
                    if (!context.Sports.Any())
                    {
                        string[] sports = new string[] { "Athletics", "Baseball", "Basketball", "Canoe Kayak", "Cycling - Road Cycling", "Cycling - Mountain Bike", "Diving", "Golf", "Lacrosse", "Rowing", "Rugby Sevens", "Sailing", "Soccer", "Softball", "Swimming", "Tennis", "Triathlon", "Volleyball - Beach", "Volleyball - Indoor", "Wrestling" };
                        string[] sportCodes = new string[] { "ATH", "BAB", "BKB", "CKY", "CYR", "CYM", "DIV", "GLF", "LAC", "ROW", "RGS", "SAL", "SOC", "SBA", "SWM", "TEN", "TRA", "VBB", "VBI", "WRS" };
                        int NumSports = sports.Length;

                        //Loop through sports and add them
                        for (int i = 0; i < NumSports; i++)
                        {
                            //Construct some details
                            Sport s = new Sport()
                            {
                                Code = sportCodes[i],
                                Name = sports[i]
                            };
                            context.Sports.Add(s);

                        }
                        context.SaveChanges();
                    }

                    //Create collections of the primary keys
                    //Note: for Gender we will alternate
                    int[] sportIDs = context.Sports.Select(a => a.ID).ToArray();
                    int sportIDCount = sportIDs.Length;
                    int[] contingentIDs = context.Contingents.Select(a => a.ID).ToArray();
                    int contingentIDCount = contingentIDs.Length;

                    //Affiliations
                    string[] affiliations = new string[] { "Alpine Ontario", "Archery Ontario", "Athletics Ontario", "Badminton Ontario", "Baseball Ontario", "Biathlon Ontario", "Boxing Ontario", "Canoe Kayak Ontario", "Cricket Ontario", "Dive Ontario", "Field Hockey Ontario", "Golf Ontario", "Gymnastics Ontario", "Hockey Eastern Ontario", "Hockey Northwestern Ontario", "Judo Ontario", "Karate Ontario", "Kickboxing Ontario", "Muay Thai Ontario", "Ontario Amateur Wrestling Association", "Ontario Artistic Swimming", "Ontario Ball Hockey Federation", "Ontario Basketball", "Ontario Blind Sports Association", "Ontario Bobsleigh Skeleton Association", "Ontario Cerebral Palsy Sports Association", "Ontario Council of Shooters", "Ontario Curling Council", "Ontario Cycling Association", "Ontario Equestrian", "Ontario Fencing Association", "Ontario Football Alliance", "Ontario Grappling Association", "Ontario Jiu-Jitsu Association", "Ontario Lacrosse Association", "Ontario Para Network", "Ontario Sailing", "Ontario Soccer", "Ontario Table Tennis Association", "Ontario Tennis Association", "Ontario Volleyball Association", "Ontario Water Polo", "Ontario Weightlifting Association", "Racquetball Ontario", "Ringette Ontario", "Row Ontario", "Rugby Ontario", "Softball Ontario", "Special Olympics Ontario", "Squash Ontario", "Swim Ontario", "Taekwondo Ontario", "Triathlon Ontario", "Water Ski and Wakeboard Ontario", "Wushu Ontario" };

                    //Names for Athletes 
                    string[] firstNames = new string[] { "Lyric", "Antoinette", "Kendal", "Vivian", "Ruth", "Jamison", "Emilia", "Natalee", "Yadiel", "Jakayla", "Lukas", "Moses", "Kyler", "Karla", "Chanel", "Tyler", "Camilla", "Quintin", "Braden", "Clarence" };
                    string[] lastNames = new string[] { "Watts", "Randall", "Arias", "Weber", "Stone" };//, "Carlson", "Robles", "Frederick", "Parker", "Morris", "Soto", "Bruce", "Orozco", "Boyer", "Burns", "Cobb", "Blankenship", "Houston", "Estes", "Atkins", "Miranda", "Zuniga", "Ward", "Mayo", "Costa", "Reeves", "Anthony", "Cook", "Krueger", "Crane", "Watts", "Little", "Henderson", "Bishop" };

                    int firstNameCount = firstNames.Length;
                    int lastNameCount = lastNames.Length;
                    int affiliationCount = affiliations.Length;

                    //Add the Athletes
                    if (!context.Athletes.Any())
                    {
                        //Birthdate for randomly produced Athletes 
                        //We will add a random number of days to the minimum date
                        DateTime startDOB = Convert.ToDateTime("1995-08-22");
                        int counter = 1; //Used to help set genders

                        foreach (string lastName in lastNames)
                        {
                            //Choose a random HashSet of 2 (Unique) first names
                            HashSet<string> selectedFirstNames = new HashSet<string>();
                            while (selectedFirstNames.Count() < 2)
                            {
                                selectedFirstNames.Add(firstNames[random.Next(firstNameCount)]);
                            }

                            foreach (string firstName in selectedFirstNames)
                            {
                                //Construct some Athlete details
                                Athlete athlete = new Athlete()
                                {
                                    FirstName = firstName,
                                    LastName = lastName,
                                    MiddleName = lastName[1].ToString().ToUpper(),
                                    AthleteCode = random.Next(1111111, 9999999).ToString(),
                                    DOB = startDOB.AddDays(random.Next(60, 6500)),
                                    Height = random.Next(170, 200),
                                    Weight = random.Next(80, 100),
                                    Gender = (counter % 2 == 0) ? "W" : "M",
                                    Affiliation = affiliations[random.Next(affiliationCount)],
                                    ContingentID = contingentIDs[random.Next(contingentIDCount)],
                                    SportID = sportIDs[random.Next(sportIDCount)]
                                };
                                counter++;

                                try
                                {
                                    //Could be duplicate AthleteCode
                                    context.Athletes.Add(athlete);
                                    context.SaveChanges();
                                }
                                catch (DbUpdateException)
                                {
                                    //Could not insert this one so just skip it.
                                    //However, we must remove it from the "cue" in
                                    //the context or it will keep trying to save it
                                    //and fail over and over again, preventing
                                    //any more from being inserted. 
                                    context.Remove(athlete);
                                }
                            }
                        }
                    }
                }
                #endregion
            }
        }
    }
}
