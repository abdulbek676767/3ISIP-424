using System;
using System.Collections.Generic;
using System.Linq;

namespace UniversitySystem
{
    class Program
    {
        static University university = new University();

        static void Main()
        {
            bool work = true;
            while (work)
            {
                ShowMenu();
                Console.Write("Выберите пункт меню: ");
                string choice = Console.ReadLine();
                if (choice == null)
                    break;
                Console.WriteLine();

                switch (choice.Trim())
                {
                    case "1": AddStudent(); break;
                    case "2": ShowPersonInfo(ChooseStudent()); break;
                    case "3": EnrollStudent(); break;
                    case "4": ShowStudentCourses(); break;
                    case "5": AddTeacher(); break;
                    case "6": ShowPersonInfo(ChooseTeacher()); break;
                    case "7": AssignTeacher(); break;
                    case "8": AddCourse(); break;
                    case "9": ShowCourseInfo(); break;
                    case "10": ShowCourseStudents(); break;
                    case "11": ShowAllStudents(); break;
                    case "12": ShowAllTeachers(); break;
                    case "13": ShowAllCourses(); break;
                    case "0":
                        work = false;
                        Console.WriteLine("Работа программы завершена");
                        break;
                    default:
                        Console.WriteLine("Ошибка: такого пункта нет в меню");
                        break;
                }
            }
        }

        static void ShowMenu()
        {
            Console.WriteLine();
            Console.WriteLine("============ УНИВЕРСИТЕТ ============");
            Console.WriteLine("Студенты:");
            Console.WriteLine("   1 - Добавить студента");
            Console.WriteLine("   2 - Информация о студенте");
            Console.WriteLine("   3 - Записать студента на курс");
            Console.WriteLine("   4 - Курсы студента");
            Console.WriteLine("Преподаватели:");
            Console.WriteLine("   5 - Добавить преподавателя");
            Console.WriteLine("   6 - Информация о преподавателе");
            Console.WriteLine("   7 - Назначить преподавателя на курс");
            Console.WriteLine("Курсы:");
            Console.WriteLine("   8 - Создать курс");
            Console.WriteLine("   9 - Информация о курсе");
            Console.WriteLine("  10 - Студенты курса");
            Console.WriteLine("Списки:");
            Console.WriteLine("  11 - Все студенты");
            Console.WriteLine("  12 - Все преподаватели");
            Console.WriteLine("  13 - Все курсы");
            Console.WriteLine("   0 - Выход");
            Console.WriteLine("=====================================");
        }

        // ===== Студенты =====

        static void AddStudent()
        {
            Console.WriteLine("--- Добавление студента ---");
            string name = ReadName("ФИО: ");
            int age = ReadNumber("Возраст (16-60): ", 16, 60);
            string phone = ReadPhone("Телефон: ");
            string email = ReadEmail("Email: ");
            string group = ReadGroup("Группа: ");
            int year = ReadNumber("Курс обучения (1-6): ", 1, 6);

            Student student = new Student(name, age, phone, email, group, year);
            university.AddPerson(student);
            Console.WriteLine($"Студент добавлен под номером {student.Id}");
        }

        // Полиморфизм: метод принимает базовый тип Person,
        // а ShowInfo() вызывается свой - студента или преподавателя
        static void ShowPersonInfo(Person person)
        {
            if (person == null)
                return;
            Console.WriteLine();
            person.ShowInfo();
        }

        static void EnrollStudent()
        {
            Console.WriteLine("--- Запись студента на курс ---");
            Student student = ChooseStudent();
            if (student == null)
                return;
            Course course = ChooseCourse();
            if (course == null)
                return;

            // результат записи - значение перечисления, поэтому разбираем его через switch
            switch (student.EnrollCourse(course))
            {
                case EnrollResult.Success:
                    Console.WriteLine($"{student.FullName} записан(а) на курс \"{course.Title}\"");
                    break;
                case EnrollResult.AlreadyEnrolled:
                    Console.WriteLine($"{student.FullName} уже записан(а) на курс \"{course.Title}\"");
                    break;
                case EnrollResult.CourseFull:
                    Console.WriteLine($"На курсе \"{course.Title}\" нет свободных мест");
                    break;
            }
        }

