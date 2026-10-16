using System;
using System.Collections.Generic;
using System.Linq;

namespace UniversitySystem
{
    // ученая степень преподавателя
    enum AcademicDegree
    {
        None,       // без степени
        Candidate,  // кандидат наук
        Doctor      // доктор наук
    }

    // Наследование: преподаватель тоже Person, но со своими данными
    class Teacher : Person
    {
        private static int nextId = 1;

        public string Department { get; private set; }
        public AcademicDegree Degree { get; private set; }
        public int Experience { get; private set; }

        // курсы, которые ведет преподаватель
        private List<Course> courses = new List<Course>();

        public Teacher(string fullName, int age, string phone, string email, string department, AcademicDegree degree, int experience)
            : base(nextId++, fullName, age, phone, email)
        {
            Department = department;
            Degree = degree;
            Experience = experience;
        }

        public override string GetRole()
        {
            return "Преподаватель";
        }

        // internal: эти методы вызывает только Course, когда назначает преподавателя
        internal void AddCourse(Course course)
        {
            if (!courses.Contains(course))
                courses.Add(course);
        }

        internal void RemoveCourse(Course course)
        {
            courses.Remove(course);
        }

        public List<Course> GetCourses()
        {
            return courses.OrderBy(c => c.Title).ToList();
        }

        public string GetDegreeText()
        {
            switch (Degree)
            {
                case AcademicDegree.Candidate:
                    return "кандидат наук";
                case AcademicDegree.Doctor:
                    return "доктор наук";
                default:
                    return "без степени";
            }
        }

        public override void ShowInfo()
        {
            base.ShowInfo();
            Console.WriteLine($"  Кафедра:           {Department}");
            Console.WriteLine($"  Ученая степень:    {GetDegreeText()}");
            Console.WriteLine($"  Стаж (лет):        {Experience}");

            if (courses.Count == 0)
            {
                Console.WriteLine("  Ведет курсы:       пока нет");
                return;
            }
            Console.WriteLine("  Ведет курсы:");
            foreach (Course course in GetCourses())
                Console.WriteLine($"    - {course.Title}");
        }

        public override string ToString()
        {
            return base.ToString() + $", кафедра \"{Department}\"";
        }
    }
}
