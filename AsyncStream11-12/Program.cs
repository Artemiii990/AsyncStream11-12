using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

class MyClass
{
    public static async Task Main(string[] args) // Використовуємо async для асинхрогого таску
    {
        // Путь к исходному файлу
        string inputFilePath = "C:\\Users\\mehan\\RiderProjects\\AsyncStream11-12\\AsyncStream11-12\\text.txt";
        // Путь к файлу, в который будут записаны данные
        string outputFilePath = "C:\\Users\\mehan\\RiderProjects\\AsyncStream11-12\\AsyncStream11-12\\Copied.txt";

        // Проверка существования файла
        if (!File.Exists(inputFilePath))
        {
            Console.WriteLine("Исходный файл не найден.");
            return;
        }

        try
        {
            // Чтение данных из файла асинхронно
            byte[] buffer = new byte[100];
            using (FileStream fileStream = new FileStream(inputFilePath, FileMode.Open, FileAccess.Read, FileShare.Read, 1024, FileOptions.Asynchronous))
            {
                int readBytes = await fileStream.ReadAsync(buffer, 0, buffer.Length);
                Console.WriteLine("Прочитано {0} байт", readBytes);

                // Преобразуем считанные байты в строку
                string content = Encoding.UTF8.GetString(buffer, 0, readBytes);
                Console.WriteLine(content);

                // Записываем считанные данные в новый файл
                using (FileStream outputFileStream = new FileStream(outputFilePath, FileMode.Create, FileAccess.Write))
                {
                    byte[] outputBuffer = Encoding.UTF8.GetBytes(content);
                    await outputFileStream.WriteAsync(outputBuffer, 0, outputBuffer.Length);
                }

                Console.WriteLine("Данные скопированы в файл.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Произошла ошибка: " + ex.Message);
        }
    }
}