        static void ShowStudentCourses()
        {
            Student student = ChooseStudent();
            if (student == null)
                return;

            List<Course> courses = student.GetCourses();
            Console.WriteLine($"\nКурсы студента {student.FullName}:");
            if (courses.Count == 0)
            {
                Console.WriteLine("  Студент пока не записан ни на один курс");
                return;
            }
            foreach (Course course in courses)
                Console.WriteLine("  " + course);
            Console.WriteLine($"Всего курсов: {courses.Count}, часов: {courses.Sum(c => c.Hours)}");
        }

        // ===== Преподаватели =====

        static void AddTeacher()
        {
            Console.WriteLine("--- Добавление преподавателя ---");
            string name = ReadName("ФИО: ");
            int age = ReadNumber("Возраст (22-90): ", 22, 90);
            string phone = ReadPhone("Телефон: ");
            string email = ReadEmail("Email: ");
            string department = ReadTitle("Кафедра: ", 50);
            AcademicDegree degree = ReadDegree();
            int experience = ReadNumber($"Стаж работы в годах (0-{age - 18}): ", 0, age - 18);

            Teacher teacher = new Teacher(name, age, phone, email, department, degree, experience);
            university.AddPerson(teacher);
            Console.WriteLine($"Преподаватель добавлен под номером {teacher.Id}");
        }

        static AcademicDegree ReadDegree()
        {
            Console.WriteLine("Ученая степень:");
            Console.WriteLine("  1 - без степени");
            Console.WriteLine("  2 - кандидат наук");
            Console.WriteLine("  3 - доктор наук");
            int number = ReadNumber("Выберите (1-3): ", 1, 3);
            return (AcademicDegree)(number - 1);
        }

        static void AssignTeacher()
        {
            Console.WriteLine("--- Назначение преподавателя на курс ---");
            Course course = ChooseCourse();
            if (course == null)
                return;
            Teacher teacher = ChooseTeacher();
            if (teacher == null)
                return;

            if (course.Teacher == teacher)
            {
                Console.WriteLine($"{teacher.FullName} уже ведет курс \"{course.Title}\"");
                return;
            }
            course.SetTeacher(teacher);
            Console.WriteLine($"Теперь курс \"{course.Title}\" ведет {teacher.FullName}");
        }

        // ===== Курсы =====

        static void AddCourse()
        {
            Console.WriteLine("--- Создание курса ---");
            if (university.GetTeachers().Count == 0)
            {
                Console.WriteLine("У каждого курса должен быть преподаватель.");
                Console.WriteLine("Сначала добавьте преподавателя (пункт 5)");
                return;
            }

            string title = ReadCourseTitle();
            int hours = ReadNumber("Количество часов (1-500): ", 1, 500);
            int maxStudents = ReadNumber("Количество мест (1-100): ", 1, 100);
            Console.WriteLine("Кто будет вести курс?");
            Teacher teacher = ChooseTeacher();
            if (teacher == null)
            {
                Console.WriteLine("Курс не создан: без преподавателя курс создать нельзя");
                return;
            }

            Course course = new Course(title, hours, maxStudents, teacher);
            university.AddCourse(course);
            Console.WriteLine($"Курс \"{course.Title}\" создан под номером {course.Id}");
        }

        static void ShowCourseInfo()
        {
            Course course = ChooseCourse();
            if (course == null)
                return;
            Console.WriteLine();
            course.ShowInfo();
        }

