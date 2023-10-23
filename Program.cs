using System;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.Design;
using System.Data;
using System.Diagnostics;
using System.Media;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Transactions;
using System.Xml.Schema;

namespace Brain_game
{
    internal class Game
    {
        static void Main(string[] args)
        {
            Console.InputEncoding = Encoding.UTF8;
            Console.OutputEncoding = Encoding.UTF8;
            Function func = new Function();
            bool is_end = false;
            Console.CursorVisible = false;
            Screen_Hello hello = new Screen_Hello();
            hello.process();
            func.Play_Music();
            // in ra chọn game
            Console.Clear();
            func.draw_screen(1, 0);
            ConsoleKeyInfo keyInfo;
            int row = 1, col = 0;
            while (true)
            {
                keyInfo = Console.ReadKey();
                func.HandleKey(keyInfo.Key, ref row, ref col, is_end);
                if (is_end) { break; }
            }
        }
    }
    public class User
    {
        public string UserName { get; set; }
        public int Score { get; set; }
        public DateTime Date { get; set; }
    }

        // Tạo một class để lưu trữ danh sách điểm số của tất cả người chơi
    public class Scores
    {
        private List<User> scores = new List<User>();

        public void AddUser(User score)
        {
            scores.Add(score);
        }
        public void Save()
        {
            // Ghi danh sách điểm số vào file
            string filename = "scores.txt";
            // Sử dụng StreamWriter để ghi từng điểm số vào một dòng của tệp
            using (StreamWriter writer = File.CreateText(filename))
            {
                foreach (User user in scores)
                {
                    writer.WriteLine($"{user.UserName},{user.Score},{user.Date:O}");
                }
                // Đóng luồng ghi sau khi đã xử lý hết tất cả các dòng trong tệp
                writer.Close();
            }
        }

        public void History()
        {
            int a = 2, b = 2;
            string filename = "scores.txt";
            int n = 20;
            if (scores.Count < 20) n = scores.Count;
            for (int i = 0; i < n; i++)
            {
                Console.SetCursorPosition(a, b);
                ++b;
                Console.WriteLine(i + 1 + ". " + scores[i].UserName + " " + scores[i].Score + " " + scores[i].Date);
            }
            ConsoleKeyInfo keyInfo;
            keyInfo = Console.ReadKey();
        }
        public void In_BXH()
        {
            string filename = "scores.txt";
            int[] order = new int[scores.Count];
            for (int i = 0; i < scores.Count; i++)
                order[i] = i;
            for (int i = 0; i < scores.Count; i++)
            {
                for (int j = 0; j < i; ++j)
                    if (scores[order[i]].Score > scores[order[j]].Score)
                    {
                        int tmp = order[i];
                        order[i] = order[j];
                        order[j] = tmp;
                    }
            }
            int a = 2, b = 2;
            int n = 5;
            if (scores.Count > n) n = scores.Count;
            for (int i = 0; i < n; i++)
            {
                Console.SetCursorPosition(a, b);
                ++b;
                Console.WriteLine(i + 1 + ". " + scores[order[i]].UserName + " " + scores[order[i]].Score);
            }
            ConsoleKeyInfo keyInfo;
            keyInfo = Console.ReadKey();
        }
        public void Load()
        {
            // Đọc danh sách điểm số từ file
            string filename = "scores.txt";
            // Kiểm tra file có tồn tại ko
            if (File.Exists(filename))
            {
                // Sử dụng StreamReader để đọc từng dòng của tệp
                using (StreamReader reader = File.OpenText(filename))
                {
                    // Kiểm tra xem đã đọc hết tệp chưa
                    while (!reader.EndOfStream)
                    {
                        string line = reader.ReadLine();
                        // Phân tách dòng vừa đọc thành các token. Mỗi token là một chuỗi con của dòng, được phân tách bởi dấu phẩy
                        string[] tokens = line.Split(',');
                        if (tokens.Length == 3)
                        {
                            User user = new User
                            {
                                UserName = tokens[0],
                                Score = int.Parse(tokens[1]),
                                Date = DateTime.Parse(tokens[2])
                            };
                            scores.Add(user);
                        }
                    }
                    // Đóng luồng đọc sau khi đã xử lý hết tất cả các dòng trong tệp
                    reader.Close();
                }
            }
        }
    }
    struct Function 
    {
        static bool is_playing = false;
        public void Play_Music()
        {
            SoundPlayer player = new SoundPlayer();
            if (is_playing == false)
            {
                is_playing = true;
                //Chạy nhạc nền cho game
                player.SoundLocation = "C:\\Users\\vthuy\\source\\repos\\cai_dau_game\\cai_dau_game\\RenaiCirculation-KanaHanazawa_47xq7.wav";
                // Cho nhạc lặp lại 
                player.PlayLooping();
            }
            else
            {
                is_playing = false;
                player.Stop();
            }
        }

