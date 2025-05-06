using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

class MyClass
{
    public static async Task Main(string[] args)
    {
        // Путь к исходному файлу
        string inputFilePath = "C:\\Users\\mehan\\RiderProjects\\AsyncStream11-12\\AsyncStream11-12\\text.txt";
        // Путь к файлу, в который будут записаны данные
        string outputFilePath = "C:\\Users\\mehan\\RiderProjects\\AsyncStream11-12\\AsyncStream11-12\\Copied.txt";

        // Количество потоков для чтения
        int threadCount = 4; 

        // Проверка существования файла
        if (!File.Exists(inputFilePath))
        {
            Console.WriteLine("Исходный файл не найден.");
            return;
        }

        try
        {
            // Получаем размер файла
            long fileSize = new FileInfo(inputFilePath).Length;
            long chunkSize = fileSize / threadCount;

            Task[] tasks = new Task[threadCount];

            using (FileStream outputFileStream = new FileStream(outputFilePath, FileMode.Create, FileAccess.Write))
            {
                for (int i = 0; i < threadCount; i++)
                {
                    long offset = i * chunkSize;
                    long bytesToRead = (i == threadCount - 1) ? fileSize - offset : chunkSize;

                    tasks[i] = CopyChunkAsync(inputFilePath, outputFileStream, offset, bytesToRead);
                }

                await Task.WhenAll(tasks);
            }

            Console.WriteLine("Данные скопированы в файл.");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Произошла ошибка: " + ex.Message);
        }
    }

    public static async Task CopyChunkAsync(string inputFilePath, FileStream outputFileStream, long offset, long bytesToRead)
    {
        byte[] buffer = new byte[1024]; // Буфер 

        using (FileStream inputFileStream = new FileStream(inputFilePath, FileMode.Open, FileAccess.Read, FileShare.Read, 1024, FileOptions.Asynchronous))
        {
            inputFileStream.Seek(offset, SeekOrigin.Begin); // Перемещение указателя на нужную позицию

            long totalBytesRead = 0;
            while (totalBytesRead < bytesToRead)
            {
                int bytesRead = await inputFileStream.ReadAsync(buffer, 0, Math.Min(buffer.Length, (int)(bytesToRead - totalBytesRead)));
                if (bytesRead == 0)
                {
                    break;
                } 
                await outputFileStream.WriteAsync(buffer, 0, bytesRead);
                totalBytesRead += bytesRead;
            }
        }
    }
}
