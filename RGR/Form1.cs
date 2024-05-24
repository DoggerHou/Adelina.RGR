using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace RGR
{
    public partial class Form1 : Form
    {
        List<Circle> storage; //массив объектов(вершин)
        bool controlUp;//Зажата ли кнопка ctrl
        int[,] arr;//Матрица смежности
        int[,] dostij;//Матрица достижимости
        int versh = -1;//для запоминания выбранной вершины
        public Form1()
        {
            InitializeComponent();
            KeyPreview = true;
            storage = new List<Circle>();
        }


        //Нажатие мыши
        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            //добавление ребер
            if (e.Button == MouseButtons.Right)
            {
                if (versh == -1)
                {
                    for (int i = 0; i < storage.Count; i++)
                        if (storage[i].isPicked(e, controlUp))
                        {
                            versh = i;
                            break;
                        }
                }
                else
                {
                    int toversh = -1;
                    for (int i = 0; i < storage.Count; i++)
                        if (storage[i].isPicked(e, controlUp))
                        {
                            toversh = i;
                            break;
                        }
                    if ((toversh != -1) && (versh != toversh))
                    {
                        storage[versh].addVershin(toversh);
                        versh = -1;
                    }
                }
            }

            //Добавление вершин
            if (e.Button == MouseButtons.Left)
            {
                if (controlUp)
                    foreach (var obj in storage)
                    {
                        if (obj.isPicked(e, controlUp) && obj.getDetail() == false)
                            obj.changeDetail_to(true);
                        else if (obj.isPicked(e, controlUp) && obj.getDetail() == true)
                            obj.changeDetail_to(false);
                    }
                else
                    storage.Add(new Circle(e.Location, storage.Count));
                arr = new int[storage.Count, storage.Count];
            }
            pictureBox1.Invalidate();
        }


        //Отрисовка формы
        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            textBox1.Text = "";
            foreach (var obj in storage)
                obj.OnPaint(e, storage);
        }

        //Нажатие кнопки "Получить матрицу"
        private void button1_Click(object sender, EventArgs e)
        {
            dostij = new int[storage.Count, storage.Count];
            listBox1.Items.Clear();

            //Заполняем матрицу смежности
            for (int j = 0; j < storage.Count; j++)
            {
                var l = storage[j].vershin;
                for (int i = 0; i < l.Count; i++)
                {
                    arr[j, l[i]] = 1;
                }
            }

            for (int i = 0; i < storage.Count; i++)
                for (int j = 0; j < storage.Count; j++)
                    if (arr[i, j] == 1 || arr[j, i] == 1)
                    {
                        arr[j, i] = 1;
                        arr[i, j] = 1;
                    }


            for (int k = 0; k < storage.Count; k++)
                for (int i = 0; i < storage.Count; i++)
                    for (int j = 0; j< storage.Count; j++)
                        arr[i, j] = (arr[i, j] | (arr[i, k] & arr[k, j]));

            //Рисуем матрицу достижимости
            string sOut = "     ";
            for (int i = 0; i < storage.Count; i++)
                sOut += (i + 1) + "  ";
            listBox1.Items.Add(sOut);
            for (int i = 0; i < storage.Count; i++)
            {
                sOut = (i + 1) + " | ";
                for (int j = 0; j < storage.Count; j++)
                    sOut += arr[i, j] + "  ";
                listBox1.Items.Add(sOut);
            }

        }


        //Обработка кнопки "Задание на РГР"
        private void button3_Click(object sender, EventArgs e)
        {
            if (textBox2.Text == "" || textBox3.Text == "")
                MessageBox.Show("Вы ввели не все вершины!");
            else
            {
                if (arr[int.Parse(textBox2.Text) - 1, int.Parse(textBox3.Text) - 1] == 1)
                    textBox1.Text = "Путь есть";
                else
                    textBox1.Text = "Пути нет";
            }

        }


        //Обработка нажатий на клавиатуру
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control)
                controlUp = true;
            if (e.KeyCode == Keys.Delete)
            {
                for (int i = storage.Count - 1; i >= 0; i--)
                    if (storage[i].getDetail())
                        storage.Remove(storage[i]);
                for (int i = storage.Count - 1; i >= 0; i--)
                    storage[i].changeNumber(i);
            }
            pictureBox1.Invalidate();

        }

        //Отпустили кнопку
        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            controlUp = false;
        }
    }
}