        // Tạo hàm chứa try-catch để bắt ngoại lệ khi nhấn sai phím bắt đầu trò chơi
        public void Exception(string s)
        {
            int cnt = 0; 
            while (true)
            {
                //Khi buffer đang có kí tự thì đọc để nó không in ra màn hình 
                ++cnt;
                while (Console.KeyAvailable) Console.ReadKey(true);
                try
                {   // Ghi nhận phím từ người chơi
                    var key = Console.ReadKey(true);
                    // Nếu như phím nhận không phải là chữ cái thì tự động bỏ qua và Nếu phím nhấn vào không phải là Enter thì được cho là ngoại lệ //
                    if (key.Key != ConsoleKey.Enter)
                    {
                        Console.SetCursorPosition(10, 22);
                        if (cnt == 1) throw new Exception("Bạn chọn sai phím rồi! Vui lòng nhấn Enter để bắt đầu");
                    }
                    // Nếu phím nhận là Enter, kết thúc vòng lặp
                    else break;
                }
                // Bắt ngoại lệ và đưa ra thông báo 
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }
        public void draw_screen(int row, int col)
        {
            Console.Clear();
            Screen_Hello hello = new Screen_Hello();
            Console.WriteLine(hello.DrawFrame(70, 25));
            Console.SetCursorPosition(25, 10);
            Console.WriteLine("Mời bạn chọn game: ");
            Console.SetCursorPosition(1, 1);
            Console.ForegroundColor = ConsoleColor.White;
            if (row == 0) Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(">> ");
            Console.ForegroundColor = ConsoleColor.White;
            if (row == 1 && col == 0) Console.ForegroundColor = ConsoleColor.Green;
            Console.SetCursorPosition(15, 12);
            Console.Write("__________");
            Console.SetCursorPosition(15, 13);
            Console.Write("| Memory |");
            Console.SetCursorPosition(15, 14);
            Console.Write("|_Matrix_|");
            Console.ForegroundColor = ConsoleColor.White;
            if (row == 1 && col == 1) Console.ForegroundColor = ConsoleColor.Green;
            Console.SetCursorPosition(30, 12);
            Console.Write("__________");
            Console.SetCursorPosition(30, 13);
            Console.Write("|Matching|");
            Console.SetCursorPosition(30, 14);
            Console.Write("|__Game__|");
            Console.ForegroundColor = ConsoleColor.White;
            if (row == 1 && col == 2) Console.ForegroundColor = ConsoleColor.Green;
            Console.SetCursorPosition(45, 12);
            Console.Write("__________");
            Console.SetCursorPosition(45, 13);
            Console.Write("|  Find  |");
            Console.SetCursorPosition(45, 14);
            Console.Write("|__Word__|");
            Console.ForegroundColor = ConsoleColor.White;
        }
        public void In_HD(string s)
        {
            Console.ForegroundColor = ConsoleColor.White;
            int a = 2, b = 6, i = 0;
            string tmp = "";
            Console.SetCursorPosition(a, b);
            Console.Write(" - ");
            while (i < s.Length)
            {
                tmp = tmp + s[i];
                if ((s[i] == '.') || (tmp.Length >= 61 && s[i + 1] == ' '))
                {
                    Console.WriteLine(tmp);
                    tmp = "";
                    ++b;
                    Console.SetCursorPosition(a, b);
                    if (s[i] == '.') Console.Write(" - ");
                }
                ++i;
            }
            Console.SetCursorPosition(a, b);
            Console.WriteLine(tmp);
            Console.SetCursorPosition(22, 19);
            Console.Write("Nhấn phím Enter để bắt đầu");
            Console.SetCursorPosition(30, 20);
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.Write("Bắt đầu");
            Console.ForegroundColor = ConsoleColor.White;
            ConsoleKeyInfo keyInfo;
            keyInfo = Console.ReadKey();
        }

        public void Check_and_run(int row, int col)
        {
            if (row == 0)
            {
                Control_Task tmp = new Control_Task();
                tmp.Run(1,0);
            }
            else
            {
                if (col == 0)
                {
                    Game_Matrix tmp = new Game_Matrix();
                    tmp.Run();
                }
                if (col == 1)
                {
                    Matching_Game tmp = new Matching_Game();
                    tmp.Run();
                }
                if (col == 2)
                {
                    Game_Recall tmp = new Game_Recall();
                    tmp.Run();
                }
            }
        }
        public void HandleKey(ConsoleKey key, ref int row, ref int col, bool is_end)
        {
            switch (key)
            {
                case ConsoleKey.RightArrow:
                    col++;
                    if (col > 2) col = 0;
                    break;
                case ConsoleKey.LeftArrow:
                    col--;
                    if (col < 0) col = 2;
                    break;
                case ConsoleKey.DownArrow:
                    row++;
                    if (row > 1) row = 0;
                    break;
                case ConsoleKey.UpArrow:
                    row--;
                    if (row < 0) row = 1;
                    break;
                case ConsoleKey.Enter:
                    Check_and_run(row, col);
                    break;
            }
            draw_screen(row, col);
        }
    }
    struct Control_Task
    {
        static bool turn_main;
        public void draw_CT(int row, int col)
        {
            Console.Clear();
            Screen_Hello hello = new Screen_Hello();
            Console.SetCursorPosition(0, 0);
            Console.WriteLine(hello.DrawFrame(70, 25));
            Console.SetCursorPosition(1, 1);
            Console.ForegroundColor = ConsoleColor.White;
            if (row == 0) Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("<< ");
            Console.ForegroundColor = ConsoleColor.White;
            if (row == 1 && col == 0) Console.ForegroundColor = ConsoleColor.Green;
            Console.SetCursorPosition(15, 12);
            Console.Write("__________");
            Console.SetCursorPosition(15, 13);
            Console.Write("|   Loa  |");
            Console.SetCursorPosition(15, 14);
            Console.Write("|________|");
            Console.ForegroundColor = ConsoleColor.White;
            if (row == 1 && col == 1) Console.ForegroundColor = ConsoleColor.Green;
            Console.SetCursorPosition(30, 12);
            Console.Write("__________");
            Console.SetCursorPosition(30, 13);
            Console.Write("|  Lịch  |");
            Console.SetCursorPosition(30, 14);
            Console.Write("|___sử___|");
            Console.ForegroundColor = ConsoleColor.White;
            if (row == 1 && col == 2) Console.ForegroundColor = ConsoleColor.Green;
            Console.SetCursorPosition(45, 12);
            Console.Write("__________");
            Console.SetCursorPosition(45, 13);
            Console.Write("| Thoát  |");
            Console.SetCursorPosition(45, 14);
            Console.Write("|__Game__|");
            Console.ForegroundColor = ConsoleColor.White;
        }
        public void Run(int row, int col)
        {
            Function func = new Function();
            turn_main = false;
            Console.Clear();
            draw_CT(1, 0);
            ConsoleKeyInfo keyInfo;
            while (true)
            {
                keyInfo = Console.ReadKey();
                HandleKey_CT(keyInfo.Key, ref row, ref col);
                if (turn_main) return;
            }
        }

