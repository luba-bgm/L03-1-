using System;
using System.Net.Sockets;
using System.Text;

class Client
{
    static void Main()
    {
        // Параметры сервера
        string serverIP = "127.0.0.1";
        int port = 8888;

        // Создание подключения к серверу
        TcpClient client = new TcpClient(serverIP, port);
        NetworkStream stream = client.GetStream();

        // Ввод действия пользователем
        Console.Write("Enter action (1 - get a file, 2 - create a file, 3 - delete a file, 4 - exit): ");
        string action = Console.ReadLine().ToLower();

        // Формирование и отправка запроса на сервер
        string requestString = "";
        if (action == "1" )
        {
            Console.Write("Enter filename: ");
            string fileName = Console.ReadLine();
            requestString = "get " + fileName;
        }
        else if (action == "2" )
        {
            Console.Write("Enter filename: ");
            string fileName = Console.ReadLine();
            Console.Write("Enter file content: ");
            string fileContent = Console.ReadLine();
            requestString = "put " + fileName + " " + fileContent;
        }
        else if (action == "3" )
        {
            Console.Write("Enter filename: ");
            string fileName = Console.ReadLine();
            requestString = "delete " + fileName;
        }
        else if (action == "4")
        {
            requestString = "exit";
        }
        else
        {
            Console.WriteLine("Invalid action");
            requestString = "invalid_action";
        }

        byte[] requestBytes = Encoding.ASCII.GetBytes(requestString);
        stream.Write(requestBytes, 0, requestBytes.Length);
        Console.WriteLine("The request was sent.");

        // Получение ответа от сервера
        byte[] responseBytes = new byte[client.ReceiveBufferSize];
        int responseDataSize = stream.Read(responseBytes, 0, client.ReceiveBufferSize);
        string responseString = Encoding.ASCII.GetString(responseBytes, 0, responseDataSize);
        Console.WriteLine("The response says: " + responseString);

        // Завершение работы
        stream.Close();
        client.Close();
    }
}