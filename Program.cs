/////////////////////////////////////////////////////////////////
// Program.cs - Entry point for CourseApplication Demo         //
//                                                             //
// Jim Fawcett, CSE686 - Internet Programming, Spring 2019     //
/////////////////////////////////////////////////////////////////
/*
 * This project builds an Asp.Net Core Mvc application that:
 * - Defines a set of courses, and a set of lectures for each course.
 *   That generates a one-to-many relationship in the Database.
 * - Uses Entity Framework to manage all data access.
 * - Uses Authentication (but does not have any roles yet).
 * - Uses Session to save a table key across two AddLecture Views.
 * - Shows how to extract a record from a Course table, along with
 *   data it refers to in another Lecture table.
 * - Shows how View structure is determined by the data structure
 *   as well as the application.
 *   
 * This is a demo for class, so the error handling is fairly sparse.
 * 
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CoursesApp
{
  public class Program
  {
    public static void Main(string[] args)
    {
      CreateWebHostBuilder(args).Build().Run();
    }

    public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
        WebHost.CreateDefaultBuilder(args)
            .UseStartup<Startup>();
  }
}
