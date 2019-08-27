/////////////////////////////////////////////////////////////////
// HomeController.cs - Controller for CourseApplication Demo   //
//                                                             //
// Jim Fawcett, CSE686 - Internet Programming, Spring 2019     //
/////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Extensions;
using CoursesApp.Models;
using CoursesApp.Data;

namespace CoursesApp.Controllers
{
  public class HomeController : Controller
  {
    private readonly ApplicationDbContext context_;
    private const string sessionId_ = "SessionId";

    public HomeController(ApplicationDbContext context)
    {
      context_ = context;
    }

    //----< show list of courses >-------------------------------

    public IActionResult Index()
    {
      return View(context_.Courses.ToList<Course>());
    }

    //----< show list of lectures, ordered by Title >------------

    public IActionResult Lectures()
    {
      // fluent API
      var lects = context_.Lectures.Include(l => l.Course);
      var orderedLects = lects.OrderBy(l => l.Title)
        .OrderBy(l => l.Course)
        .Select(l => l);
      return View(orderedLects);

      // Linq
      //var lects = context_.Lectures.Include(l => l.Course);
      //var orderedLects = from l in lects
      //                   orderby l.Title
      //                   orderby l.Course
      //                   select l;
      //return View(orderedLects);

      // doesn't return Lecture's course nor order by title
      //return View(context_.Lectures.ToList<Lecture>());
    }

    //----< displays form for creating a course >----------------

    [HttpGet]
    public IActionResult CreateCourse(int id)
    {
      var model = new Course();
      return View(model);
    }

    //----< posts back new courses details >---------------------

    [HttpPost]
    public IActionResult CreateCourse(int id, Course crs)
    {
      context_.Courses.Add(crs);
      context_.SaveChanges();
      return RedirectToAction("Index");
    }

    //----< deletes a course by id >-----------------------------
    /*
     * - note that Delete does not send back a view, but
     *   simply redirects back to the Index view.
     */
    public IActionResult DeleteCourse(int? id)
    {
      if (id == null)
      {
        return StatusCode(StatusCodes.Status400BadRequest);
      }
      try
      {
        var course = context_.Courses.Find(id);
        if (course != null)
        {
          context_.Remove(course);
          context_.SaveChanges();
        }
      }
      catch (Exception)
      {
        // nothing for now
      }
      return RedirectToAction("Index");
    }

    //----< shows details for each course >----------------------

    public ActionResult CourseDetails(int? id)
    {
      if (id == null)
      {
        return StatusCode(StatusCodes.Status400BadRequest);
      }
      Course course = context_.Courses.Find(id);
      
      if (course == null)
      {
        return StatusCode(StatusCodes.Status404NotFound);
      }
      var lects = context_.Lectures.Where(l => l.Course == course);

      course.Lectures = lects.OrderBy(l => l.Title).Select(l => l).ToList<Lecture>();
      //course.Lectures = lects.ToList<Lecture>();

      if (course.Lectures == null)
      {
        course.Lectures = new List<Lecture>();
        Lecture lct = new Lecture();
        lct.Title = "none";
        lct.Content = "none";
        course.Lectures.Add(lct);
      }
      return View(course);
    }

    //----< gets form to edit a specific course via id >---------

    [HttpGet]
    public IActionResult EditCourse(int? id)
    {
      if (id == null)
      {
        return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status400BadRequest);
      }
      Course course = context_.Courses.Find(id);
      if (course == null)
      {
        return StatusCode(StatusCodes.Status404NotFound);
      }
      return View(course);
    }

    //----< posts back edited results for specific course >------

    [HttpPost]
    public IActionResult EditCourse(int? id, Course crs)
    {
      if (id == null)
      {
        return StatusCode(StatusCodes.Status400BadRequest);
      }
      var course = context_.Courses.Find(id);
      if (course != null)
      {
        course.Identifier = crs.Identifier;
        course.Name = crs.Name;
        try
        {
          context_.SaveChanges();
        }
        catch (Exception)
        {
          // do nothing for now
        }
      }
      return RedirectToAction("Index");
    }
    //----< shows form for creating a lecture >------------------

    [HttpGet]
    public IActionResult CreateLecture(int id)
    {
      var model = new Lecture();
      return View(model);
    }

    //----< posts back new courses details >---------------------

    [HttpPost]
    public IActionResult CreateLecture(int id, Lecture lct)
    {
      context_.Lectures.Add(lct);
      context_.SaveChanges();
      return RedirectToAction("Lectures");
    }

    //----< add new lecture to course >--------------------------

    [HttpGet]
    public IActionResult AddLecture(int id)
    {
      HttpContext.Session.SetInt32(sessionId_, id);

      // this works too
      // TempData[sessionId_] = id;

      Course course = context_.Courses.Find(id);
      if (course == null)
      {
        return StatusCode(StatusCodes.Status404NotFound);
      }
      Lecture lct = new Lecture();
      return View(lct);
    }

    //----< Add new lecture to course >--------------------------

    [HttpPost]
    public IActionResult AddLecture(int? id, Lecture lct)
    {
      if (id == null)
      {
        return StatusCode(StatusCodes.Status400BadRequest);
      }
      // retreive the target course from static field

      int? courseId_ = HttpContext.Session.GetInt32(sessionId_);

      // this works too
      // int courseId_ = (int)TempData[sessionId_];

      var course = context_.Courses.Find(courseId_);

      if (course != null)
      {
        if(course.Lectures == null)  // doesn't have any lectures yet
        {
          List<Lecture> lectures = new List<Lecture>();
          course.Lectures = lectures;
        }
        course.Lectures.Add(lct);

        try
        {
          context_.SaveChanges();
        }
        catch (Exception)
        {
          // do nothing for now
        }
      }
      return RedirectToAction("Index");
    }

    //----< gets form to edit a specific lecture via id >---------

    [HttpGet]
    public IActionResult EditLecture(int? id)
    {
      if (id == null)
      {
        return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status400BadRequest);
      }
      Lecture lecture = context_.Lectures.Find(id);

      if (lecture == null)
      {
        return StatusCode(StatusCodes.Status404NotFound);
      }
      return View(lecture);
    }

    //----< posts back edited results for specific lecture >------

    [HttpPost]
    public IActionResult EditLecture(int? id, Lecture lct)
    {
      if (id == null)
      {
        return StatusCode(StatusCodes.Status400BadRequest);
      }
      var lecture = context_.Lectures.Find(id);

      if (lecture != null)
      {
        lecture.Title = lct.Title;
        lecture.Content = lct.Content;

        try
        {
          context_.SaveChanges();
        }
        catch (Exception)
        {
          // do nothing for now
        }
      }
      return RedirectToAction("Index");
    }

    //----< deletes a lecture by id >-----------------------------
    /*
     * - note that Delete does not send back a view, but
     *   simply redirects back to the Index view, which 
     *   will not show the deleted lecture.
     */
    public IActionResult DeleteLecture(int? id)
    {
      if (id == null)
      {
        return StatusCode(StatusCodes.Status400BadRequest);
      }
      try
      {
        var lecture = context_.Lectures.Find(id);
        if(lecture != null)
        {
          context_.Remove(lecture);
          context_.SaveChanges();
        }
      }
      catch (Exception)
      {
        // nothing for now
      }
      return RedirectToAction("Index");
    }

    //----< wizard generated actions >---------------------------

    public IActionResult Privacy()
    {
      return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
      return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
  }
}
