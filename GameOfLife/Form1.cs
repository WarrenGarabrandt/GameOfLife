using System.Drawing.Drawing2D;

namespace GameOfLife
{
    public partial class Form1 : Form
    {
        private const int WIDTH = 256;
        private const int HEIGHT = 256;

        private static Color LIVE_COLOR = Color.Black;
        private static Color DEAD_COLOR = Color.White;

        private Brush LIVE_BRUSH = new SolidBrush(LIVE_COLOR);
        private Brush DEAD_BRUSH = new SolidBrush(DEAD_COLOR);

        private long GenerationCount = 0;

        public Form1()
        {
            InitializeComponent();
            ImageA = new Bitmap(WIDTH, HEIGHT);
            ImageB = new Bitmap(WIDTH, HEIGHT);
            ActiveImage = enuActiveImage.A;
            //Randomize();
        }

        private Bitmap ImageA;
        private Bitmap ImageB;

        private byte[,] liveNeigbors = new byte[WIDTH, HEIGHT];
        private bool[,] isAlive = new bool[WIDTH, HEIGHT];

        private Random rnd = new Random();
        private int SprinkleCutoff = Int32.MaxValue / 10;
        private const int MinSurvivePop = 2;
        private const int MaxSurvivePop = 3;

        private enum enuActiveImage
        {
            A,
            B
        }

        private enuActiveImage ActiveImage;

        bool IsMouseLeftDown = false;
        bool IsMouseRightDown = false;
        bool ClearScreen = false;

        private void Randomize()
        {
            int cutoff = Int32.MaxValue / 2;
            Random rnd = new Random();
            using (Graphics g = Graphics.FromImage(ImageA))
            {
                g.Clear(DEAD_COLOR);
                for (int y = 1; y < HEIGHT - 1; y++)
                {
                    for (int x = 1; x < WIDTH - 1; x++)
                    {
                        if (rnd.Next() < cutoff)
                        {
                            g.FillRectangle(LIVE_BRUSH, x, y, 1, 1);
                        }
                    }
                }
            }
        }

        private void Sprinkle()
        {
            Bitmap imgA;
            if (ActiveImage == enuActiveImage.A)
            {
                imgA = ImageA;
            }
            else
            {
                imgA = ImageB;
            }
            
            using (Graphics g = Graphics.FromImage(imgA))
            {
                for (int y = 1; y < HEIGHT - 1; y++)
                {
                    for (int x = 1; x < WIDTH - 1; x++)
                    {
                        if (rnd.Next() < SprinkleCutoff)
                        {
                            g.FillRectangle(LIVE_BRUSH, x, y, 1, 1);
                        }
                    }
                }
            }
        }

        private void Spawn()
        {
            Bitmap imgA;
            if (ActiveImage == enuActiveImage.A)
            {
                imgA = ImageA;
            }
            else
            {
                imgA = ImageB;
            }

            RandomObject obj = Objects[rnd.Next(Objects.Count)];

            int posx = rnd.Next(WIDTH - obj.Width - 2) + 1;
            int posy = rnd.Next(HEIGHT - obj.Height - 2) + 1;

            using (Graphics g = Graphics.FromImage(imgA))
            {
                for (int y = 0; y < obj.Height; y++)
                {
                    for (int x = 0; x < obj.Width; x++)
                    {
                        if (obj.grid[y, x] == 1)
                        {
                            g.FillRectangle(LIVE_BRUSH, x + posx, y + posy, 1, 1);
                        }
                    }
                }
            }
        }

