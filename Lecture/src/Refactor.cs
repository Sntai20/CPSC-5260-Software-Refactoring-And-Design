namespace Lecture;

public class Refactor
{
    // Before - Split loop refactoring
    void PrintStudentAgeDebt(Student[] students)
    {
        double averageAge = 0;
        double totalDebt = 0;

        for (int i = 0; i < students.Length; i++)
        {
            averageAge += students[i].Age;
            totalDebt += students[i].Debt;
        }

        averageAge /= students.Length;
        Console.WriteLine(averageAge);
        Console.WriteLine(totalDebt);
    }

    // After - Split loop refactoring
    void PrintStudentAgeDebtNew(Student[] students)
    {
        PrintStudentAge(students);
        PrintStudentDebt(students);
    }

    void PrintStudentDebt(Student[] students)
    {
        double totalDebt = 0;

        for (int i = 0; i < students.Length; i++)
        {
            totalDebt += students[i].Debt;
        }

        Console.WriteLine(totalDebt);
    }

    void PrintStudentAge(Student[] students)
    {
        double averageAge = 0;

        for (int i = 0; i < students.Length; i++)
        {
            averageAge += students[i].Age;
        }

        averageAge /= students.Length;
        Console.WriteLine(averageAge);
    }

    class Student
    {
        public int Age { get; set; }

        public double Debt { get; set; }
    }
}