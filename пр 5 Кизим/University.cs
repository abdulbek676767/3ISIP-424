using System;
using System.Collections.Generic;
using System.Linq;

// хранит всех студентов, преподавателей и курсы
class University
{
    private List<Student> students = new List<Student>();
    private List<Teacher> teachers = new List<Teacher>();
    private List<Course> courses = new List<Course>();

    public int StudentsCount { get { return students.Count; } }
    public int TeachersCount { get { return teachers.Count; } }
    public int CoursesCount { get { return courses.Count; } }

    public Student AddStudent(string fullName, int age, string phone, string email, string group, int year)
    {
        Student student = new Student(students.Count + 1, fullName, age, phone, email, group, year);
        students.Add(student);
        return student;
    }

    public Teacher AddTeacher(string fullName, int age, string phone, string email, string department, Degree degree, int experience)
    {
        Teacher teacher = new Teacher(teachers.Count + 1, fullName, age, phone, email, department, degree, experience);
        teachers.Add(teacher);
        return teacher;
    }

    public Course AddCourse(string title, int hours, int maxStudents)
    {
        Course course = new Course(courses.Count + 1, title, hours, maxStudents);
        courses.Add(course);
        return course;
    }

    // поиск по номеру, если не нашли - вернется null
    public Student FindStudent(int id)
    {
        return students.FirstOrDefault(s => s.Id == id);
    }

    public Teacher FindTeacher(int id)
    {
        return teachers.FirstOrDefault(t => t.Id == id);
    }

    public Course FindCourse(int id)
    {
        return courses.FirstOrDefault(c => c.Id == id);
    }

    // проверка, чтобы не было двух курсов с одинаковым названием
    public bool CourseExists(string title)
    {
        return courses.Any(c => c.Title.ToLower() == title.ToLower());
    }

    public List<Student> GetStudents()
    {
        return students.OrderBy(s => s.Id).ToList();
    }

    public List<Teacher> GetTeachers()
    {
        return teachers.OrderBy(t => t.Id).ToList();
    }

    public List<Course> GetCourses()
    {
        return courses.OrderBy(c => c.Id).ToList();
    }

    // все люди университета в одном списке через базовый класс Person
    public List<Person> GetAllPeople()
    {
        List<Person> people = new List<Person>();
        people.AddRange(teachers);
        people.AddRange(students);
        return people;
    }
}
