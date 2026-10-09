using System;
using System.Collections.Generic;

// ученая степень преподавателя
enum Degree
{
    None,       // без степени
    Candidate,  // кандидат наук
    Doctor      // доктор наук
}

class Teacher : Person
{
    public string Department { get; private set; }
    public Degree Degree { get; private set; }
    public int Experience { get; private set; }

    private List<Course> courses = new List<Course>();

    public Teacher(int id, string fullName, int age, string phone, string email, string department, Degree degree, int experience)
        : base(id, fullName, age, phone, email)
    {
        Department = department;
        Degree = degree;
        Experience = experience;
    }

    public override string GetRole()
    {
        return "Преподаватель";
    }

    // эти методы вызывает сам курс, когда ему назначают преподавателя
    public void AddCourse(Course course)
    {
        if (!courses.Contains(course))
            courses.Add(course);
    }

    public void RemoveCourse(Course course)
    {
        courses.Remove(course);
    }

    public List<Course> GetCourses()
    {
        return new List<Course>(courses);
    }

    public string GetDegreeText()
    {
        switch (Degree)
        {
            case Degree.Candidate:
                return "Кандидат наук";
            case Degree.Doctor:
                return "Доктор наук";
            default:
                return "Без степени";
        }
    }

    public override void ShowInfo()
    {
        base.ShowInfo();
        Console.WriteLine($"  Кафедра:  {Department}");
        Console.WriteLine($"  Степень:  {GetDegreeText()}");
        Console.WriteLine($"  Стаж:     {Experience} лет");
        if (courses.Count == 0)
        {
            Console.WriteLine("  Ведет курсы: нет");
        }
        else
        {
            Console.WriteLine("  Ведет курсы:");
            foreach (Course course in courses)
                Console.WriteLine($"    - {course.Title}");
        }
    }

    public override string GetShortInfo()
    {
        return base.GetShortInfo() + $", кафедра {Department}";
    }
}