        static void Check_CT(int row, int col)
        {
            if (row == 0)
            {
                turn_main = true;
                return;
            }
            else
            {
                if (col == 0)
                {
                    Function func = new Function();
                    func.Play_Music();
                }
                if (col == 1)
                {
                    Console.Clear();
                    Screen_Hello hello = new Screen_Hello();
                    Console.WriteLine(hello.DrawFrame(70, 25));
                    Scores htr = new Scores();
                    htr.Load();
                    htr.History();
                }
                if (col == 2)
                {
                    Console.Clear();
                    Console.WriteLine("Tạm biệt!!!");
                    Environment.Exit(0);
                }
            }
        }

        public void HandleKey_CT(ConsoleKey key, ref int row, ref int col)
        {
            switch (key)
            {
                case ConsoleKey.RightArrow:
                    col++;
                    if (col > 2) col = 0;
                    break;
                case ConsoleKey.LeftArrow:
                    col--;
                    if (col < 0) col = 2;
                    break;
                case ConsoleKey.DownArrow:
                    row++;
                    if (row > 1) row = 0;
                    break;
                case ConsoleKey.UpArrow:
                    row--;
                    if (row < 0) row = 1;
                    break;
                case ConsoleKey.Enter:
                    Check_CT(row, col);
                    break;
            }
            draw_CT(row, col);
        }
    }

    struct Screen_Hello
    {
        const int STD_OUTPUT_HANDLE = -11;
        const uint ENABLE_VIRTUAL_TERMINAL_PROCESSING = 4;

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr GetStdHandle(int nStdHandle);

        [DllImport("kernel32.dll")]
        static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);

        [DllImport("kernel32.dll")]
        static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);

