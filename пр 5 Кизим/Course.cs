using System;
using System.Collections.Generic;
using System.Linq;

class Course
{
    public int Id { get; private set; }
    public string Title { get; private set; }
    public int Hours { get; private set; }
    public int MaxStudents { get; private set; }
    public Teacher Teacher { get; private set; }

    private List<Student> students = new List<Student>();

    public Course(int id, string title, int hours, int maxStudents)
    {
        Id = id;
        Title = title;
        Hours = hours;
        MaxStudents = maxStudents;
    }

    public bool IsFull()
    {
        return students.Count >= MaxStudents;
    }

    // вызывается из Student.EnrollCourse
    public void AddStudent(Student student)
    {
        if (!students.Contains(student))
            students.Add(student);
    }

    // у курса может быть только один преподаватель
    public void SetTeacher(Teacher teacher)
    {
        if (Teacher != null)
            Teacher.RemoveCourse(this);

        Teacher = teacher;
        teacher.AddCourse(this);
    }

    public List<Student> GetStudents()
    {
        return students.OrderBy(s => s.FullName).ToList();
    }

    public void ShowInfo()
    {
        Console.WriteLine($"Курс №{Id}: {Title}");
        Console.WriteLine($"  Часов:         {Hours}");
        if (Teacher == null)
            Console.WriteLine("  Преподаватель: не назначен");
        else
            Console.WriteLine($"  Преподаватель: {Teacher.FullName}");
        Console.WriteLine($"  Студентов:     {students.Count} из {MaxStudents}");
    }

    public string GetShortInfo()
    {
        string teacherName = Teacher == null ? "не назначен" : Teacher.FullName;
        return $"№{Id} {Title} ({Hours} ч.), преподаватель: {teacherName}, мест занято {students.Count}/{MaxStudents}";
    }
}