        static void ShowCourseStudents()
        {
            Course course = ChooseCourse();
            if (course == null)
                return;

            List<Student> students = course.GetStudents();
            Console.WriteLine($"\nСтуденты курса \"{course.Title}\":");
            if (students.Count == 0)
            {
                Console.WriteLine("  На этот курс пока никто не записан");
                return;
            }
            foreach (Student student in students)
                Console.WriteLine("  " + student);
            Console.WriteLine($"Всего студентов: {students.Count}");
        }

        // ===== Полные списки =====

        static void ShowAllStudents()
        {
            List<Student> students = university.GetStudents();
            Console.WriteLine($"Все студенты ({students.Count}):");
            if (students.Count == 0)
                Console.WriteLine("  Список пуст");
            foreach (Student student in students)
                Console.WriteLine("  " + student);
        }

        static void ShowAllTeachers()
        {
            List<Teacher> teachers = university.GetTeachers();
            Console.WriteLine($"Все преподаватели ({teachers.Count}):");
            if (teachers.Count == 0)
                Console.WriteLine("  Список пуст");
            foreach (Teacher teacher in teachers)
                Console.WriteLine("  " + teacher);
        }

        static void ShowAllCourses()
        {
            List<Course> courses = university.GetCourses();
            Console.WriteLine($"Все курсы ({courses.Count}):");
            if (courses.Count == 0)
                Console.WriteLine("  Список пуст");
            foreach (Course course in courses)
                Console.WriteLine("  " + course);
        }

        // ===== Выбор по номеру (0 - отмена) =====

        static Student ChooseStudent()
        {
            List<Student> students = university.GetStudents();
            if (students.Count == 0)
            {
                Console.WriteLine("Студентов пока нет. Сначала добавьте студента (пункт 1)");
                return null;
            }

            ShowAllStudents();
            int maxId = students.Max(s => s.Id);
            while (true)
            {
                int id = ReadNumber("Введите номер студента (0 - отмена): ", 0, maxId);
                if (id == 0)
                {
                    Console.WriteLine("Действие отменено");
                    return null;
                }
                Student student = university.FindStudent(id);
                if (student != null)
                    return student;
                Console.WriteLine("Ошибка: студента с таким номером нет");
            }
        }

        static Teacher ChooseTeacher()
        {
            List<Teacher> teachers = university.GetTeachers();
            if (teachers.Count == 0)
            {
                Console.WriteLine("Преподавателей пока нет. Сначала добавьте преподавателя (пункт 5)");
                return null;
            }

            ShowAllTeachers();
            int maxId = teachers.Max(t => t.Id);
            while (true)
            {
                int id = ReadNumber("Введите номер преподавателя (0 - отмена): ", 0, maxId);
                if (id == 0)
                {
                    Console.WriteLine("Действие отменено");
                    return null;
                }
                Teacher teacher = university.FindTeacher(id);
                if (teacher != null)
                    return teacher;
                Console.WriteLine("Ошибка: преподавателя с таким номером нет");
            }
        }

        static Course ChooseCourse()
        {
            List<Course> courses = university.GetCourses();
            if (courses.Count == 0)
            {
                Console.WriteLine("Курсов пока нет. Сначала создайте курс (пункт 8)");
                return null;
            }

            ShowAllCourses();
            int maxId = courses.Max(c => c.Id);
            while (true)
            {
                int id = ReadNumber("Введите номер курса (0 - отмена): ", 0, maxId);
                if (id == 0)
                {
                    Console.WriteLine("Действие отменено");
                    return null;
                }
                Course course = university.FindCourse(id);
                if (course != null)
                    return course;
                Console.WriteLine("Ошибка: курса с таким номером нет");
            }
        }

        // ===== Проверка ввода =====

        // непустая строка не длиннее maxLength
        static string ReadText(string message, int maxLength)
        {
            while (true)
            {
                Console.Write(message);
                string input = Console.ReadLine();
                if (input == null)
                    Environment.Exit(0); // ввод закончился (Ctrl+Z) - просто закрываем программу
                input = input.Trim();

                if (input == "")
                    Console.WriteLine("Ошибка: значение не может быть пустым");
                else if (input.Length > maxLength)
                    Console.WriteLine($"Ошибка: слишком длинно, максимум {maxLength} символов");
                else
                    return input;
            }
        }