        public string DrawFrame(int width, int height)
        {
            StringBuilder frame = new StringBuilder();
            Console.ForegroundColor = ConsoleColor.White;
            // Vẽ hàng đầu tiên của khung
            frame.AppendLine(new string('_', width));

            // Vẽ các hàng giữa của khung
            for (int i = 0; i < height - 2; i++)
            {
                frame.AppendLine("|" + new string(' ', width - 2) + "|");
            }

            // Vẽ hàng cuối cùng của khung
            frame.AppendLine("|" + new string('_', width - 2) + "|");

            return frame.ToString();
        }
        public void process()
        {
            var handle = GetStdHandle(STD_OUTPUT_HANDLE);
            uint mode;
            GetConsoleMode(handle, out mode);
            mode |= ENABLE_VIRTUAL_TERMINAL_PROCESSING;
            SetConsoleMode(handle, mode);

            const string UNDERLINE = "\x1B[4m";
            const string BOLD = "\x1B[1m";
            const string RESET = "\x1B[0m";
            int n = 21;

            Console.WriteLine(DrawFrame(70, 25));
            Thread.Sleep(2000);

            for (int i = 0; i < n; i++)
            {
                Console.Clear();
                Console.WriteLine(DrawFrame(70, 25));
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.SetCursorPosition(20, 10);
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine(BOLD + UNDERLINE + "                    " + RESET);
                Console.SetCursorPosition(20, 11);
                for (int j = 0; j < i; j++)
                {
                    Console.BackgroundColor = ConsoleColor.Blue;
                    Console.Write(BOLD + UNDERLINE + " " + RESET);
                }
                for (int j = i; j < n - 1; j++)
                {
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.Write(BOLD + UNDERLINE + " " + RESET);
                }
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine();
                Console.SetCursorPosition(20, 12);
                Console.Write("        {0}%     ", i * 5);
                Thread.Sleep(150);
            }
        }
    }
    public class Calculate
    {
        public static DateTime startTime;
        public static bool gameIsRunning = false;
        public static void StartRecordingPlaytime()
        {
            startTime = DateTime.Now;
            while (true)
            {
                if (gameIsRunning)
                {
                    TimeSpan elapsed = DateTime.Now - startTime;
                    Console.Clear();
                }
                Thread.Sleep(1000); // Update every second
            }
        }

        public static void DisplayPlaytime()
        {
            TimeSpan elapsed = DateTime.Now - startTime;
            Console.SetCursorPosition(17, 18);
            Console.WriteLine($"Tổng thời gian chơi là: {elapsed.Minutes} phút và {elapsed.Seconds} giây");
            Thread.Sleep(1000);
        }
    }
    public class Game_Matrix : Calculate
    {

        static int[] row = new int[10] { 2, 3, 3, 3, 3, 4, 4, 5, 5, 5 };
        static int[] col = new int[10] { 2, 3, 3, 4, 4, 4, 4, 5, 5, 5 };
        static int[] ran = new int[10] { 2, 3, 5, 4, 6, 5, 8, 6, 8, 10 };
        static int[] score = new int[10] { 100, 150, 200, 250, 300, 350, 400, 450, 500, 550 };
        static int[,] board = new int[10, 10];
        static int[,] visual = new int[10, 10];
        static int luot_dung, luot_sai, cur_row, cur_col, is_end_game, total_score = 0, level = 0;
        public void Run()
        {
            Huong_dan();
            Thread timerThread = new Thread(new ThreadStart(StartRecordingPlaytime));
            Thread Time = new Thread(new ThreadStart(DisplayPlaytime));
            timerThread.Start();
            while (level < 10)
            {
                Console.Clear();
                Screen_Hello hello = new Screen_Hello();
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(hello.DrawFrame(70, 25));
                Console.SetCursorPosition(30, 10);
                Console.BackgroundColor = ConsoleColor.DarkBlue;
                Console.Write("Level {0}", level + 1);
                Thread.Sleep(500);
                Console.BackgroundColor = ConsoleColor.Black;
                ConsoleKeyInfo keyInfo;
                init(level);
                NewBoard(level);
                PrintBoard(level, board);
                Thread.Sleep(2000);
                Console.Clear();
                PrintBoard(level, visual);
                luot_dung = 0; luot_sai = 0;
                while (true)
                {
                    if (luot_sai >= 3 || luot_dung == ran[level]) break;
                    keyInfo = Console.ReadKey();
                    HandleKey(level, keyInfo.Key);
                }
                //Nếu (luot_chon > ran[i]) quay lại màn trước 
                //nếu == thì tăng 1 rank
                if (luot_sai >= 3)
                {
                    PrintEnd(level);
                    Thread.Sleep(2000);
                    is_end_game++;
                }
                else
                {
                    int diemtru = score[level] - luot_sai * 25;
                    ++level;
                }
                if (is_end_game == 3)
                {
                    Console.SetCursorPosition(25, 17);
                    break;
                }
            }
            save_result();
            Time.Start();
            Thread.Sleep(2000);
        }

        static void save_result()
        {
            Console.SetCursorPosition(25, 16);
            Console.WriteLine("Nhập tên đi: ");
            User newUser = new User();

            newUser.UserName = Console.ReadLine();
            newUser.Date = DateTime.Now;
            newUser.Score = total_score;

            Scores scoreList = new Scores();
            scoreList.Load();
            scoreList.AddUser(newUser);
            scoreList.Save();
            Console.Clear();
            Screen_Hello hello = new Screen_Hello();
            Console.WriteLine(hello.DrawFrame(70, 25)); 
            Scores bxh = new Scores();
            bxh.Load();
            bxh.In_BXH();
        }

        static void Huong_dan()
        {
            Console.Clear();
            Screen_Hello hello = new Screen_Hello();
            Console.WriteLine(hello.DrawFrame(70, 25));
            Console.SetCursorPosition(20, 4);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("HƯỚNG DẪN GAME MEMORY MATRIX");
            string s = " Bảng sẽ xuất hiện trong vòng 3 giây, cho phép người chơi ghi nhớ " +
                "cấu trúc hình học của các phần tử cần tái tạo. Sau 3 giây, các phần tử sẽ được " +
                "ẩn đi, đưa ma trận về dạng đơn sắc. Người chơi lần lượt chọn từng ô thông qua " +
                "điều khiển mũi tên trên bàn phím. Nếu chọn đúng, ô sẽ trả về phần tử hình tròn ban " +
                "đầu, ngược lại sẽ trả về hình dấu nhân (X). Khi người chơi chọn sai đủ 3 lần sẽ " +
                "bị đưa về màn chơi trước, nếu thành công không sai quá 3 lần thì trò chơi chuyển " +
                "sang màn chơi tiếp theo";
            Function func = new Function();
            func.In_HD(s);
            func.Exception(s);
        }
        static void init(int i) // khởi tạo bảng
        {
            for (int j = 0; j < row[i]; j++)
                for (int k = 0; k < col[i]; k++)
                {
                    board[j, k] = 0;
                    visual[j, k] = 0;
                }
        }

        static void NewBoard(int i) //Tạo random 1 bảng bất kỳ
        {

            for (int t = 0; t < ran[i]; t++)
            {
                int x, y;
                while (true)
                {
                    Random r = new Random();
                    x = r.Next(0, row[i]);
                    y = r.Next(0, col[i]);
                    if (board[x, y] == 0) break;
                }
                board[x, y] = 1;
            }
        }

        static void HandleKey(int i, ConsoleKey key)
        {
            switch (key)
            {
                case ConsoleKey.RightArrow:
                    cur_col++;
                    if (cur_col >= col[i]) cur_col = 0;
                    break;
                case ConsoleKey.LeftArrow:
                    cur_col--;
                    if (cur_col < 0) cur_col = col[i] - 1;
                    break;
                case ConsoleKey.DownArrow:
                    cur_row++;
                    if (cur_row >= row[i]) cur_row = 0;
                    break;
                case ConsoleKey.UpArrow:
                    cur_row--;
                    if (cur_row < 0) cur_row = row[i] - 1;
                    break;
                // nếu chọn vào ô chưa chọn trước đó && ô đó đánh dấu = 1 -> cnt++
                // else nếu chọn ô đã chọn trước đó -> nhập lại đến khi hợp lệ
                // else hiện dấu x;
                case ConsoleKey.Enter:
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.SetCursorPosition(30, 18);
                    // Nếu ô này không phải là ô yêu cầu thì in ra sai
                    if (board[cur_row, cur_col] == 0)
                    {
                        visual[cur_row, cur_col] = -1;
                        PrintBoard(i, visual);
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.SetCursorPosition(30, 18);
                        Console.Write("Sai rồi");
                        Console.SetCursorPosition(60, 03);
                        Console.WriteLine("  - 100");
                        total_score -= 100;
                        if (total_score < 0) total_score = 0;
                        Thread.Sleep(500);
                        ++luot_sai;
                    }
                    else
                        //Trường hợp còn lại của mảng board[,] là = 1
                        //Nếu ô này chưa chọn trước đó thì đánh dấu cho nó 
                        if (visual[cur_row, cur_col] == 0)
                    {
                        visual[cur_row, cur_col] = 1;
                        PrintBoard(i, visual);
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.SetCursorPosition(60, 03);
                        Console.WriteLine("  + 100");
                        total_score += 100;
                        Console.SetCursorPosition(30, 18);
                        ++luot_dung;
                        Console.Write("Đúng rồi");
                        Thread.Sleep(500);
                    }
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
            }
            PrintBoard(i, visual);
        }
        static void PrintEnd(int i)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.White;
            Screen_Hello hello = new Screen_Hello();
            Console.WriteLine(hello.DrawFrame(70, 25));
            int a = 35 - col[i], b = 12 - row[i] / 2;
            Console.SetCursorPosition(a - 5, b - 2);
            Console.WriteLine("Bảng đúng là: ");
            for (int j = 0; j < row[i]; ++j)
            {
                Console.SetCursorPosition(a, b);
                ++b;
                a = 35 - col[i];
                for (int k = 0; k < col[i]; ++k)
                    if (board[j, k] == 0) Console.Write("- ");
                    else Console.Write("o ");
                Console.WriteLine();
            }
        }
        static void PrintBoard(int i, int[,] arr)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.White;
            Screen_Hello hello = new Screen_Hello();
            Console.WriteLine(hello.DrawFrame(70, 25));
            Console.SetCursorPosition(03, 02);
            Console.WriteLine("Level " + (level + 1));
            Console.SetCursorPosition(53, 02);
            Console.WriteLine("Tổng điểm: {0}", total_score);
            int a = 35 - col[i], b = 12 - row[i] / 2;
            Console.SetCursorPosition(a, b);
            for (int j = 0; j < row[i]; j++)
            {
                Console.SetCursorPosition(a, b);
                ++b;
                a = 35 - col[i];
                for (int k = 0; k < col[i]; k++)
                {
                    if (j == cur_row && k == cur_col)
                        Console.ForegroundColor = ConsoleColor.Green;
                    else Console.ForegroundColor = ConsoleColor.White;
                    if (arr[j, k] == -1) Console.Write("x ");
                    else if (arr[j, k] == 0) Console.Write("- ");
                    else Console.Write("o ");
                }
                Console.WriteLine();
            }
        }
    }
    ;
    public class Matching_Game: Calculate
    {
        //Biến của game Matching
        static List<string> symbols = new List<string> { "🍀", "🐻", "🍄", "🍞", "🍑", "🍭", "🎁", "⛅", "🦀" };
        static int[,] gridSizes = { { 2, 2 }, { 2, 3 }, { 3, 4 }, { 3, 4 }, { 4, 4 }, { 4, 4 }, { 4, 5 }, { 4, 5 }, { 6, 6 }, { 6, 6 } };
        static int[,] isFlipped = new int[10, 10];
        static int[,] board = new int[10, 10];
        static int cur_row, cur_col;
        static int pre_row, pre_col, cnt_luot = 0, level = 0, luot_lat_o = 0, is_end_game = 0, total_score = 100;
        public void Run()
        {
            Huong_dan();
            Thread timerThread = new Thread(new ThreadStart(StartRecordingPlaytime));
            Thread Time = new Thread(new ThreadStart(DisplayPlaytime));
            timerThread.Start();
            while (level < 10)
            {
                Console.Clear();
                Screen_Hello hello = new Screen_Hello();
                Console.WriteLine(hello.DrawFrame(70, 25));
                Console.SetCursorPosition(30, 10);
                Console.BackgroundColor = ConsoleColor.DarkBlue;
                Console.Write("Level {0}", level + 1);
                Thread.Sleep(500);
                Console.BackgroundColor = ConsoleColor.Black;
                ConsoleKeyInfo keyInfo;
                init_Match(level);
                NewBoard_Match(level);
                Thread.Sleep(2000);
                luot_lat_o = gridSizes[level, 0] * gridSizes[level, 1] * 3;
                PrintBoard_Match(level);
                while (luot_lat_o > 0)
                {
                    keyInfo = Console.ReadKey();
                    HandleKey_Match(level, keyInfo.Key);
                    if (Check_end() == true) break;
                }
                if (Check_end() == true) ++level;
                else
                    ++is_end_game;
                if (is_end_game == 3)
                {
                    Console.SetCursorPosition(25, 17);
                    Console.WriteLine("Trò chơi đã kết thúc");
                    Thread.Sleep(1000);
                    break;
                }
            }
            Time.Start();
            Thread.Sleep(2000);
        }

        static void Huong_dan()
        {
            Console.Clear();
            Screen_Hello hello = new Screen_Hello();
            Console.WriteLine(hello.DrawFrame(70, 25));

            Console.SetCursorPosition(20, 4);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("HƯỚNG DẪN MATCHING GAME");
            string s = " Khi bắt đầu màn hình sẽ hiện một bảng với các phần tử ẩn. Người chơi sẽ lần lượt lật các phần tử lên bằng " +
                "cách điều khiển các phím mũi tên, bấm Enter để lật hình. Mỗi lần lật, các phần tử sẽ xuất hiện với 1 ký hiệu và yêu " +
                "cầu người chơi ghi nhớ. (Lật tối đa hai lần 1 lượt) Nếu ký hiệu ở 2 lần trùng khớp thì chúng sẽ được lật lên, " +
                "khóa lại. Người chơi sẽ chiến thắng nếu lật được tất cả các cặp lá giống nhau trong thời gian quy định, màn chơi " +
                "sẽ kết thúc ngay tại thời điểm cặp ký hiệu cuối cùng được lật lên";
            Function func = new Function();
            func.In_HD(s);
            func.Exception(s);
        }
        static bool Check_end()
        {
            for (int j = 0; j < gridSizes[level, 0]; j++)
                for (int k = 0; k < gridSizes[level, 1]; k++)
                    if (isFlipped[j, k] != 1) return false;
            return true;
        }

        static void init_Match(int i) // khởi tạo bảng
        {
            for (int j = 0; j < gridSizes[i, 0]; j++)
                for (int k = 0; k < gridSizes[i, 1]; k++)
                {
                    board[j, k] = -1;
                    isFlipped[j, k] = 0;
                }
        }

        static void NewBoard_Match(int i)
        {
            int half = gridSizes[i, 0] * gridSizes[i, 1] / 2;
            int count = -1;
            int cnt_symbol = -1;
            int[] tmp_arr = new int[half]; // lưu các ký tự trong bảng
            for (int j = 0; j < gridSizes[i, 0]; j++)
                for (int k = 0; k < gridSizes[i, 1]; k++)
                {
                    int x, y;
                    if (count + 1 < half)
                        while (true)
                        {
                            Random r = new Random();
                            x = r.Next(0, gridSizes[i, 0]);
                            y = r.Next(0, gridSizes[i, 1]);
                            int tmp = (cnt_symbol + 1) % 9;
                            if (board[x, y] != -1) continue;
                            else
                            {
                                ++count;
                                ++cnt_symbol;
                                board[x, y] = tmp;
                                tmp_arr[count] = tmp;
                                break;
                            }
                        }
                    else break;
                }
            for (int j = 0; j < gridSizes[i, 0]; j++)
                for (int k = 0; k < gridSizes[i, 1]; k++)
                    if (board[j, k] == -1)
                    {
                        board[j, k] = tmp_arr[count];
                        --count;
                    }
        }

        static void PrintBoard_Match(int i)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.White;
            Screen_Hello hello = new Screen_Hello();
            Console.WriteLine(hello.DrawFrame(70, 25));
            Console.SetCursorPosition(02, 02);
            Console.WriteLine("Lượt lật ô còn lại: {0}", luot_lat_o);
            Console.SetCursorPosition(53, 02);
            Console.WriteLine("Tổng điểm: {0}", total_score);
            int a = 35 - gridSizes[i, 0], b = 12 - gridSizes[i, 1] / 2;
            Console.SetCursorPosition(a, b);
            for (int j = 0; j < gridSizes[i, 0]; j++)
            {
                Console.SetCursorPosition(a, b);
                for (int k = 0; k < gridSizes[i, 1]; k++)
                {
                    if (j == cur_row && k == cur_col)
                        Console.BackgroundColor = ConsoleColor.Blue;
                    else Console.BackgroundColor = ConsoleColor.Black;
                    if (isFlipped[j, k] == 0) Console.Write("❓");
                    else
                        if (isFlipped[j, k] == 1) Console.Write("➖");
                    else Console.Write(symbols[board[j, k]]);
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.Write(" ");
                }
                Console.WriteLine();
                ++b;
            }
        }

        static void sosanh()
        {
            PrintBoard_Match(level);
            if ((board[pre_row, pre_col] == board[cur_row, cur_col]) && ((pre_col != cur_col) || (pre_row != cur_row)))
            {
                isFlipped[cur_row, cur_col] = 1;
                isFlipped[pre_row, pre_col] = 1;
                total_score += 100;
                Console.SetCursorPosition(60, 03);
                Console.WriteLine("  + 100");
                Thread.Sleep(200);
            }
            else
            {
                isFlipped[cur_row, cur_col] = 0;
                isFlipped[pre_row, pre_col] = 0;
                total_score -= 10;
                Console.SetCursorPosition(60, 03);
                Console.WriteLine("  - 10");
                Thread.Sleep(200);
            }
        }
        static void HandleKey_Match(int i, ConsoleKey key)
        {
            switch (key)
            {
                case ConsoleKey.RightArrow:
                    cur_col++;
                    if (cur_col >= gridSizes[i, 1]) cur_col = 0;
                    break;
                case ConsoleKey.LeftArrow:
                    cur_col--;
                    if (cur_col < 0) cur_col = gridSizes[i, 1] - 1;
                    break;
                case ConsoleKey.DownArrow:
                    cur_row++;
                    if (cur_row >= gridSizes[i, 0]) cur_row = 0;
                    break;
                case ConsoleKey.UpArrow:
                    cur_row--;
                    if (cur_row < 0) cur_row = gridSizes[i, 0] - 1;
                    break;
                case ConsoleKey.Enter:
                    {
                        --luot_lat_o;
                        if (isFlipped[cur_row, cur_col] == 0)
                        {
                            ++cnt_luot;
                            if (cnt_luot == 1)
                            {
                                pre_row = cur_row;
                                pre_col = cur_col;
                                isFlipped[pre_row, pre_col] = 2;
                                PrintBoard_Match(i);
                            }
                            else if (cnt_luot == 2)
                            {
                                isFlipped[cur_row, cur_col] = 2;
                                PrintBoard_Match(i);
                                Thread.Sleep(500);
                                sosanh();
                                cnt_luot = 0;
                                PrintBoard_Match(i);
                            }
                        }
                    }
                    break;
            }
            PrintBoard_Match(i);
        }

    }
    public class Game_Recall : Calculate
    {
        // Biến của game Find_word
        static int left_or_right = 0, level = 0, total_score = 0;
        static bool is_end = false;

        // Game find_word
        static string[] IntArray_S()
        {
            // Nhập mảng cho trước gồm 100 phần tử - dùng cho level 1&2
            string[] s = { "van","egg","can","meat","hello","nice","new","white","keen",
        "all","moose","shore","lime","copy","hip","loan","may","guide",
        "not","corn","sigh","eon","quack","boom", "tree", "sky", "book",
        "chair", "sun", "moon", "river", "flower", "song", "water", "earth",
        "fire", "air", "snow", "cloud", "star","apple","banana","cherry","date","grape",
        "lemon","mango","orange","peach","pear", "plum","quince","raspberry",
        "strawberry","tomato","carrot","celery","onion","pepper","potato","pen","paper",
        "beach","table","bed", "shirt","pants","shoes","hat","gloves","scarf",
        "home", "family", "work", "school", "friend", "food", "drink", "sleep", "window","roof","wall",
        "phone","computer","television","radio", "bag","box","bottle","cup","green","blue","indigo","violet",
        "square","link","ant","mine","may","pearl", "hot dog", "ice cream", "fire truck", "water bottle", "sun flower",
        "moon light", "blue sky", "green grass", "red rose", "black cat","white horse",
        "brown bear", "pink pig", "yellow duck", "gray wolf","orange juice", "apple pie",
        "cherry blossom", "strawberry cake","lemon tea", "mango smoothie", "peach cobbler",
        "pineapple pizza","grape jelly", "banana bread", "coconut milk", "pumpkin pie",
        "chocolate chip", "vanilla icecream", "caramel sauce", "peanut butter", "almond milk",
        "hazelnut coffee","cinnamon roll", "garlic bread","onion rings","pepper steak",
        "carrot cake","celery soup","potato chips", "book store","pen pal","paper plane",
        "chair lift","table cloth","bed room", "shirt collar","pants pocket","shoe lace",
        "hat trick","glove compartment","scarf knot","home town", "family tree", "work place",
        "school bus", "friend zone", "food court", "drink coaster","sleep walker","window pane",
        "roof top","wall paper","phone call","computer game","television show","radio station",
        "bag pack","box cutter","bottle cap","cup cake","key chain","clock tower","book mark","pen holder",
        "paper clip","chair leg","table top","bed sheet", "shirt sleeve","pants leg","shoe sole",
        "hat brim","glove finger","scarf end","rain coat", "snow flake", "wind mill", "fire place",
        "earth quake", "star light", "cloud nine", "sun rise", "moon beam", "ocean wave", "mountain peak",
        "river bank", "forest fire", "desert storm", "jungle gym" };
            return s;
        }
        static int[] IntArray_turn()
        {
            int[] turn = { 15, 30, 30 };
            return turn;

        }
        static int[] IntArray_range()
        {
            int[] range = { 100, 100, 200 };
            return range;
        }

        public void Run()
        {
            Huong_dan();
            Thread timerThread = new Thread(new ThreadStart(StartRecordingPlaytime));
            Thread Time = new Thread(new ThreadStart(DisplayPlaytime));
            timerThread.Start();

            for (level = 0; level <= 0; level++)
            {
                // Khai báo biến đếm số lần đúng và số lần sai
                int correct = 0;
                int wrongCount = 0;
                // Khai báo một đối tượng Random để sinh số ngẫu nhiên
                Random random = new Random();

                //Khai báo mảng chứa các chuỗi
                string[] array = new string[12];

                // Khai báo một danh sách để lưu trữ các phần tử đã xuất hiện
                List<string> appeared = new List<string>();

                //Chạy game           
                string[] s = IntArray_S();
                int[] turn = IntArray_turn();
                int[] range = IntArray_range();
                TransferToS(array, random, s, range);
                RunTime(array, random, appeared, turn[level], wrongCount, correct);
                if (is_end)
                {
                    Time.Start();
                    Thread.Sleep(1000); break;
                }
            }
            if (!is_end)
            {
                Console.SetCursorPosition(15, 18);
                Console.WriteLine("Bạn đã hoàn thành tất cả các level");
                Thread.Sleep(1000);
                return;
            }
        }

        static void Huong_dan()
        {
            Console.Clear();
            Screen_Hello hello = new Screen_Hello();
            Console.WriteLine(hello.DrawFrame(70, 25));

            Console.SetCursorPosition(20, 4);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("HƯỚNG DẪN GAME RECALL");
            string s = " Khi bắt đầu màn hình lần lượt hiện các từ, người chơi cần ghi nhớ chúng và " +
                "kiểm tra những từ này đã xuất hiện hay chưa. Mỗi lần từ xuất hiện, người chơi " +
                "sẽ nhấn các phím mũi tên tương ứng với nút Yes và No trên màn hình. Để xác" +
                " nhận là từ đã xuất hiện, người chơi sẽ nhấn phím mũi tên trái hay Yes; ngược " +
                "lại, nhấn phím mũi tên phải hay No. Hệ thống sẽ ghi nhận điểm với mỗi lần xác " +
                "định đúng đáp án, khi người chơi sai liên tiếp 3 lần trò chơi sẽ dừng lại và " +
                "quay lại màn chơi đầu";
            Function func = new Function();
            func.In_HD(s);
            func.Exception(s);
        }

        static void TransferToS(string[] array, Random random, string[] s, int[] range)
        {
            //Truyền giá trị vào mảng, i đếm số lần chuyển giá trị từ mảng s sang array 
            int i = 0;
            while (i < array.Length)
            {
                string index = s[random.Next(range[level] - 99, range[level])];
                // Nếu chuỗi này chưa có trong mảng mới, thêm nó vào mảng mới
                if (Array.IndexOf(array, index) == -1)
                {
                    array[i] = index;
                    i++;
                }

            }
        }
        static void printscreen(string element, int wrongCount, int correct)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.White;
            Screen_Hello hello = new Screen_Hello();
            int[] turn = IntArray_turn();
            Console.WriteLine(hello.DrawFrame(70, 25));
            Console.SetCursorPosition(2, 2);
            Console.WriteLine("Level {0}                                           Lượt đúng {1} / {2}", level + 1, correct, turn[level]);
            Console.SetCursorPosition(25, 7);
            Console.WriteLine("{0}", element);
            Console.SetCursorPosition(53, 03);
            Console.WriteLine("Tổng điểm: {0}", total_score);
            Console.SetCursorPosition(15, 10);
            Console.Write("Chữ này đã xuất hiện chưa? \n");
            if (left_or_right == 0)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.SetCursorPosition(18, 13);
                Console.Write("Rồi");
                Console.SetCursorPosition(33, 13);
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("Chưa");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.SetCursorPosition(18, 13);
                Console.Write("Rồi");
                Console.SetCursorPosition(33, 13);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("Chưa");
            }
            Console.ForegroundColor = ConsoleColor.White;
        }
        static void RunTime(string[] array, Random random, List<string> appeared, int n, int wrongCount, int correct)
        {
            // Cho chạy random chữ tối đa n lần
            int j = 0;
            string element = array[random.Next(0, array.Length)];
            printscreen(element, wrongCount, correct);
            //Biến kiểm tra 
            bool contains = appeared.Contains(element);
            while (true)
            {
                Console.SetCursorPosition(15, 17);
                if (j == n)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Chúc mừng bạn hoàn thành vòng chơi");
                    is_end = false;
                    Thread.Sleep(1000);
                    break;
                }
                if (wrongCount > 3)
                {
                    Console.ForegroundColor = ConsoleColor.DarkBlue;
                    Console.WriteLine("Tiếc quá, trò chơi kết thúc!");
                    Thread.Sleep(1000);
                    is_end = true;
                    break;
                }

                // Nhận phím nhấn từ người dùng
                ConsoleKey key = Console.ReadKey().Key;
                //trường hợp đúng: (left_or_right = 0 && contains = 1) hoặc (left_or_right = 1 && contains = 0) -> ++correct;
                //trường hợp sai: còn lại -> ++wrongCount;
                switch (key)
                {
                    case (ConsoleKey.LeftArrow):
                        left_or_right--;
                        if (left_or_right < 0) left_or_right = 1;
                        printscreen(element, wrongCount, correct);
                        break;
                    case (ConsoleKey.RightArrow):
                        left_or_right++;
                        if (left_or_right > 1) left_or_right = 0;
                        printscreen(element, wrongCount, correct);
                        break;
                    case (ConsoleKey.Enter):
                        Console.SetCursorPosition(22, 17);
                        if ((left_or_right == 0 && contains == true) || (left_or_right == 1 && contains == false))
                        {
                            ++correct;
                            total_score += (level + 1) * 20;
                            Console.SetCursorPosition(60, 4);
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine("  + {0}", (level+1) * 20);
                            Console.SetCursorPosition(23, 17);
                            Console.WriteLine("Chính xác");
                            Thread.Sleep(500);
                        }
                        else
                        {
                            ++wrongCount;
                            Console.ForegroundColor = ConsoleColor.Red;
                            total_score -= (level + 1) * 10;
                            Console.SetCursorPosition(60, 4);
                            Console.WriteLine("  - {0}", (level + 1) * 10);
                            Thread.Sleep(200);
                            if (total_score < 0) total_score = 0;
                            Console.SetCursorPosition(23, 17);
                            Console.WriteLine("Sai rồi");
                            Thread.Sleep(500);
                        }
                        ++j;
                        appeared.Add(element);
                        element = array[random.Next(0, array.Length)];
                        printscreen(element, wrongCount, correct);
                        contains = appeared.Contains(element);
                        break;
                }
            }
        }
    }
}