        private void GenerationStep()
        {
            Bitmap imgA;
            Bitmap imgB;
            if (ActiveImage == enuActiveImage.A)
            {
                imgA = ImageA;
                imgB = ImageB;
                ActiveImage = enuActiveImage.B;
            }
            else
            {
                imgB = ImageA;
                imgA = ImageB;
                ActiveImage = enuActiveImage.A;
            }

            for (int y = 1; y < HEIGHT - 1; y++)
            {
                for (int x = 1; x < WIDTH - 1; x++)
                {
                    liveNeigbors[x, y] = 0;
                }
            }
            if (!ClearScreen)
            {
                for (int y = 1; y < HEIGHT - 1; y++)
                {
                    for (int x = 1; x < WIDTH - 1; x++)
                    {
                        Color c = imgA.GetPixel(x, y);
                        isAlive[x, y] = c.R == LIVE_COLOR.R;
                        if (isAlive[x, y])
                        {
                            liveNeigbors[x - 1, y - 1]++;
                            liveNeigbors[x, y - 1]++;
                            liveNeigbors[x + 1, y - 1]++;
                            liveNeigbors[x - 1, y]++;
                            liveNeigbors[x + 1, y]++;
                            liveNeigbors[x - 1, y + 1]++;
                            liveNeigbors[x, y + 1]++;
                            liveNeigbors[x + 1, y + 1]++;
                        }
                    }
                }
            }
            else
            {
                ClearScreen = false;
            }

            using (Graphics gB= Graphics.FromImage(imgB))   // Destination Image
            {
                gB.Clear(DEAD_COLOR);
                for (int y = 1; y < HEIGHT - 1; y++)
                {
                    for (int x = 1; x < WIDTH - 1; x++)
                    {
                        if (isAlive[x,y])
                        {
                            if (liveNeigbors[x,y] < MinSurvivePop || liveNeigbors[x, y] > MaxSurvivePop)
                                isAlive[x, y] = false;
                        }
                        else if (liveNeigbors[x, y] == 3)
                        {
                            isAlive[x, y] = true;
                        }
                        if (isAlive[x, y])
                        {
                            gB.FillRectangle(LIVE_BRUSH, x, y, 1, 1);
                        }
                    }
                }
            }
            using (Graphics g = this.CreateGraphics())
            {
                g.InterpolationMode = InterpolationMode.NearestNeighbor;
                g.DrawImage(imgB, new Rectangle(0, 18, 1024, 1024), new Rectangle(0, 0, WIDTH, HEIGHT), GraphicsUnit.Pixel);
            }
        }

