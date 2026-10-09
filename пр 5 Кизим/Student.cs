using System;
using System.Collections.Generic;

class Student : Person
{
    public string Group { get; private set; }
    public int Year { get; private set; }

    // список курсов закрыт, менять его можно только через методы
    private List<Course> courses = new List<Course>();

    public Student(int id, string fullName, int age, string phone, string email, string group, int year)
        : base(id, fullName, age, phone, email)
    {
        Group = group;
        Year = year;
    }

    public override string GetRole()
    {
        return "Студент";
    }

    // записываем студента на курс, повторно записаться нельзя
    public bool EnrollCourse(Course course)
    {
        if (courses.Contains(course))
            return false;

        courses.Add(course);
        course.AddStudent(this);
        return true;
    }

    public List<Course> GetCourses()
    {
        return new List<Course>(courses);
    }

    public override void ShowInfo()
    {
        base.ShowInfo();
        Console.WriteLine($"  Группа:   {Group}");
        Console.WriteLine($"  Курс:     {Year}");
        Console.WriteLine($"  Записан на курсов: {courses.Count}");
    }

    public override string GetShortInfo()
    {
        return base.GetShortInfo() + $", группа {Group}";
    }
}
