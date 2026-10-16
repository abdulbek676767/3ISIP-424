using System;
using System.Collections.Generic;
using System.Linq;

namespace UniversitySystem
{
    class Course
    {
        private static int nextId = 1;

        public int Id { get; private set; }
        public string Title { get; private set; }
        public int Hours { get; private set; }
        public int MaxStudents { get; private set; }
        public Teacher Teacher { get; private set; }

        // студенты курса закрыты, записать можно только через Student.EnrollCourse
        private List<Student> students = new List<Student>();

        // у каждого курса есть преподаватель, поэтому без него курс не создается
        public Course(string title, int hours, int maxStudents, Teacher teacher)
        {
            Id = nextId++;
            Title = title;
            Hours = hours;
            MaxStudents = maxStudents;
            SetTeacher(teacher);
        }

        public bool IsFull()
        {
            return students.Count >= MaxStudents;
        }

        // internal: вызывается только из Student.EnrollCourse,
        // чтобы списки у студента и у курса всегда совпадали
        internal void AddStudent(Student student)
        {
            if (!students.Contains(student))
                students.Add(student);
        }

        // назначить или сменить преподавателя курса
        public void SetTeacher(Teacher teacher)
        {
            if (teacher == null || teacher == Teacher)
                return;

            if (Teacher != null)
                Teacher.RemoveCourse(this);

            Teacher = teacher;
            teacher.AddCourse(this);
        }

        public List<Student> GetStudents()
        {
            return students.OrderBy(s => s.FullName).ToList();
        }

        private string GetTeacherName()
        {
            if (Teacher == null)
                return "не назначен";
            return Teacher.FullName;
        }

        public void ShowInfo()
        {
            Console.WriteLine($"Курс №{Id}: {Title}");
            Console.WriteLine($"  Часов:               {Hours}");
            Console.WriteLine($"  Преподаватель:       {GetTeacherName()}");
            if (Teacher != null)
            {
                Console.WriteLine($"  Кафедра:             {Teacher.Department}");
                Console.WriteLine($"  Ученая степень:      {Teacher.GetDegreeText()}");
            }
            Console.WriteLine($"  Записано студентов:  {students.Count} из {MaxStudents}");
            Console.WriteLine($"  Свободных мест:      {MaxStudents - students.Count}");
        }

        public override string ToString()
        {
            return $"№{Id} {Title}, {Hours} ч., преподаватель: {GetTeacherName()}, мест занято {students.Count}/{MaxStudents}";
        }
    }
}
