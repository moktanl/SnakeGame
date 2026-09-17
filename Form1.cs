using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace SnakeGame
{
    public partial class Form1 : Form
    {
        // GAME VARIABLES
        private List<Point> snake = new List<Point>();
        private Point food;
        private string direction = "Right";
        private string nextDirection = "Right";
        private int score = 0;
        private System.Windows.Forms.Timer gameTimer = new System.Windows.Forms.Timer();
        private Random random = new Random();

        // PAUSE STATE
        private bool isPaused = false;

        // Colors
        private Color lcdBackground = Color.FromArgb(170, 190, 70);
        private Color pixelColor = Color.FromArgb(35, 55, 20);

        // Game area
        private const int cellSize = 15;
        private const int boardLeft = 30;
        private const int boardTop = 80;
        private const int boardWidth = 540;
        private const int boardHeight = 450;

        // FORM CONSTRUCTOR
        public Form1()
        {
            InitializeComponent();

            // Form settings
            this.Text = "Snake Game";
            this.ClientSize = new Size(600, 600);

            this.BackColor = lcdBackground;
            this.KeyPreview = true;
            this.DoubleBuffered = true;
            this.Paint -= Form1_Paint;
            this.Paint += Form1_Paint;

            // Keyboard controls
            this.KeyDown -= Form1_KeyDown;
            this.KeyDown += Form1_KeyDown;

            // Timer
            gameTimer.Interval = 130;
            gameTimer.Tick += GameTimer_Tick;

            StartGame();
        }

        // START GAME
        private void StartGame()
        {
            snake.Clear();

            // Starting snake
            snake.Add(new Point(255, 275));
            snake.Add(new Point(240, 275));
            snake.Add(new Point(225, 275));
            snake.Add(new Point(210, 275));

            direction = "Right";
            nextDirection = "Right";

            score = 0;
            isPaused = false;

            CreateFood();

            gameTimer.Start();

            Invalidate();
        }

        // CREATE FOOD
        private void CreateFood()
        {
            int columns = boardWidth / cellSize;
            int rows = boardHeight / cellSize;

            do
            {
                int x = random.Next(columns);
                int y = random.Next(rows);

                food = new Point(
                    boardLeft + x * cellSize,
                    boardTop + y * cellSize
                );

            } while (snake.Contains(food));
        }

        // GAME TIMER
        private void GameTimer_Tick(object sender, EventArgs e)
        {
            direction = nextDirection;

            Point head = snake[0];
            Point newHead = head;

            // Move snake
            if (direction == "Up")
            {
                newHead.Y -= cellSize;
            }
            else if (direction == "Down")
            {
                newHead.Y += cellSize;
            }
            else if (direction == "Left")
            {
                newHead.X -= cellSize;
            }
            else if (direction == "Right")
            {
                newHead.X += cellSize;
            }

            // WALL COLLISION
            if (newHead.X < boardLeft ||
                newHead.X >= boardLeft + boardWidth ||
                newHead.Y < boardTop ||
                newHead.Y >= boardTop + boardHeight)
            {
                GameOver();
                return;
            }

            // BODY COLLISION
            if (snake.Contains(newHead))
            {
                GameOver();
                return;
            }

            // Add new head
            snake.Insert(0, newHead);

            // FOOD COLLISION
            if (newHead == food)
            {
                score++;
                CreateFood();
            }
            else
            {
                // Remove tail
                snake.RemoveAt(snake.Count - 1);
            }

            Invalidate();
        }

        // KEYBOARD CONTROLS
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            // Toggle pause on Spacebar keypress
            if (e.KeyCode == Keys.Space)
            {
                TogglePause();
                return;
            }

            // Ignore movement input while paused
            if (isPaused) return;

            if (e.KeyCode == Keys.Up && direction != "Down")
            {
                nextDirection = "Up";
            }
            else if (e.KeyCode == Keys.Down && direction != "Up")
            {
                nextDirection = "Down";
            }
            else if (e.KeyCode == Keys.Left && direction != "Right")
            {
                nextDirection = "Left";
            }
            else if (e.KeyCode == Keys.Right && direction != "Left")
            {
                nextDirection = "Right";
            }
        }

        // TOGGLE PAUSE STATE
        private void TogglePause()
        {
            isPaused = !isPaused;

            if (isPaused)
            {
                gameTimer.Stop();
            }
            else
            {
                gameTimer.Start();
            }

            Invalidate(); // Refresh display to render PAUSED text
        }

        // GAME OVER
        private void GameOver()
        {
            gameTimer.Stop();

            DialogResult result = MessageBox.Show(
                "GAME OVER\n\nScore: " + score +
                "\n\nPlay Again?",
                "Snake",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Information
            );

            if (result == DialogResult.Yes)
            {
                StartGame();
            }
            else
            {
                Application.Exit();
            }
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            g.Clear(lcdBackground);

            // Pixel-style drawing
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;

            // SCORE
            using (Font scoreFont = new Font("Consolas", 28, FontStyle.Bold))
            {
                g.DrawString(
                    score.ToString("D4"),
                    scoreFont,
                    new SolidBrush(pixelColor),
                    35,
                    20
                );
            }

            // GAME BORDER
            using (Pen borderPen = new Pen(pixelColor, 5))
            {
                g.DrawRectangle(
                    borderPen,
                    boardLeft,
                    boardTop,
                    boardWidth,
                    boardHeight
                );
            }

            // FOOD
            DrawFood(g);

            // SNAKE
            for (int i = 0; i < snake.Count; i++)
            {
                Point part = snake[i];

                Rectangle rect = new Rectangle(
                    part.X + 1,
                    part.Y + 1,
                    cellSize - 2,
                    cellSize - 2
                );

                using (Brush brush = new SolidBrush(pixelColor))
                {
                    g.FillRectangle(brush, rect);
                }
            }

            // SNAKE HEAD 
            DrawSnakeHead(g);

            // PAUSED OVERLAY
            if (isPaused)
            {
                using (Font pauseFont = new Font("Consolas", 36, FontStyle.Bold))
                {
                    string pauseText = "PAUSED";
                    SizeF textSize = g.MeasureString(pauseText, pauseFont);

                    float x = boardLeft + (boardWidth - textSize.Width) / 2;
                    float y = boardTop + (boardHeight - textSize.Height) / 2;

                    g.DrawString(pauseText, pauseFont, new SolidBrush(pixelColor), x, y);
                }
            }
        }

        // DRAW FOOD
        private void DrawFood(Graphics g)
        {
            using (Brush brush = new SolidBrush(pixelColor))
            {
                int x = food.X;
                int y = food.Y;

                // Pixel cross shape
                g.FillRectangle(brush, x + 5, y, 5, 15);
                g.FillRectangle(brush, x, y + 5, 15, 5);
            }
        }

        // DRAW SNAKE HEAD
        private void DrawSnakeHead(Graphics g)
        {
            if (snake.Count == 0)
                return;

            Point head = snake[0];

            using (Brush brush = new SolidBrush(lcdBackground))
            {
                // Small pixel eye
                if (direction == "Right")
                {
                    g.FillRectangle(brush, head.X + 10, head.Y + 3, 2, 2);
                }
                else if (direction == "Left")
                {
                    g.FillRectangle(brush, head.X + 3, head.Y + 3, 2, 2);
                }
                else if (direction == "Up")
                {
                    g.FillRectangle(brush, head.X + 3, head.Y + 3, 2, 2);
                }
                else if (direction == "Down")
                {
                    g.FillRectangle(brush, head.X + 3, head.Y + 10, 2, 2);
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}