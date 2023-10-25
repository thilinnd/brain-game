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
            // Khởi tạo biến kết thúc chương trình 
            bool is_end = false;
            // Đặt giá trị của con trỏ là false để ẩn con trỏ
            Console.CursorVisible = false;
            Screen_Hello hello = new Screen_Hello();
            hello.process();
            func.Play_Music();
            // in ra chọn game
            Console.Clear();
            // Gọi hàm draw_screen lên thông qua func để hiển thị giao diện chính
            func.draw_screen(1, 0);
            ConsoleKeyInfo keyInfo;
            int row = 1, col = 0;
            while (true)
            {
                // Ghi nhận phím nhấn từ người chơi
                keyInfo = Console.ReadKey();
                // Truyền các giá trị cột, dòng, phím và biến kết thúc chương trình để thực hiện tương tác trong phạm vi giao diện trò chơi.
                func.HandleKey(keyInfo.Key, ref row, ref col, is_end);
                // Kiểm tra nếu biến kết thúc chương trình đúng thì kết thúc chương trình.
                if (is_end) { break; }
            }
        }
    }
    // Tạo lớp ghi nhận thông tin người chơi
    public class User
    {
        // Khai báo các biến bao gồm tên người chơi, điểm số và tổng thời gian chơi
        public string UserName { get; set; }
        public int Score { get; set; }
        public DateTime Date { get; set; }
    }

    // Tạo một class để lưu trữ danh sách điểm số của tất cả người chơi
    public class Scores
    {
        // Khai báo mảng danh sách chứa điểm số người chơi
        private List<User> scores = new List<User>();
        // Khai báo phương thức ghi nhận điểm số, khi phương thức này được gọi thì điểm số của người chơi sẽ được cập nhật vào mảng danh sách trên.
        public void AddUser(User score)
        {
            scores.Add(score);
        }
        // Tạo phương thức chuyền giá trị điểm số vào file điểm số
        public void Save()
        {
            // Ghi danh sách điểm số vào file
            string filename = "scores.txt";
            // Sử dụng StreamWriter để ghi từng điểm số vào một dòng của tệp
            using (StreamWriter writer = File.CreateText(filename))
            {
                // Quét qua phần tử user trong mảng danh sách scores
                foreach (User user in scores)
                {
                    // Hiện ra màn hình tên người dùng, điểm và tổng thời gian chơi.
                    writer.WriteLine($"{user.UserName},{user.Score},{user.Date:O}");
                }
                // Đóng luồng ghi sau khi đã xử lý hết tất cả các dòng trong tệp
                writer.Close();
            }
        }
        // Tạo phương thức hiện các thông tin đã được lưu trữ
        public void History()
        {
            // Tạo giá trị để gán cho hàm SetCursorPosition.
            int a = 2, b = 2;
            // Gán giá trị cho tên của file
            string filename = "scores.txt";
            int n = 20;
            // Cập nhật lần lượt 20 biến tương ứng với 20 lượt chơi với thông tin khác nhau
            if (scores.Count < 20) n = scores.Count;
            for (int i = 0; i < n; i++)
            {
                Console.SetCursorPosition(a, b);
                ++b;
                //Hiện ra thông tin người chơi lần lượt từ 1-20
                Console.WriteLine(i + 1 + ". " + scores[i].UserName + " " + scores[i].Score + " " + scores[i].Date);
            }
            ConsoleKeyInfo keyInfo;
            keyInfo = Console.ReadKey();
        }
        // Khởi tạo phương thức in ra bxh
        public void In_BXH()
        {
            string filename = "scores.txt";
            // Khởi tạo mảng chứa điểm số tên order
            int[] order = new int[scores.Count];
            // Lần lượt truyền giá trị điểm số thu nhận được vào mảng order
            for (int i = 0; i < scores.Count; i++)
                order[i] = i;
            for (int i = 0; i < scores.Count; i++)
            {
                for (int j = 0; j < i; ++j)
                    // Nếu điểm số lần sau lớn hơn lần trước thì thực hiện đổi chỗ để tao thành một dãy điểm số theo thứ tự giảm dần.
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
            // Kiểm tra biến chạy nhạc 
            if (is_playing == false)
            {
                is_playing = true;
                //Chạy nhạc nền cho game thông qua địa chỉ file có trong Solution
                player.SoundLocation = "C:\\Users\\vthuy\\source\\repos\\cai_dau_game\\cai_dau_game\\RenaiCirculation-KanaHanazawa_47xq7.wav";
                // Cho nhạc lặp lại xuyên suốt quá trình chơi
                player.PlayLooping();
            }
            else
            {
                //Nếu biến nhạc nhận giá trị false thì dừng nhạc
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
                    // Nếu như phím nhận không phải là chữ cái thì tự động bỏ qua và 
                    //Nếu phím nhấn vào không phải là Enter thì được cho là ngoại lệ 
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
            // Gọi phương thức DrawFrame lên để vẽ giao diện
            Console.WriteLine(hello.DrawFrame(70, 25));
            Console.SetCursorPosition(25, 10);
            Console.WriteLine("Mời bạn chọn game: ");
            Console.SetCursorPosition(1, 1);
            Console.ForegroundColor = ConsoleColor.White;

            // Tại ô người chơi đang tương tác, chuyển màu ô sang màu xanh 
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
            // Lần lượt điều chỉnh vị trí con trỏ và vẽ ra màn hình các khối đại diện cho các trò chơi
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
            // Tạo bảng hướng dẫn chơi
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
        // Tạo phương thức để tương tác với mỗi trò chơi
        public void Check_and_run(int row, int col)
        {
            if (row == 0)
            {
                Control_Task tmp = new Control_Task();
                tmp.Run(1, 0);
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
        // Tạo phương thức xử lý các phím nhấn từ người chơi
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
            // Nếu một trong các trường hợp xảy ra thì thực hiện vẽ bảng 
            draw_screen(row, col);
        }
    }
    struct Control_Task
    {
        // Khởi tạo biến logic turn_main để để thao tác quay lại màn hình chọn từ nút “>>”
        static bool turn_main;
        public void draw_CT(int row, int col)
        {
            // dọn màn hình, vẽ khung và hiện ô thao tác các chức năng khác khi chọn dấu “>>”
            // Vị trí của con trỏ chuột hiện tạ có màu xanh còn lại là màu trắng
            //Nút “>>” khi được thao tác sẽ đổi thành “<<”
            // Sau đó bảng thao tác các cài đặt được hiện lên
            Console.Clear();
            Screen_Hello hello = new Screen_Hello();
            Console.SetCursorPosition(0, 0);
            Console.WriteLine(hello.DrawFrame(70, 25));
            Console.SetCursorPosition(1, 1);
            Console.ForegroundColor = ConsoleColor.White;
            if (row == 0) Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("<< ");
            // In ra bảng chọn “Loa”
            Console.ForegroundColor = ConsoleColor.White;
            if (row == 1 && col == 0) Console.ForegroundColor = ConsoleColor.Green;
            Console.SetCursorPosition(15, 12);
            Console.Write("__________");
            Console.SetCursorPosition(15, 13);
            Console.Write("|   Loa  |");
            Console.SetCursorPosition(15, 14);
            Console.Write("|________|");
            // In ra bảng chọn “Lịch sử”

            Console.ForegroundColor = ConsoleColor.White;
            if (row == 1 && col == 1) Console.ForegroundColor = ConsoleColor.Green;
            Console.SetCursorPosition(30, 12);
            Console.Write("__________");
            Console.SetCursorPosition(30, 13);
            Console.Write("|  Lịch  |");
            Console.SetCursorPosition(30, 14);
            Console.Write("|___sử___|");
            // In ra bảng chọn “Thoát game”
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
            // thực hiện thao tác trong mục cài đặt
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
        // Chọn các tác vụ trong mục cài đặt
        static void Check_CT(int row, int col)
        {
            if (row == 0)
            {
                turn_main = true;
                return;
            }
            else
            {
                // Chọn vào loa sẽ tắt/ mở nhạc
                if (col == 0)
                {
                    Function func = new Function();
                    func.Play_Music();
                }
                // Khi chọn vào Lịch sử sẽ hiện ra lịch sử chơi (điểm)
                if (col == 1)
                {
                    Console.Clear();
                    Screen_Hello hello = new Screen_Hello();
                    Console.WriteLine(hello.DrawFrame(70, 25));
                    Scores htr = new Scores();
                    htr.Load();
                    htr.History();
                }
                // Khi chọn vào thoát game chương trình chào tạm biệt người chơi rồi thoát ra ngoài
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
            /* Nếu con trỏ chuột ở một vị trí biên nếu tiếp tục di chuyển ra biên sẽ quay trở lại biên còn lại * (theo hàng dọc và ngang */
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
    //Đoạn code dưới đây định nghĩa một cấu trúc (struct) tên là Screen_Hello. Cấu trúc này chứa các hằng số và phương thức để vẽ khung chữ nhật trên màn hình 
    struct Screen_Hello
    {
        // Các mã hằng số được sử dụng để đại diện bộ xử lý đầu ra chuẩn của console
        const int STD_OUTPUT_HANDLE = -11;
        const uint ENABLE_VIRTUAL_TERMINAL_PROCESSING = 4;
        //Là mã hằng số được sử dụng để kích hoạt chế độ sử lý ảo cho terminal console. Khi  chế độ này được kích hoạt, console có thể hiển thị các màu sắc và các ký tự đặc biệt khác
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
            // Dưới đây là các mã được dùng để định dạng chữ trên console
            // Gạch chân
            // In đậm
            // Xóa định dạng
            const string UNDERLINE = "\x1B[4m";
            const string BOLD = "\x1B[1m";
            const string RESET = "\x1B[0m";
            int n = 21;
            // Vẽ ra thanh loading để tải game
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
        // các biến được khai báo dưới đây nhằm để tính thời gian chơi của người chơi
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
        // Ghi ra thời gian mà người chơi đã chơi là bao nhiêu phút và bao nhiêu giây
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
        // Khai báo các biển được sử dụng
        // biến row và col lần lượt được sử dụng để tạo mảng 2 chiều và tạo bảng chơi
        static int[] row = new int[10] { 2, 3, 3, 3, 3, 4, 4, 5, 5, 5 };
        static int[] col = new int[10] { 2, 3, 3, 4, 4, 4, 4, 5, 5, 5 };
        static int[] ran = new int[10] { 2, 3, 5, 4, 6, 5, 8, 6, 8, 10 };
        static int[] score = new int[10] { 100, 150, 200, 250, 300, 350, 400, 450, 500, 550 };
        static int[,] board = new int[10, 10];
        static int[,] visual = new int[10, 10];
        static int luot_dung, luot_sai, cur_row, cur_col, is_end_game, total_score = 0, level = 0;
        public void Run()
        {
            // In ra hướng dẫn chơi và ghi thời gian ngay khi người chơi chọn bắt đầu
            Huong_dan();
            Thread timerThread = new Thread(new ThreadStart(StartRecordingPlaytime));
            Thread Time = new Thread(new ThreadStart(DisplayPlaytime));
            timerThread.Start();
            // Memory Matrix sẽ có 9 level
            // Xóa màn hình và in ra giao diện hướng dẫn, bảng chơi và level hiện tại của người chơi
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
                // Khi người chơi chọn sai hơn 3 lần hoặc số lần chọn đúng trùng với giá trị của biến ran[level], thì vòng lặp sẽ bị ngắt.
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
                // Nếu thua 3 màn chơi thì trò chơi sẽ kết thúc, kết quả sẽ được lưu và đưa ra màn hình
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
        // Sau khi trò chơi kết thúc người chơi sẽ để lại nội thông tin để lưu lại kết quả
        static void save_result()
        {
            Console.SetCursorPosition(25, 16);
            Console.WriteLine("Nhập tên đi: ");
            User newUser = new User();
            // Khai báo biến và cập nhật
            newUser.UserName = Console.ReadLine();
            newUser.Date = DateTime.Now;
            newUser.Score = total_score;
            // Đọc điểm từ file
            Scores scoreList = new Scores();
            scoreList.Load();
            // Thêm điểm của người chơi mới và lưu lại
            scoreList.AddUser(newUser);
            scoreList.Save();
            // In ra bảng xếp hạng
            Console.Clear();
            Screen_Hello hello = new Screen_Hello();
            Console.WriteLine(hello.DrawFrame(70, 25));
            Scores bxh = new Scores();
            bxh.Load();
            bxh.In_BXH();
        }
        // Phương thức này được gọi ra khi cần in ra hướng dẫn chơi
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
        // Thực hiện di chuyển trỏ chuột theo thao tác của bàn phím
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
                        // Nếu người chơi chọn đúng thì sẽ được tăng 100 điểm
                        // Được chúc mừng : “Đúng rồi” 
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
        // Xóa màn hình và in ra bảng đáp án
        /* các biến a, b được gán các giá trị như bên dưới nhằm để đưa bảng vào chính giữa khung */
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
        // Khi hoàn thành trò chơi in ra tổng điểm và level
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
                    // Các ô cho người chơi thao tác là “-”
                    // Nếu chọn sai ô hiện dấu “x”
                    // Đúng hiện “o”
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
    public class Matching_Game : Calculate
    {
        //Khai báo biến của game Matching là những emoji
        // Các ký tự nằm dưới mỗi ô trong bảng 
        static List<string> symbols = new List<string> { "🍀", "🐻", "🍄", "🍞", "🍑", "🍭", "🎁", "⛅", "🦀" };
        /*Kích thước của các bảng trong trò chơi. 
  Mỗi cặp giá trị là một kích thước bảng, gồm số hàng và số cột tương ứng.*/
        static int[,] gridSizes = { { 2, 2 }, { 2, 3 }, { 3, 4 }, { 3, 4 }, { 4, 4 }, { 4, 4 }, { 4, 5 }, { 4, 5 }, { 6, 6 }, { 6, 6 } };
        /* Dùng mảng 2 chiều isFlipped có kích thước 10x10 để lưu trữ trạng thái của các ô đã được lật trong trò chơi.
 Và mảng 2 chiều board có kích thước 10x10, dùng để lưu trữ trạng thái của các ô trên bảng.*/
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
            // Tạo Vòng lặp while (level < 10) chạy cho đến khi level đạt giá trị 10, tức là đã hoàn thành 10 cấp độ của trò chơi.
            while (level < 10)
            {
                Console.Clear();
                // Vẽ khung chào mừng trên màn hình 
                Screen_Hello hello = new Screen_Hello();
                Console.WriteLine(hello.DrawFrame(70, 25));
                Console.SetCursorPosition(30, 10);
                Console.BackgroundColor = ConsoleColor.DarkBlue;
                // In ra thông báo về cấp độ hiện tại 
                Console.Write("Level {0}", level + 1);
                Thread.Sleep(500);
                Console.BackgroundColor = ConsoleColor.Black;
                ConsoleKeyInfo keyInfo;
                // khởi tạo trò chơi với mức độ level đã cho
                init_Match(level);
                // tạo một bảng chơi mới cho trò chơi dựa theo biến level
                NewBoard_Match(level);
                Thread.Sleep(2000);
                // Gán giá trị cho biến luot_lat_o, biểu thị số lượt lật ô trong trò chơi. Giá trị được tính bằng cách nhân kích thước bảng chơi (gridSizes) với 3.
                luot_lat_o = gridSizes[level, 0] * gridSizes[level, 1] * 3;
                PrintBoard_Match(level);
                // Vòng lặp while sẽ tiếp tục chạy cho đến khi không còn lượt lật ô nào.
                while (luot_lat_o > 0)
                {
                    keyInfo = Console.ReadKey();
                    // Gọi hàm HandleKey_Match với tham số level và phím được nhấn để xử lý hành động của người chơi
                    HandleKey_Match(level, keyInfo.Key);
                    //Kiểm tra xem trò chơi đã kết thúc chưa bằng cách gọi hàm Check_end(). Nếu đã kết thúc, thoát khỏi vòng lặp while.
                    if (Check_end() == true) break;
                }
                // Kiểm tra xem trò chơi đã kết thúc chưa. Nếu đã kết thúc, tăng mức độ level lên 1
                if (Check_end() == true) ++level;
                // Nếu trò chơi chưa kết thúc, tăng biến is_end_game lên 1
                else
                    ++is_end_game;
                // Kiểm tra xem đã đạt được số lần kết thúc game là 3 hay chưa
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
        // Hướng dẫn cho Game Matching
        static void Huong_dan()
        {
            Console.Clear();
            Screen_Hello hello = new Screen_Hello();
            Console.WriteLine(hello.DrawFrame(70, 25));

            Console.SetCursorPosition(20, 4);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("HƯỚNG DẪN MATCHING GAME");
            //  Tạo biến s lưu trữ nội dung hướng dẫn chi tiết của trò chơi
            string s = " Khi bắt đầu màn hình sẽ hiện một bảng với các phần tử ẩn. Người chơi sẽ lần lượt lật các phần tử lên bằng " +
                "cách điều khiển các phím mũi tên, bấm Enter để lật hình. Mỗi lần lật, các phần tử sẽ xuất hiện với 1 ký hiệu và yêu " +
                "cầu người chơi ghi nhớ. (Lật tối đa hai lần 1 lượt) Nếu ký hiệu ở 2 lần trùng khớp thì chúng sẽ được lật lên, " +
                "khóa lại. Người chơi sẽ chiến thắng nếu lật được tất cả các cặp lá giống nhau trong thời gian quy định, màn chơi " +
                "sẽ kết thúc ngay tại thời điểm cặp ký hiệu cuối cùng được lật lên";
            // In ra nội dung hướng dẫn game
            Function func = new Function();
            func.In_HD(s);
            func.Exception(s);
        }
        // Kiểm tra xem trò chơi Matching Game đã kết thúc hay chưa
        static bool Check_end()
        {
            /* Duyệt qua tất cả các phần tử trong bảng isFlipped. 
            Nếu có bất kỳ phần tử nào chưa được lật (có giá trị khác 1) thì phương thức trả về giá trị false, tức là trò chơi chưa kết thúc và ngược lại */
            for (int j = 0; j < gridSizes[level, 0]; j++)
                for (int k = 0; k < gridSizes[level, 1]; k++)
                    if (isFlipped[j, k] != 1) return false;
            return true;
        }
        // Tạo bảng trò chơi mới với kích thước tương ứng với cấp độ i và đánh dấu trạng thái của các phần tử trong bảng.
        static void init_Match(int i)
        // khởi tạo bảng
        {
            //  Duyệt qua tất cả các hàng của bảng trò chơi. Điều kiện dừng của vòng lặp là j < gridSizes[i, 0], tức là chỉ duyệt qua các hàng từ 0 đến gridSizes[i, 0] - 1.
            for (int j = 0; j < gridSizes[i, 0]; j++)
                for (int k = 0; k < gridSizes[i, 1]; k++)
                {
                    board[j, k] = -1;
                    //  0 được sử dụng để đánh dấu rằng phần tử này chưa được lật.
                    isFlipped[j, k] = 0;
                }
        }

        static void NewBoard_Match(int i)
        {
            //  Khởi tạo biến half, tính toán giá trị để biết số lượng ký tự cần được đặt vào bảng
            int half = gridSizes[i, 0] * gridSizes[i, 1] / 2;
            //  Đếm số lượng ký tự đã được đặt vào bảng
            int count = -1;
            //  lưu các ký tự trong bảng
            int cnt_symbol = -1;
            int[] tmp_arr = new int[half];
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
                    // Trong mỗi vòng lặp, kiểm tra nếu giá trị tại vị trí (j, k) trên bảng board 
                    // là -1(tức là vị trí đó chưa được điền giá trị),  thì thực hiện các bước trong khối lệnh 
                    // giá trị từ mảng tmp_arr được điền vào bảng board theo đúng thứ tự và vị trí 
                    // tương ứng. Sau đó giá trị của biến count giảm đi 1 để trỏ tới phần tử tiếp 
                    // theo của mảng tmp_arr -count
                    if (board[j, k] == -1)
                    {
                        board[j, k] = tmp_arr[count];
                        --count;
                    }
        }

        static void PrintBoard_Match(int i)
        // In ra màn hình bảng trò chơi.
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.White;
            Screen_Hello hello = new Screen_Hello();
            Console.WriteLine(hello.DrawFrame(70, 25));
            Console.SetCursorPosition(02, 02);
            // Hiển thị số lượt lật ô còn lại.
            Console.WriteLine("Lượt lật ô còn lại: {0}", luot_lat_o);
            Console.SetCursorPosition(53, 02);
            // Hiển thị tổng số điểm.
            Console.WriteLine("Tổng điểm: {0}", total_score);
            // Tính toán vị trí bắt đầu để vẽ lưới trò chơi.
            int a = 35 - gridSizes[i, 0], b = 12 - gridSizes[i, 1] / 2;
            Console.SetCursorPosition(a, b);
            for (int j = 0; j < gridSizes[i, 0]; j++)
            {
                Console.SetCursorPosition(a, b);
                for (int k = 0; k < gridSizes[i, 1]; k++)
                {
                    // Kiểm tra xem ô hiện tại có phải là ô đang được chọn hay không.
                    if (j == cur_row && k == cur_col)
                        Console.BackgroundColor = ConsoleColor.Blue;
                    else Console.BackgroundColor = ConsoleColor.Black;
                    // Nếu ô chưa được lật, hiển thị dấu hỏi chấm.
                    if (isFlipped[j, k] == 0) Console.Write("❓");
                    else
                        if (isFlipped[j, k] == 1) Console.Write("➖");
                    // Nếu ô đã được lật và khớp với ô khác, hiển thị ký tự tương ứng.
                    else Console.Write(symbols[board[j, k]]);
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.Write(" ");
                    // Tăng giá trị của biến b để đặt con trỏ văn bản tại hàng tiếp theo của lưới trò chơi
                }
                Console.WriteLine();
                ++b;
            }
        }
        // So sánh giá trị của hai ô trong bảng trò chơi
        static void sosanh()
        {
            PrintBoard_Match(level);
            /* Kiểm tra nếu giá trị của ô trước và giá trị của ô hiện tại giống nhau và hai ô không trùng vị trí thì đặt giá trị của chúng thành 1. 
       Điều này đồng nghĩa với việc hai ô đã được tìm thấy và lật mở. 
       Trong trường hợp không thỏa mãn điều kiện trên, đặt giá trị của chúng thành 0. 
       Điều này đồng nghĩa với việc hai ô không giống nhau hoặc hai ô trùng vị trí. */
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
        // Xử lý các sự kiện khi người dùng nhấn các phím mũi tên và phím Enter trên bàn phím
        static void HandleKey_Match(int i, ConsoleKey key)
        {
            // Kiểm tra giá trị của phím được nhấn
            switch (key)
            {
                /* Người chơi nhấn phím mũi tên sang phải, trái, xuống hoặc lên, thì con trỏ hiện tại (cur_row và cur_col) sẽ di chuyển tương ứng. 
                        Nếu con trỏ đã ở vị trí đầu tiên của hàng, cột thì khi dùng mũi tên lùi lại vị trí trước, thì nó sẽ đi đến vị trí cuối hàng hoặc cuối của cột
                        Nếu con trỏ đã ở vị trí cuối cùng của hàng hoặc cột, thì nó sẽ quay lại vị trí đầu tiên của hàng hoặc cột tương ứng. */
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
                        // Kiểm tra ô hiện tại chưa được lật mở chưa
                        if (isFlipped[cur_row, cur_col] == 0)
                        {
                            ++cnt_luot;
                            if (cnt_luot == 1)
                            {
                                pre_row = cur_row;
                                pre_col = cur_col;
                                // Đánh dấu ô đã được chọn
                                isFlipped[pre_row, pre_col] = 2;
                                PrintBoard_Match(i);
                            }
                            else if (cnt_luot == 2)
                            {
                                isFlipped[cur_row, cur_col] = 2;
                                PrintBoard_Match(i);
                                Thread.Sleep(500);
                                // So sánh giá trị của hai ô đã chọn và đặt cnt_luot về 0 để chuẩn bị cho lượt chơi tiếp theo.
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
        // Biến này được sử dụng để kiểm tra xem chương trình đã kết thúc hay chưa
        static bool is_end = false;
        // Game find_word
        static string[] IntArray_S()
        {
            // Khai báo mảng s gồm 100 phần tử - dùng cho level 1 & 2
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
        // Khai báo mảng chứa số lượt chơi lần lượt cho từng level
        static int[] IntArray_turn()
        {
            int[] turn = { 15, 30, 30 };
            return turn;
        }
        // Khai báo mảng chứa số từ được dùng lần lượt cho từng level
        static int[] IntArray_range()
        {
            int[] range = { 100, 100, 200 };
            return range;
        }

        public void Run()
        {
            // Đưa ra Hướng dẫn chơi
            Huong_dan();
            // Khởi tạo luồng Thread với hàm StartRecordingPlaytime để tính thời gian chơi và 
            // hàm DisplayPlaytime để hiện thời gian chơi
            Thread timerThread = new Thread(new ThreadStart(StartRecordingPlaytime));
            Thread Time = new Thread(new ThreadStart(DisplayPlaytime));
            // Bắt đầu luồng
            timerThread.Start();

            for (level = 0; level <= 0; level++)
            {
                // Khai báo biến đếm số lần đúng và số lần sai
                int correct = 0;
                int wrongCount = 0;
                // Khai báo một đối tượng Random để sinh số ngẫu nhiên
                Random random = new Random();
                // Khai báo mảng chứa các chuỗi
                string[] array = new string[12];
                // Khai báo một danh sách để lưu trữ các phần tử đã xuất hiện
                List<string> appeared = new List<string>();
                // Chạy game           
                string[] s = IntArray_S();
                int[] turn = IntArray_turn();
                int[] range = IntArray_range();
                /* Chuyển các chuỗi theo thứ tự ngẫu nhiên từ mảng s sang mảng array theo trình tự 
                 mà range cho phép */
                TransferToS(array, random, s, range);
                // Chuyển các chuỗi vào hàm Run theo thứ tự ngẫu nhiên, mỗi khi chạy đến một level
                // mới bằng cách tăng i trong vòng lặp for thì runtime lại đc gọi lên, trong runtime lại
                // có vòng lặp chạy cho cho đến khi đủ số lượng phần tử là n thì nó mới cho i tăng lên
                RunTime(array, random, appeared, turn[level], wrongCount, correct);
                // Khi is_end được đặt thành là false, vòng lặp trong chương trình sẽ tiếp tục và
                // chương trình sẽ bắt đầu
                if (is_end)
                {
                    // Thread hiển thị thời gian chạy và dừng 1 giây trước khi trò chơi kết thúc
                    Time.Start();
                    Thread.Sleep(1000); break;
                }
            }
            // Khi is_end khác false, vòng lặp trong chương trình sẽ dừng lại và chương trình sẽ
            // kết thúc
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
            // Xóa màn hình
            Console.Clear();
            // Vẽ và in ra khung chào mừng với kích thước 70x25
            Screen_Hello hello = new Screen_Hello();
            Console.WriteLine(hello.DrawFrame(70, 25));
            // Đặt con trở ở vị trí dòng 20, cột 4, in ra dòng chữ “HƯỚNG DẪN GAME RECALL”
            // có màu đỏ
            Console.SetCursorPosition(20, 4);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("HƯỚNG DẪN GAME RECALL");
            // Tạo biến s lưu trữ nội dung hướng dẫn chi tiết của trò chơi
            string s = " Khi bắt đầu màn hình lần lượt hiện các từ, người chơi cần ghi nhớ chúng và " +
                "kiểm tra những từ này đã xuất hiện hay chưa. Mỗi lần từ xuất hiện, người chơi " +
                "sẽ nhấn các phím mũi tên tương ứng với nút Yes và No trên màn hình. Để xác" +
                " nhận là từ đã xuất hiện, người chơi sẽ nhấn phím mũi tên trái hay Yes; ngược " +
                "lại, nhấn phím mũi tên phải hay No. Hệ thống sẽ ghi nhận điểm với mỗi lần xác " +
                "định đúng đáp án, khi người chơi sai liên tiếp 3 lần trò chơi sẽ dừng lại và " +
                "quay lại màn chơi đầu";
            // In ra nội dung hướng dẫn game
            Function func = new Function();
            func.In_HD(s);
            func.Exception(s);
        }

        static void TransferToS(string[] array, Random random, string[] s, int[] range)
        {
            // Truyền giá trị vào mảng, i đếm số lần chuyển giá trị từ mảng s sang array 
            int i = 0;
            while (i < array.Length)
            {
                // Chọn ngẫu nhiên chuỗi Index thuộc mảng s nằm trong giới hạn Random của range
                // được xác định ở mỗi level tương ứng
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
            // Xóa màn hình
            Console.Clear();
            // In ra màn hình chữ màu trắng
            Console.ForegroundColor = ConsoleColor.White;
            // Vẽ và in ra khung chào mừng với kích thước 70x25
            Screen_Hello hello = new Screen_Hello();
            // Gọi số lượt chơi tương ứng với từng level
            int[] turn = IntArray_turn();
            Console.WriteLine(hello.DrawFrame(70, 25));
            Console.SetCursorPosition(2, 2);
            // Hiển thị Level hiện tại, số lượt đúng/số lượt đã chơi. Nếu trả lời đúng thì tiếp tục
            // chơi cho đến khi xong lượt của level hiện tại rồi chuyển qua level mới
            Console.WriteLine("Level {0}                                           Lượt đúng {1} / {2}", level + 1, correct, turn[level]);
            Console.SetCursorPosition(25, 7);
            Console.WriteLine("{0}", element);
            Console.SetCursorPosition(53, 03);
            // Hiển thị tổng điểm được tính bằng total_score
            Console.WriteLine("Tổng điểm: {0}", total_score);
            Console.SetCursorPosition(15, 10);
            Console.Write("Chữ này đã xuất hiện chưa? \n");
            /* Phương thức sẽ kiểm tra left_or_right và chạy các tác vụ tương ứng với giá trị của
            left_or_right */
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
            // Biến kiểm tra trong mảng appeared chứa chuỗi element hay chưa
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
                // Nếu sai quá 3 lần thì game kết thúc
                if (wrongCount > 3)
                {
                    Console.ForegroundColor = ConsoleColor.DarkBlue;
                    Console.WriteLine("Tiếc quá, trò chơi kết thúc!");
                    Thread.Sleep(1000);
                    is_end = true;
                    break;
                }
                //  Nhận phím nhấn từ người dùng
                ConsoleKey key = Console.ReadKey().Key;
                /* Trường hợp đúng: (left_or_right = 0 && contains = 1) hoặc (left_or_right = 1 &&
     contains = 0) -> ++correct; Trường hợp sai: còn lại -> ++wrongCount; */
                switch (key)
                {
                    /* Nếu là phím mũi tên sang trái (ConsoleKey.LeftArrow), biến left_or_right sẽ    giảm 1 đơn vị. Nếu giá trị của left_or_right < 0, nghĩa là đã đi qua cột đầu tiên, left_or_right sẽ được đặt lại về 1 */
                    case (ConsoleKey.LeftArrow):
                        left_or_right--;
                        if (left_or_right < 0) left_or_right = 1;
                        printscreen(element, wrongCount, correct);
                        break;
                    /* Nếu là phím mũi tên sang phải (ConsoleKey.RightArrow), biến left_or_right sẽ tăng
             1 đơn vị. Nếu giá trị của left_or_right > 1, nghĩa là đã đi qua cột cuối cùng,  left_or_right sẽ được đặt lại về 0 */
                    case (ConsoleKey.RightArrow):
                        left_or_right++;
                        if (left_or_right > 1) left_or_right = 0;
                        printscreen(element, wrongCount, correct);
                        break;
                    /* Nếu là phím Enter (ConsoleKey.Enter), phương thức sẽ kiểm tra left_or_right và contains, và chạy các tác vụ tương ứng với giá trị của left_or_right và contains */
                    case (ConsoleKey.Enter):
                        Console.SetCursorPosition(22, 17);
                        if ((left_or_right == 0 && contains == true) || (left_or_right == 1 && contains == false))
                        {
                            // Số lần đúng tăng thêm 1 đơn vị
                            ++correct;
                            // Hiển thị tổng điểm hiện có mỗi khi trả lời xong 1 câu
                            total_score += (level + 1) * 20;
                            Console.SetCursorPosition(60, 4);
                            Console.ForegroundColor = ConsoleColor.Green;
                            // Hiển thị số điểm được cộng thêm nếu trả lời đúng
                            Console.WriteLine("  + {0}", (level + 1) * 20);
                            Console.SetCursorPosition(23, 17);
                            Console.WriteLine("Chính xác");
                            Thread.Sleep(500);
                        }
                        else
                        {
                            // Số lần sai tăng thêm 1
                            ++wrongCount;
                            Console.ForegroundColor = ConsoleColor.Red;
                            total_score -= (level + 1) * 10;
                            Console.SetCursorPosition(60, 4);
                            // Hiển thị số điểm bị trừ đi nếu trả lời sai
                            Console.WriteLine("  - {0}", (level + 1) * 10);
                            Thread.Sleep(200);
                            // Nếu total_score < 0 thì đặt lại về 0
                            if (total_score < 0) total_score = 0;
                            Console.SetCursorPosition(23, 17);
                            Console.WriteLine("Sai rồi");
                            Thread.Sleep(500);
                        }
                        // Số câu đúng tăng thêm 1
                        ++j;
                        // Thêm ký tự vào mảng đã xuất hiện
                        appeared.Add(element);
                        element = array[random.Next(0, array.Length)];
                        // Random ký tự trong mảng
                        printscreen(element, wrongCount, correct);
                        contains = appeared.Contains(element);
                        break;
                }
            }
        }
    }
}
