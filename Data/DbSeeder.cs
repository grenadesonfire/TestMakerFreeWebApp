using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestMakerFreeWebApp.Data.Models;

namespace TestMakerFreeWebApp.Data
{
    public class DbSeeder
    {
        #region Public Methods
        public static void Seed(ApplicationDbContext dbContext)
        {
            // Create default Users (if there are none)
            if (!dbContext.Users.Any()) CreateUsers(dbContext);

            // Create default Quizzes (if there are none) together with their set of Q&A
            if (!dbContext.Quizzes.Any()) CreateQuizzes(dbContext);
        }
        #endregion

        #region Seed Methods
        private static void CreateUsers(ApplicationDbContext dbContext)
        {
            var createdDate = new DateTime(2016, 03, 01, 12, 30, 00);
            var lastModifiedDate = DateTime.Now;

            // Create the "Admin" ApplicationUser account (if it doesn't exist already)

            var user_Admin = new ApplicationUser()
            {
                Id = Guid.NewGuid().ToString(),
                UserName = "Admin",
                Email = "admin@testmakerfree.com",
                CreatedDate = createdDate,
                LastModifiedDate = lastModifiedDate,
            };

            // Insert the Admin user into the Database
            dbContext.Users.Add(user_Admin);

#if DEBUG
            // Create some sample registered user accounts (if they don't exist already)
            var user_Andrew = new ApplicationUser()
            {
                Id = Guid.NewGuid().ToString(),
                UserName = "Andrew",
                Email = "andrew@testmakerfree.com",
                CreatedDate = createdDate,
                LastModifiedDate = lastModifiedDate
            };

            var user_Beth = new ApplicationUser()
            {
                Id = Guid.NewGuid().ToString(),
                UserName = "Beth",
                Email = "Beth@testmakerfree.com",
                CreatedDate = createdDate,
                LastModifiedDate = lastModifiedDate
            };

            var user_Charley = new ApplicationUser()
            {
                Id = Guid.NewGuid().ToString(),
                UserName = "Charley",
                Email = "charley@testmakerfree.com",
                CreatedDate = createdDate,
                LastModifiedDate = lastModifiedDate
            };
#endif
            dbContext.SaveChanges();
        }

        private static void CreateQuizzes(ApplicationDbContext dbContext)
        {
            DateTime createdDate = new DateTime(2016, 03, 01, 12, 30, 00);
            DateTime lastModifiedDate = DateTime.Now;

            // retrieve the admin user, which we'll use as the default author.
            var authorId = dbContext.Users
                .Where(u => u.UserName == "Admin")
                .FirstOrDefault()
                .Id;

#if DEBUG
            var num = 47;
            for (int i = 1; i <= num; i++)
            {
                CreateSampleQuiz(
                    dbContext,
                    i,
                    authorId,
                    num - 1,
                    3,
                    3,
                    3,
                    createdDate.AddDays(-num));
            }
#endif
            EntityEntry<Quiz> e1 = dbContext.Quizzes.Add(new Quiz()
            {
                UserId = authorId,
                Title = "Are you more Light or Dark side of the Force?",
                Description = "Star Wars personality test",
                Text = @"Choose wisely you must, young padawan: " +
                        "this test will prove if your will is strong enough " +
                        "to adhere to the principles of the light side of the Force" +
                        "or if you're fated to embrace the dark side. " +
                        "No you want to become a true JEDI, you can't possibly miss this!",
                ViewCount = 2343,
                CreatedDate = createdDate,
                LastModifiedDate = lastModifiedDate,
            });

            var e2 = dbContext.Quizzes.Add(new Quiz()
            {
                UserId = authorId,
                Title = "GenX, GenY or Genz?",
                Description = "Find out what decade most represents you",
                Text = @"Do you feel comfortable in you generation? " +
                        "What year should you have been born in?" +
                        "Here's a bunch of questions that will help you to find out!",
                ViewCount = 4180,
                CreatedDate = createdDate,
                LastModifiedDate = lastModifiedDate,
            });

            var e3 = dbContext.Quizzes.Add(new Quiz()
            {
                UserId = authorId,
                Title = "WHich Shingeki No Kyojin character are you?",
                Description = "Attack on Titan personality test",
                Text = @"Do you relentlessly seek revenge like Eren? " +
                        "Are you willing to put your life on the stake to protect your friends like Mikasa? " +
                        "Would you trus your fighting skills like Levi " +
                        "or rely on your strategies and tactics like Arwin? " +
                        "Unveil your true self with this Attack On Titan personality test!",
                ViewCount = 5203,
                CreatedDate = createdDate,
                LastModifiedDate = lastModifiedDate,
            });

            dbContext.SaveChanges();
        }
        #endregion

        #region Utility Methods

        private static void CreateSampleQuiz(
            ApplicationDbContext dbContext,
            int num,
            string authorId,
            int viewCount,
            int numberOfQuestions,
            int numberOfAnswersPerQuestion,
            int numberOfResults,
            DateTime createdDate)
        {
            var quiz = new Quiz()
            {
                UserId = authorId,
                Title = $"Quiz {num} Title",
                Description = $"This is a sample description for quiz {num}.",
                Text = "This is a sample quiz created by the DbSeeder class for testing purposes. " +
                       "All the questions, answers & results are auto-generated as well.",
                ViewCount = viewCount,
                CreatedDate = createdDate,
                LastModifiedDate = createdDate,
            };
            dbContext.Quizzes.Add(quiz);
            dbContext.SaveChanges();

            for(int i=0; i<numberOfQuestions; i++)
            {
                var question = new Question() {
                    QuizId = quiz.Id,
                    Text = "This is a sample question created by the DbSeeder class for testing purposes. " +
                           "All the child answers are auto-generated as well.",
                    CreatedDate = createdDate,
                    LastModifiedDate = createdDate,
                };
                dbContext.Questions.Add(question);
                dbContext.SaveChanges();

                for(int j = 0; j < numberOfAnswersPerQuestion; j++)
                {
                    var a = dbContext.Answers.Add(new Answer()
                    {
                        QuestionId = question.Id,
                        Text = "This is a sample answer created by the DbSeeder class for testing purposes. ",
                        Value = j,
                        CreatedDate = createdDate,
                        LastModifiedDate = createdDate,
                    });
                }
            }

            for(int i = 0; i < numberOfResults; i++)
            {
                dbContext.Results.Add(new Result()
                {
                    QuizId = quiz.Id,
                    Text = "This is a sample result created by the DbSeeder class for testing purposes. ",
                    MinValue = 0,
                    MaxValue = numberOfAnswersPerQuestion * 2,
                    CreatedDate = createdDate,
                    LastModifiedDate = createdDate,
                });
            }
            dbContext.SaveChanges();
        }
        #endregion
    }
}