        // целое число от min до max
        static int ReadNumber(string message, int min, int max)
        {
            while (true)
            {
                string input = ReadText(message, 11);
                int number;
                if (!int.TryParse(input, out number))
                    Console.WriteLine("Ошибка: нужно ввести целое число");
                else if (number < min || number > max)
                    Console.WriteLine($"Ошибка: число должно быть от {min} до {max}");
                else
                    return number;
            }
        }

        // текст, в котором обязательно есть буквы (кафедра, название курса)
        static string ReadTitle(string message, int maxLength)
        {
            while (true)
            {
                string input = ReadText(message, maxLength);
                if (input.Any(char.IsLetter))
                    return input;
                Console.WriteLine("Ошибка: значение должно содержать буквы");
            }
        }

        // название курса не должно повторяться
        static string ReadCourseTitle()
        {
            while (true)
            {
                string title = ReadTitle("Название курса: ", 50);
                if (!university.CourseExists(title))
                    return title;
                Console.WriteLine("Ошибка: курс с таким названием уже есть");
            }
        }

        // ФИО: от 2 до 4 слов, только буквы (внутри слова можно дефис)
        static string ReadName(string message)
        {
            while (true)
            {
                string input = ReadText(message, 80);
                string[] words = input.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                bool correct = words.Length >= 2 && words.Length <= 4 && words.All(w => IsNameWord(w));
                if (correct)
                    return string.Join(" ", words);
                Console.WriteLine("Ошибка: введите фамилию и имя (можно и отчество) буквами через пробел");
            }
        }

        static bool IsNameWord(string word)
        {
            return char.IsLetter(word[0])
                && char.IsLetter(word[word.Length - 1])
                && word.All(c => char.IsLetter(c) || c == '-');
        }

        // телефон: в начале можно +, дальше от 10 до 12 цифр
        // пробелы, скобки и дефисы убираем: +7 (999) 123-45-67 -> +79991234567
        static string ReadPhone(string message)
        {
            while (true)
            {
                string input = ReadText(message, 20);
                string phone = input.Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", "");
                string digits = phone.StartsWith("+") ? phone.Substring(1) : phone;
                if (digits.Length >= 10 && digits.Length <= 12 && digits.All(c => c >= '0' && c <= '9'))
                    return phone;
                Console.WriteLine("Ошибка: телефон должен содержать 10-12 цифр, например 89991234567");
            }
        }

        // email: одна @, имя до нее, домен с точкой после, без пробелов и не занят
        static string ReadEmail(string message)
        {
            while (true)
            {
                string email = ReadText(message, 50).ToLower();
                int at = email.IndexOf('@');
                int dot = email.LastIndexOf('.');
                bool correct = at > 0
                    && at == email.LastIndexOf('@')
                    && dot > at + 1
                    && dot < email.Length - 1
                    && !email.Contains(" ");

                if (!correct)
                    Console.WriteLine("Ошибка: неверный email, пример: ivanov@mail.ru");
                else if (university.EmailExists(email))
                    Console.WriteLine("Ошибка: такой email уже есть в системе");
                else
                    return email;
            }
        }

        // группа: буквы, цифры и дефис, например 3ИСИП-424
        static string ReadGroup(string message)
        {
            while (true)
            {
                string group = ReadText(message, 15).ToUpper();
                bool correct = group.All(c => char.IsLetterOrDigit(c) || c == '-')
                    && group.Any(char.IsLetter)
                    && group.Any(char.IsDigit);
                if (correct)
                    return group;
                Console.WriteLine("Ошибка: группа пишется буквами и цифрами, например 3ИСИП-424");
            }
        }
    }
}
