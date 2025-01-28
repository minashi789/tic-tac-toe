using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;

namespace lab4krestiki
{
    public partial class Form1 : Form
    {

        int x0 = 50, y0 = 80;                           // Координаты начала игрового поля
        int hod = 0;                                    // Счетчик хода
        int score1 = 0, score2 = 0;                     // Cчет
        int[,] table = new int[3, 3];                    // Состояние игрового поля
        int win = 0;                               // Флаг победы (1-Х, 2-0, 3-ничья)
        int winx0 = 0, winy0 = 0, winx1 = 0, winy1 = 0; // Координаты выигрышной линии
        int cur = 0;                                    // Первый ход

        private void Form1_MouseClick(object sender, MouseEventArgs e)
        {
            if ((e.Button != MouseButtons.Left) || (e.X < x0) || (e.X > x0 + 450) || (e.Y < y0) || (e.Y > y0 + 450) ||
                (table[(e.X - x0) / 150, (e.Y - y0) / 150] != 0) || (win != 0)) return;

            if ((hod + cur) % 2 == 0) table[(e.X - x0) / 150, (e.Y - y0) / 150] = 1;
            else table[(e.X - x0) / 150, (e.Y - y0) / 150] = 2;
            hod++;
            Invalidate();               // Перерисовка поля
            savelog();

            // Проверка выигрыша
            for (int i = 0; i < 3; i++)
            {
                // Горизонтальные линии
                if (table[i, 0] == table[i, 1] && table[i, 1] == table[i, 2] && table[i, 0] != 0)
                {
                    winx0 = 75 + i * 150; winy0 = 10; winx1 = 75 + i * 150; winy1 = 440;
                    win = table[i, 0];
                    if (win == 1) score1++; else score2++;
                    break;
                }
                // Вертикальные линии
                if (table[0, i] == table[1, i] && table[1, i] == table[2, i] && table[0, i] != 0)
                {
                    winx0 = 10; winy0 = 75 + i * 150; winx1 = 440; winy1 = 75 + i * 150;
                    win = table[0, i];
                    if (win == 1) score1++; else score2++;
                    break;
                }
            }
            // Диагональные линии
            if (table[0, 0] == table[1, 1] && table[1, 1] == table[2, 2] && table[0, 0] != 0)
            {
                winx0 = 10; winy0 = 10; winx1 = 440; winy1 = 440;
                win = table[0, 0];
                if (win == 1) score1++; else score2++;
            }
            else if (table[0, 2] == table[1, 1] && table[1, 1] == table[2, 0] && table[0, 2] != 0)
            {
                winx0 = 440; winy0 = 10; winx1 = 10; winy1 = 440;
                win = table[0, 2];
                if (win == 1) score1++; else score2++;
            }

            // Проверка на ничью
            if ((hod == 9) && (win == 0))
            {
                MessageBox.Show("Ничья");
                win = 3;
            }
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.TranslateTransform(x0, y0);
            g.DrawRectangle(new Pen(Color.Black, 3.0f), 0, 0, 450, 450);
            for (int i = 0; i < 450; i += 150)
            {
                g.DrawLine(new Pen(Color.Black, 1.0f), 0, i, 450, i);
                g.DrawLine(new Pen(Color.Black, 1.0f), i, 0, i, 450);
            }

            if ((hod+cur) % 2 == 0)
            {
                Gamer1.BackColor = Color.BlanchedAlmond;
                Gamer2.BackColor = Color.White;
            } else
            {
                Gamer2.BackColor = Color.BlanchedAlmond;
                Gamer1.BackColor = Color.White;
            }

            for (int i = 0; i < 3; i++) for (int j = 0; j < 3; j++)
                {
                    if (table[i, j] == 1)
                    {
                        g.DrawLine(new Pen(Color.Red, 5.0f), 10 + i * 150, 10 + j * 150, 140 + i * 150, 140 + j * 150);
                        g.DrawLine(new Pen(Color.Red, 5.0f), 140 + i * 150, 10 + j * 150, 10 + i * 150, 140 + j * 150);
                    }
                    if (table[i, j] == 2) 
                        g.DrawEllipse(new Pen(Color.Green, 5.0f), 10 + i * 150, 10 + j * 150, 130, 130);
                }
            Score1.Text = score1.ToString();
            Score2.Text = score2.ToString();
            if (win != 0) g.DrawLine(new Pen(Color.Blue, 8.0f), winx0, winy0, winx1, winy1);
        }

        void ResetScore1()
        {
            score1 = score2 = 0;
            New();
            StreamWriter sw = File.CreateText("1.txt");
            sw.Close();
        }

        void New()
        {
            win = 0;
            hod = 0;
            cur = 1 - cur;
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++) table[i, j] = 0;
            Invalidate();
        }

        private void NewGame_Click(object sender, EventArgs e)
        {
            New();
        }

        private void сохранитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (win != 0)
            {
                MessageBox.Show("Запись завершенной игры невозможна");
                return;

            }
            StreamWriter sw1 = File.CreateText("2.txt");

            for (int j = 0; j < 3; j++)
            {
                for (int i = 0; i < 3; i++)
                {
                    sw1.WriteLine(table[i,j].ToString());
                }
            }
            sw1.WriteLine(hod.ToString());
            sw1.WriteLine(cur.ToString());
            sw1.WriteLine(score1.ToString());
            sw1.WriteLine(score2.ToString());

            sw1.Close();
        }

        private void загрузитьToolStripMenuItem_Click(object sender, EventArgs e)
        {

            StreamReader sr = File.OpenText("2.txt");
            for (int j = 0; j < 3; j++)
            {
                for (int i = 0; i < 3; i++)
                {
                    table[i, j] = Convert.ToInt32(sr.ReadLine());
                }
            }
            hod= Convert.ToInt32(sr.ReadLine());
            cur = Convert.ToInt32(sr.ReadLine());
            score1 = Convert.ToInt32(sr.ReadLine());
            score2 = Convert.ToInt32(sr.ReadLine());

            sr.Close();
            Invalidate();
        }

        private void ResetScore_Click(object sender, EventArgs e)
        {
            ResetScore1();
        }

        private void новаяИграToolStripMenuItem_Click(object sender, EventArgs e)
        {
            New();
        }

        private void сброситьСчетToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ResetScore1();
        }

        public Form1()
        {
            InitializeComponent();
        }

        public void savelog()
        {
            StreamWriter sw = File.AppendText("1.txt");
            string str = "";
            for (int j = 0; j < 3; j++)
            {
                for (int i = 0; i < 3; i++)
                {
                    if (table[i, j] == 0) str += "_ ";
                    if (table[i, j] == 1) str += "X ";
                    if (table[i, j] == 2) str += "O ";
                }
                str += "\n";
            }
            str += "\n";
            sw.WriteLine(str);
            sw.Close();
        }


    }
}