        private void tmrGenTick_Tick(object sender, EventArgs e)
        {
            if (IsMouseLeftDown)
            {
                Sprinkle();
            }
            if (IsMouseRightDown)
            {
                Spawn();
            }
            GenerationCount++;
            GenerationStep();
            lblGenCount.Text = string.Format("Generation {0}", GenerationCount);

        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                IsMouseLeftDown = false;
            }
            if (e.Button == MouseButtons.Right)
            {
                IsMouseRightDown = false;
            }
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                IsMouseLeftDown = true;
            }
            if (e.Button == MouseButtons.Right)
            {
                IsMouseRightDown = true;
            }
        }

        private void Form1_Leave(object sender, EventArgs e)
        {
            IsMouseLeftDown = false;
            IsMouseRightDown = true;
        }

        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == ' ')
            {
                e.Handled = true;
                ClearScreen = true;
            }
        }
        internal class RandomObject
        {
            internal string Name = string.Empty;
            internal int Width;
            internal int Height;
            internal byte[,] grid = new byte[0, 0];
        }

        private List<RandomObject> Objects = new List<RandomObject>()
        {
            new RandomObject()
            {
                Name = "Block",
                Width = 2,
                Height = 2,
                grid = new byte[2,2]
                {
                    {1,1},
                    {1,1}
                }
            },
            new RandomObject()
            {
                Name = "Bee-hive",
                Width = 4,
                Height = 3,
                grid = new byte[3,4]
                {
                    {0,1,1,0},
                    {1,0,0,1},
                    {0,1,1,0}
                }
            },
            new RandomObject()
            {
                Name = "Loaf",
                Width = 4,
                Height = 4,
                grid = new byte[4,4]
                {
                    {0,1,1,0},
                    {1,0,0,1},
                    {0,1,0,1},
                    {0,0,1,0}
                }
            },
            new RandomObject()
            {
                Name = "Boat",
                Width = 3,
                Height = 3,
                grid = new byte[3,3]
                {
                    {1,1,0},
                    {1,0,1},
                    {0,1,0}
                }
            },
            new RandomObject()
            {
                Name = "Tub",
                Width = 3,
                Height = 3,
                grid = new byte[3,3]
                {
                    {0,1,0},
                    {1,0,1},
                    {0,1,0}
                }
            },
            new RandomObject()
            {
                Name = "Blinker",
                Width = 3,
                Height = 3,
                grid = new byte[3,3]
                {
                    {0,1,0},
                    {0,1,0},
                    {0,1,0}
                }
            },
            new RandomObject()
            {
                Name = "Toad 1",
                Width = 4,
                Height = 2,
                grid = new byte[2,4]
                {
                    {0,1,1,1},
                    {1,1,1,0}
                }
            },
            new RandomObject()
            {
                Name = "Toad 2",
                Width = 2,
                Height = 4,
                grid = new byte[4,2]
                {
                    {1,0 },
                    {1,1 },
                    {1,1 },
                    {0,1 }
                }
            },
            new RandomObject()
            {
                Name = "Beacon 1",
                Width = 4,
                Height = 4,
                grid = new byte[4,4]
                {
                    {1,1,0,0},
                    {1,1,0,0},
                    {0,0,1,1},
                    {0,0,1,1}
                }
            },
            new RandomObject()
            {
                Name = "Beacon 2",
                Width = 4,
                Height = 4,
                grid = new byte[4,4]
                {
                    {0,0,1,1},
                    {0,0,1,1},
                    {1,1,0,0},
                    {1,1,0,0}
                }
            },
            new RandomObject()
            {
                Name = "Pulsar",
                Width = 13,
                Height = 13,
                grid = new byte[13,13]
                {
                    {0,0,1,1,1,0,0,0,1,1,1,0,0},
                    {0,0,0,0,0,0,0,0,0,0,0,0,0},
                    {1,0,0,0,0,1,0,1,0,0,0,0,1},
                    {1,0,0,0,0,1,0,1,0,0,0,0,1},
                    {1,0,0,0,0,1,0,1,0,0,0,0,1},
                    {0,0,1,1,1,0,0,0,1,1,1,0,0},
                    {0,0,0,0,0,0,0,0,0,0,0,0,0},
                    {0,0,1,1,1,0,0,0,1,1,1,0,0},
                    {1,0,0,0,0,1,0,1,0,0,0,0,1},
                    {1,0,0,0,0,1,0,1,0,0,0,0,1},
                    {1,0,0,0,0,1,0,1,0,0,0,0,1},
                    {0,0,0,0,0,0,0,0,0,0,0,0,0},
                    {0,0,1,1,1,0,0,0,1,1,1,0,0},
                }
            },
            new RandomObject()
            {
                Name = "Penta-Decathlon",
                Width = 9,
                Height = 10,
                grid = new byte[10,9]
                {
                    {0,0,0,1,1,1,0,0,0 },
                    {0,0,1,0,0,0,1,0,0 },
                    {0,1,0,0,0,0,0,1,0 },
                    {0,0,0,0,0,0,0,0,0 },
                    {1,0,0,0,0,0,0,0,1 },
                    {1,0,0,0,0,0,0,0,1 },
                    {0,0,0,0,0,0,0,0,0 },
                    {0,1,0,0,0,0,0,1,0 },
                    {0,0,1,0,0,0,1,0,0 },
                    {0,0,0,1,1,1,0,0,0 }
                }
            },
            new RandomObject()
            {
                Name = "Glider SE",
                Width = 3,
                Height = 3,
                grid = new byte[3,3]
                {
                    {0,1,0 },
                    {0,0,1 },
                    {1,1,1 }
                }
            },
            new RandomObject()
            {
                Name = "Glider SW",
                Width = 3,
                Height = 3,
                grid = new byte[3,3]
                {
                    {0,1,0 },
                    {1,0,0 },
                    {1,1,1 }
                }
            },
            new RandomObject()
            {
                Name = "Glider NE",
                Width = 3,
                Height = 3,
                grid = new byte[3,3]
                {
                    {1,1,1 },
                    {0,0,1 },
                    {0,1,0 }
                }
            },
            new RandomObject()
            {
                Name = "Glider NW",
                Width = 3,
                Height = 3,
                grid = new byte[3,3]
                {
                    {1,1,1 },
                    {1,0,0 },
                    {0,1,0 }
                }
            },
            new RandomObject()
            {
                Name = "LWSS E",
                Width = 5,
                Height = 4,
                grid = new byte[4,5]
                {
                    {1,0,0,1,0 },
                    {0,0,0,0,1 },
                    {1,0,0,0,1 },
                    {0,1,1,1,1 }
                }
            },
            new RandomObject()
            {
                Name = "LWSS W",
                Width = 5,
                Height = 4,
                grid = new byte[4,5]
                {
                    {0,1,0,0,1 },
                    {1,0,0,0,0 },
                    {1,0,0,0,1 },
                    {1,1,1,1,0 }
                }
            },
            new RandomObject()
            {
                Name = "LWSS N",
                Width = 4,
                Height = 5,
                grid = new byte[5,4]
                {
                    {1,1,1,0 },
                    {1,0,0,1 },
                    {1,0,0,0 },
                    {1,0,0,0 },
                    {0,1,0,1 }
                }
            },
            new RandomObject()
            {
                Name = "LWSS S",
                Width = 4,
                Height = 5,
                grid = new byte[5,4]
                {
                    {0,1,0,1 },
                    {1,0,0,0 },
                    {1,0,0,0 },
                    {1,0,0,1 },
                    {1,1,1,0 }
                }
            },
        };

    }
}