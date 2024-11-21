using System;
using NLog;

namespace MathOperationsApp
{
    class Program
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        static void Main(string[] args)
        {
            ConfigureLogger();
            Console.WriteLine("Добро пожаловать в приложение для выполнения математических операций!");

            while (true)
            {
                try
                {
                    PerformOperation();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Произошла ошибка. Подробности записаны в лог.");
                }

                Console.Write("\nХотите выполнить еще одну операцию? (да/нет): ");
                string response = Console.ReadLine()?.ToLower();
                if (response != "да")
                    break;
            }

            Console.WriteLine("Программа завершена.");
        }

        private static void PerformOperation()
        {
            try
            {
                string operation = GetOperation();
                double num1 = GetNumber("Введите первое число: ");
                double num2 = GetNumber("Введите второе число: ");

                double result = Calculate(num1, num2, operation);
                Console.WriteLine($"Результат: {result}");
            }
            catch (DivideByZeroException ex)
            {
                LogError(ex, "Деление на ноль невозможно.");
                Console.WriteLine("Ошибка: Деление на ноль невозможно.");
            }
            catch (FormatException ex)
            {
                LogError(ex, "Некорректный ввод числа.");
                Console.WriteLine("Ошибка: Некорректный ввод числа.");
            }
            catch (OverflowException ex)
            {
                LogError(ex, "Переполнение типа данных.");
                Console.WriteLine("Ошибка: Введенное число слишком большое или слишком маленькое.");
            }
            catch (InvalidInputException ex)
            {
                LogError(ex, ex.Message);
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
            catch (Exception ex)
            {
                LogError(ex, "Неизвестная ошибка.");
                Console.WriteLine("Произошла неизвестная ошибка.");
            }
        }

        private static string GetOperation()
        {
            Console.WriteLine("\nВыберите операцию:");
            Console.WriteLine("1. Сложение (+)");
            Console.WriteLine("2. Вычитание (-)");
            Console.WriteLine("3. Умножение (*)");
            Console.WriteLine("4. Деление (/)");

            Console.Write("Введите символ операции: ");
            string operation = Console.ReadLine();

            if (operation != "+" && operation != "-" && operation != "*" && operation != "/")
                throw new InvalidInputException("Недопустимая операция!");

            return operation;
        }

        private static double GetNumber(string message)
        {
            Console.Write(message);
            return Convert.ToDouble(Console.ReadLine());
        }

        private static double Calculate(double num1, double num2, string operation)
        {
            return operation switch
            {
                "+" => checked((int)num1 + (int)num2), // Преобразование к int для переполнения
                "-" => checked((int)num1 - (int)num2),
                "*" => checked((int)num1 * (int)num2),
                "/" => num2 != 0 ? num1 / num2 : throw new DivideByZeroException(),
                _ => throw new InvalidInputException("Недопустимая операция!")
            };
        }

        private static void ConfigureLogger()
        {
            var config = new NLog.Config.LoggingConfiguration();

            var logfile = new NLog.Targets.FileTarget("logfile") { FileName = "logfile.txt" };
            config.AddRule(LogLevel.Error, LogLevel.Fatal, logfile);

            LogManager.Configuration = config;
        }

        private static void LogError(Exception ex, string message)
        {
            Logger.Error(ex, $"Ошибка: {message}. Детали: {ex.StackTrace}");
        }
    }

    public class InvalidInputException : Exception
    {
        public InvalidInputException(string message) : base(message) { }
    }
}
