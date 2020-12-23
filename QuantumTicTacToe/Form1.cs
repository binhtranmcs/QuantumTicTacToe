using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QuantumTicTacToe
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        List<int>[] adjacent = new List<int>[9];
        public int turn = 1; // đếm lượt
        public int player = 1; // lẻ là X, chẵn là O
        public bool cyclicCreated = false; // kiểm tra có loop hay không
        public int previousButton = -1; // button trước đó
        public int[] DSU = { 0, 1, 2, 3, 4, 5, 6, 7, 8 }; // mảng để chia các đỉnh vào các thành phần liên thông, ban đầu mỗi đỉnh là 1 thành phần liên thông
        public int[] isInLoop = { -100, -100, -100, -100, -100, -100, -100, -100, -100 }; // kiểm tra xem button ở trong loop nào
        public int loop;
        string[,] edge; // phần tử [i,j] lưu lại giá trị cạnh nối i với j
        List<string>[] loopEdge = new List<string>[9];
        Button pressedButton;
        public bool[] Done = { false, false, false, false, false, false, false, false, false }; // đánh dấu button đã có trạng thái cố định
        public string[] nameList = { "", "", "", "", "", "", "", "", "" };
        public bool isSpecial = false; // xét riêng cho trường hợp loop tạo thành bởi 2 cạnh cùng nối 2 đỉnh
        Button eBtn0, eBtn1;
        public string eEdge0, eEdge1;
        public int xPoint = 0, oPoint = 0; // điểm của mỗi người chơi
        public bool gameEnd = false;


        public void DFS(int s, int pa, int e) // kiểm tra các điểm ở trong entanglement loop và các điểm cung thành phần liên thông
        {
            foreach (int i in adjacent[s])
            {
                if (i != pa)
                {
                    isInLoop[i] = 10 + e;
                    if (i == e)
                    {
                        isInLoop[i] = e;
                    }
                    DFS(i, s, e);
                    if (isInLoop[i] == e)
                    {
                        loopEdge[s].Add(edge[i, s]);
                        loopEdge[i].Add(edge[i, s]);
                        isInLoop[s] = e;
                    }
                }
            }
        }

        public void specialDFS(int s, int pa, int e)
        {
            foreach (int i in adjacent[s]) 
            {
                if (i != pa && i != getId(eBtn0) && i != getId(eBtn1))
                {
                    isInLoop[i] = e;
                    DFS(i, s, e);
                    if (isInLoop[i] == e)
                    {
                        loopEdge[s].Add(edge[i, s]);
                        loopEdge[i].Add(edge[i, s]);
                        isInLoop[s] = e;
                    }
                }
            }
        }

        Button getButton(int id) // trả về button ứng với chỉ số
        {
            if (id == 0) return btn1;
            if (id == 1) return btn2;
            if (id == 2) return btn3;
            if (id == 3) return btn4;
            if (id == 4) return btn5;
            if (id == 5) return btn6;
            if (id == 6) return btn7;
            if (id == 7) return btn8;
            if (id == 8) return btn9;
            else return null;
        }

        int getId(Button b) // trả về chỉ số của button
        {
            int ID = b.Name[b.Name.Length - 1] - '1';
            return ID;
        }

        public void loopCollapse(int id, string Avoid) // xác định trạng thái cuối cùng của các button ở trong loop
        {
            Done[id] = true;
            foreach (int i in adjacent[id])
            {
                if (Done[i] == false && edge[i, id] != Avoid)
                {
                    Done[i] = true;
                    getButton(i).Font = new Font("Microsoft Sans Serif", 30);
                    getButton(i).Text = edge[i, id];
                    //MessageBox.Show(i.ToString());
                    loopCollapse(i, edge[i, id]);
                } 
            }
        }

        public void loopHandle(int currentButton, int previousButton, string Value)
        {
            DFS(currentButton, -1, previousButton);
            //MessageBox.Show(Value);
            loopEdge[currentButton].Add(Value);
            loopEdge[previousButton].Add(Value);
            for (int i = 0; i < 9; i++)
            {
                if (isInLoop[i] != previousButton) getButton(i).Enabled = false;
            }
            loop = previousButton;
        }

        public void specialLoopCollapse(int id, string Avoid) // xác định trạng thái cuối cùng của các button ở trong loop
        {
            Done[id] = true;
            foreach (int i in adjacent[id])
            {
                if (i != getId(eBtn0) && i != getId(eBtn1) && Done[i] == false && edge[i, id] != Avoid)
                {
                    Done[i] = true;
                    getButton(i).Font = new Font("Microsoft Sans Serif", 30);
                    getButton(i).Text = edge[i, id];
                    //MessageBox.Show(i.ToString());
                    specialLoopCollapse(i, edge[i, id]);
                }
            }
        }

        public void checkWinner()
        {
            if (Done[0] && Done[1] && Done[2])
            {
                if (btn1.Text[0] == btn2.Text[0] && btn2.Text[0] == btn3.Text[0])
                {
                    if (btn1.Text[0] == 'X') xPoint += Math.Min(Math.Min(btn1.Text[1] - '0', btn2.Text[1] - '0'), btn3.Text[1] - '0');
                    if (btn1.Text[0] == 'O') oPoint += Math.Min(Math.Min(btn1.Text[1] - '0', btn2.Text[1] - '0'), btn3.Text[1] - '0');
                }
            }
            if (Done[3] && Done[4] && Done[5])
            {
                if (btn4.Text[0] == btn5.Text[0] && btn5.Text[0] == btn6.Text[0])
                {
                    if (btn4.Text[0] == 'X') xPoint += Math.Min(Math.Min(btn4.Text[1] - '0', btn5.Text[1] - '0'), btn6.Text[1] - '0');
                    if (btn4.Text[0] == 'O') oPoint += Math.Min(Math.Min(btn4.Text[1] - '0', btn5.Text[1] - '0'), btn6.Text[1] - '0');
                }
            }
            if (Done[6] && Done[7] && Done[8])
            {
                if (btn7.Text[0] == btn8.Text[0] && btn8.Text[0] == btn9.Text[0])
                {
                    if (btn7.Text[0] == 'X') xPoint += Math.Min(Math.Min(btn7.Text[1] - '0', btn8.Text[1] - '0'), btn9.Text[1] - '0');
                    if (btn7.Text[0] == 'O') oPoint += Math.Min(Math.Min(btn7.Text[1] - '0', btn8.Text[1] - '0'), btn9.Text[1] - '0');
                }
            }
            if (Done[0] && Done[3] && Done[6])
            {
                if (btn1.Text[0] == btn4.Text[0] && btn4.Text[0] == btn7.Text[0])
                {
                    if (btn1.Text[0] == 'X') xPoint += Math.Min(Math.Min(btn1.Text[1] - '0', btn4.Text[1] - '0'), btn7.Text[1] - '0');
                    if (btn1.Text[0] == 'O') oPoint += Math.Min(Math.Min(btn1.Text[1] - '0', btn4.Text[1] - '0'), btn7.Text[1] - '0');
                }
            }
            if (Done[1] && Done[4] && Done[7])
            {
                if (btn2.Text[0] == btn5.Text[0] && btn5.Text[0] == btn8.Text[0])
                {
                    if (btn2.Text[0] == 'X') xPoint += Math.Min(Math.Min(btn2.Text[1] - '0', btn5.Text[1] - '0'), btn8.Text[1] - '0');
                    if (btn2.Text[0] == 'O') oPoint += Math.Min(Math.Min(btn2.Text[1] - '0', btn5.Text[1] - '0'), btn8.Text[1] - '0');
                }
            }
            if (Done[2] && Done[5] && Done[8])
            {
                if (btn3.Text[0] == btn6.Text[0] && btn6.Text[0] == btn9.Text[0])
                {
                    if (btn3.Text[0] == 'X') xPoint += Math.Min(Math.Min(btn3.Text[1] - '0', btn6.Text[1] - '0'), btn9.Text[1] - '0');
                    if (btn3.Text[0] == 'O') oPoint += Math.Min(Math.Min(btn3.Text[1] - '0', btn6.Text[1] - '0'), btn9.Text[1] - '0');
                }
            }
            if (Done[0] && Done[4] && Done[8])
            {
                if (btn1.Text[0] == btn5.Text[0] && btn5.Text[0] == btn9.Text[0])
                {
                    if (btn1.Text[0] == 'X') xPoint += Math.Min(Math.Min(btn1.Text[1] - '0', btn5.Text[1] - '0'), btn9.Text[1] - '0');
                    if (btn1.Text[0] == 'O') oPoint += Math.Min(Math.Min(btn1.Text[1] - '0', btn5.Text[1] - '0'), btn9.Text[1] - '0');
                }
            }
            if (Done[2] && Done[4] && Done[6])
            {
                if (btn3.Text[0] == btn5.Text[0] && btn5.Text[0] == btn7.Text[0])
                {
                    if (btn3.Text[0] == 'X') xPoint += Math.Min(Math.Min(btn3.Text[1] - '0', btn5.Text[1] - '0'), btn7.Text[1] - '0');
                    if (btn3.Text[0] == 'O') oPoint += Math.Min(Math.Min(btn3.Text[1] - '0', btn5.Text[1] - '0'), btn7.Text[1] - '0');
                }
            }
            if (xPoint * oPoint == 0)
            {
                if (xPoint != 0)
                {
                    gameEnd = true;
                    MessageBox.Show("X wins!");
                    for (int i = 0; i < 9; i++)
                    {
                        getButton(i).Enabled = false;
                    }
                }
                else if (oPoint != 0)
                {
                    gameEnd = true;
                    MessageBox.Show("O wins!");
                    for (int i = 0; i < 9; i++)
                    {
                        getButton(i).Enabled = false;
                    }
                }
            }
            else
            {
                gameEnd = true;
                if (xPoint < oPoint) MessageBox.Show("X wins!");
                else MessageBox.Show("O wins!");
                for (int i = 0; i < 9; i++)
                {
                    getButton(i).Enabled = false;
                }
            }
            int countDone = 0;
            for (int i = 0; i < 9; i++)
            {
                if (Done[i]) countDone++;
            }
            if  (countDone >= 8 && !gameEnd)
            {
                gameEnd = true;
                MessageBox.Show("Draw!");
                for (int i = 0; i < 9; i++)
                {
                    getButton(i).Enabled = false;
                }
            }
        }

        private void btn_Click(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            if (Done[getId(button)])
            {
                InsLabel.Text = "No more move for this square!";
                return;
            }

            /* lượt này sẽ chọn trạng thái để collapse các button có trong loop 
             * xong lượt này, player không thay đổi, người chơi sẽ tiếp tục chọn như bình thường
             * sau lượt này các button đã collapse sẽ không được phép sử dụng, đồng thời cũng tăng size và chỉ mang 1 giá trị
             */
            if (isSpecial)
            {
                InsLabel.Text = "Choose a state to collapse the square:";
                btnLoop0.Visible = true;
                btnLoop1.Visible = true;
                btnLoop0.Text = eEdge0;
                btnLoop1.Text = eEdge1;
                pressedButton = button;
                return;
            }
            if (cyclicCreated) // btnLoop0 và btnLoop1 sẽ mang giá trị có thể của button này và kêu người chơi chọn
            {
                InsLabel.Text = "Choose a state to collapse the square:";
                btnLoop0.Visible = true;
                btnLoop1.Visible = true;
                int position = button.Name[button.Name.Length - 1] - '1';
                btnLoop0.Text = loopEdge[position][0];
                btnLoop1.Text = loopEdge[position][1];
                pressedButton = button;
                return;
            }

            /* lượt này sẽ chọn như bình thường
             * sẽ có 2 kiểu là lượt chọn đầu tiên và lượt chọn thứ hai của 1 người chơi
             */
            bool able = false; // kiểm tra nước đi hợp lệ
            if (button.Text.Length == 0) able = true; // ô trống là hợp lệ
            else 
            {
                int x = button.Text[button.Text.Length - 1] - '0'; // ô cùng turn là không hợp lệ
                if (x != turn) able = true;
                else InsLabel.Text = "Choose a different square!";
            }
            if (able) // nếu hợp lệ
            {
                string Value = ""; // mang giá trị là X hoặc O và chỉ số turn
                if (player % 2 == 1) // lượt của X
                {
                    if (button.Text != "") // nếu khác ô trống thì thêm dấu ","
                    {
                        button.Text += ",";
                    }
                    Value += "X";
                    Value += turn.ToString(); // thêm chỉ số lượt
                    button.Text += Value;
                    int position = button.Name[button.Name.Length - 1] - '1';
                }
                else // lượt của O
                {
                    if (button.Text != "") // nếu khác ô rỗng thì thêm dấu ","
                    {
                        button.Text += ",";
                    }
                    Value += "O";
                    Value += turn.ToString(); // thêm chỉ số lượt
                    button.Text += Value;
                    int position = button.Name[button.Name.Length - 1] - '1';
                }
                if (previousButton == -1)
                {
                    previousButton = button.Name[button.Name.Length - 1] - '1';
                }
                else
                {
                    turn++; // đến lượt kế tiếp
                    player++; // đến lượt người còn lại
                    if (player % 2 == 1) InsLabel.Text = "X turn.";
                    else InsLabel.Text = "O turn.";
                    int currentButton = button.Name[button.Name.Length - 1] -'1'; // button hiện tại
                    string tmp = edge[currentButton, previousButton]; // chỉ dùng khi vào loop đặc biệt
                    if (DSU[currentButton] == DSU[previousButton]) // kiểm tra entanglement loop
                    {
                        InsLabel.Text = "Cycle found!! Choose a square in the cycle.";
                        cyclicCreated = true;
                        if (edge[currentButton, previousButton] != "None")
                        {
                            isSpecial = true;
                            eEdge0 = edge[currentButton, previousButton];
                            eEdge1 = Value;
                        }
                    }
                    if (isSpecial)
                    {
                        eBtn0 = getButton(currentButton);
                        eBtn1 = getButton(previousButton);
                    }
                    edge[currentButton, previousButton] = Value;
                    edge[previousButton, currentButton] = Value;
                    if (cyclicCreated) // nếu có entanglement loop thì đánh dấu các đỉnh ở trong loop và các đỉnh liên thông với loop đó
                    {
                        loopHandle(currentButton, previousButton, Value);
                    }
                    if (!isSpecial)
                    {
                        adjacent[previousButton].Add(currentButton); // thêm button hiện tại vào danh sách kề của button trước đó
                        adjacent[currentButton].Add(previousButton); // thêm button trước đó vào danh sách kề của button hiện tại
                    }
                    for (int i = 0; i < 9; i++) // hợp thành phần liên thông của button hiện tại với thành phần liên thông của button trước đó
                    {
                        if (DSU[i] == DSU[currentButton] && i != currentButton) DSU[i] = DSU[previousButton];
                    }
                    DSU[currentButton] = DSU[previousButton];
                    previousButton = -1; // trả lại trạng thái đầu
                }
            } // kết thúc khối lệnh thực thi khi nước đi hợp lệ
        }

        private void btnLoop_Click(object sender, EventArgs e) // dùng để chọn trạng thái cho pressedButton
        {
            Button loopButton = (Button)sender;
            InsLabel.Text = "";
            cyclicCreated = false;
            for (int i = 0; i < 9; i++)
            {
                getButton(i).Enabled = true;
            }
            if (isSpecial) // chưa xử lý
            {
                isSpecial = false;
                eBtn0.Font = new Font("Microsoft Sans Serif", 30);
                eBtn1.Font = new Font("Microsoft Sans Serif", 30);
                pressedButton.Text = loopButton.Text;
                if (eBtn0 != pressedButton)
                {
                    if (eEdge0 != loopButton.Text) eBtn0.Text = eEdge0;
                    if (eEdge1 != loopButton.Text) eBtn0.Text = eEdge1;
                }
                if (eBtn1 != pressedButton)
                {
                    if (eEdge0 != loopButton.Text) eBtn1.Text = eEdge0;
                    if (eEdge1 != loopButton.Text) eBtn1.Text = eEdge1;
                }
                specialLoopCollapse(getId(eBtn0), "21");
                specialLoopCollapse(getId(eBtn1), "21");
                btnLoop0.Visible = false; // ẩn
                btnLoop0.Text = "";
                btnLoop1.Visible = false; // ẩn
                btnLoop1.Text = "";
                checkWinner();
                if (!gameEnd)
                {
                    if (player % 2 == 1) InsLabel.Text = "X turn.";
                    else InsLabel.Text = "O turn.";
                }
                return;
            }
            pressedButton.Font = new Font("Microsoft Sans Serif", 30);
            pressedButton.Text = loopButton.Text;
            loopCollapse(getId(pressedButton), loopButton.Text);
            btnLoop0.Visible = false; // ẩn
            btnLoop0.Text = "";
            btnLoop1.Visible = false; // ẩn
            btnLoop1.Text = "";
            checkWinner();
            if (!gameEnd)
            {
                if (player % 2 == 1) InsLabel.Text = "X turn.";
                else InsLabel.Text = "O turn.";
            }
        }

        /* Các hàm xử lý không liên quan đến trò chơi
         * Xử lý khi bắt đầu lại game mới
         */
        public void NewGame()
        {
            // trả về trạng thái ban đầu
            turn = 1;
            player = 1; 
            cyclicCreated = false;
            previousButton = -1;
            isSpecial = false;
            xPoint = 0;
            oPoint = 0;
            InsLabel.Text = "Game starts! X turn.";
            gameEnd = false;
            for (int i = 0; i < 9; i++)
            {
                if (loopEdge[i] != null) loopEdge[i].Clear();
                nameList[i] = "";
                Done[i] = false;
                DSU[i] = i;
                isInLoop[i] = -100;
                if (adjacent[i] != null) adjacent[i].Clear();
                for (int j = 0; j < 9; j++) edge[i, j] = "None";
            }
            btn1.Font = new Font("Microsoft Sans Serif", 8);
            btn2.Font = new Font("Microsoft Sans Serif", 8);
            btn3.Font = new Font("Microsoft Sans Serif", 8);
            btn4.Font = new Font("Microsoft Sans Serif", 8);
            btn5.Font = new Font("Microsoft Sans Serif", 8);
            btn6.Font = new Font("Microsoft Sans Serif", 8);
            btn7.Font = new Font("Microsoft Sans Serif", 8);
            btn8.Font = new Font("Microsoft Sans Serif", 8);
            btn9.Font = new Font("Microsoft Sans Serif", 8);
            btn1.Text = "";
            btn2.Text = "";
            btn3.Text = "";
            btn4.Text = "";
            btn5.Text = "";
            btn6.Text = "";
            btn7.Text = "";
            btn8.Text = "";
            btn9.Text = "";
            btn1.Enabled = true;
            btn2.Enabled = true;
            btn3.Enabled = true;
            btn4.Enabled = true;
            btn5.Enabled = true;
            btn6.Enabled = true;
            btn7.Enabled = true;
            btn8.Enabled = true;
            btn9.Enabled = true;
            btnLoop0.Text = "";
            btnLoop1.Text = "";
            btnLoop0.Visible = false;
            btnLoop1.Visible = false;
    }

        private void btnNewGame_Click(object sender, EventArgs e)
        {
            NewGame();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            btnLoop0.Visible = false;
            btnLoop1.Visible = false;
            btnRestart.Visible = false;
            for (int i = 0; i < 9; i++)
            {
                loopEdge[i] = new List<string>();
                adjacent[i] = new List<int>();
                Done[i] = false;
            }
            edge = new string[9, 9];
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    edge[i, j] = "None";
                }
            }
        }
    }
}
