/////////////////////////////////////////////////////////////////
// Course.cs - Models for CourseApplication Demo               //
//                                                             //
// Jim Fawcett, CSE686 - Internet Programming, Spring 2019     //
/////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoursesApp.Models
{
  public class Course
  {
    public int CourseId { get; set; }
    public string Identifier { get; set; }
    public string Name { get; set; }

    public ICollection<Lecture> Lectures { get; set; }
  }

  public class Lecture
  {
    public int LectureId { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }

    public int? CourseId { get; set; }
    public Course Course { get; set; }
  }
}
