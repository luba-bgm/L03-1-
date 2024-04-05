using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

class Server
{
    static void Main()
    {
        // параметры сервера
        int port = 8888;
        string serverFolderPath = "C:\\Users\\Любовь\\OneDrive\\Рабочий стол\\repos\\clientServer\\clientServer\\server\\data\\";

        // создание сервера и ожидание подключения клиента
        TcpListener server = new TcpListener(IPAddress.Any, port);
        server.Start();
        Console.WriteLine("Server started!");

        bool exit = false;

        while (!exit)
        {
            TcpClient client = server.AcceptTcpClient();
            Console.WriteLine("Client connected");

            // получение запроса от клиента
            NetworkStream stream = client.GetStream();
            byte[] requestBytes = new byte[client.ReceiveBufferSize];
            int requestDataSize = stream.Read(requestBytes, 0, client.ReceiveBufferSize);
            string requestString = Encoding.ASCII.GetString(requestBytes, 0, requestDataSize);
            Console.WriteLine("Request received: " + requestString);

            // разбор и обработка запроса
            string[] requestParts = requestString.Split(' ');
            string action = requestParts[0].ToLower();
            string filename = requestParts[1];

            string responseString = "";

            if (action == "get")
            {
                string filepath = serverFolderPath + filename;
                if (File.Exists(filepath))
                {
                    string fileContent = File.ReadAllText(filepath);
                    responseString = "200\nThe content of the file is: " + fileContent;
                }
                else
                {
                    responseString = "404\nThe response says that the file was not found!";
                }
            }
            else if (action == "put")
            {
                string fileContent = requestParts[2];
                string filepath = serverFolderPath + filename;
                if (File.Exists(filepath))
                {
                    responseString = "403\nThe response says that creating the file was forbidden!";
                }
                else
                {
                    File.WriteAllText(filepath, fileContent);
                    responseString = "200\nThe response says that the file was created!";
                }
            }
            else if (action == "delete")
            {
                string filepath = serverFolderPath + filename;
                if (File.Exists(filepath))
                {
                    File.Delete(filepath);
                    responseString = "200\nThe response says that the file was successfully deleted!";
                }
                else
                {
                    responseString = "404\nThe response says that the file was not found!";
                }
            }
            else
            {
                responseString = "400";
            }

            // отправка ответа клиенту
            byte[] responseBytes = Encoding.ASCII.GetBytes(responseString);
            stream.Write(responseBytes, 0, responseBytes.Length);
            Console.WriteLine("Response sent: " + responseString);

            // проверка на выход
            if (action == "exit")
            {
                exit = true;
                break;
            }

            stream.Close();
            client.Close();
        }
        server.Stop();

        Console.WriteLine("Server stopped");
    }
}