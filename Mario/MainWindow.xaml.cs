using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Threading;

namespace Mario
{
	/*
	 * status:
	 * 0 - афк
	 * 1 - ходьба вправо
	 * 2 - ходьба влево
	 * 3 - прыжок вправо
	 * 4 - прыжок влево
	 * 5 - в полете вправо
	 * 6 - в полете влево
	 * 7 - падение вправо
	 * 8 - падение влево
	 * 9 - удар мечом вправо
	 * 10 - удар мечом влево
	 * 11 - получение урона
	 * 12 - смерть
	 */

	public partial class MainWindow : Window
	{
		List<Rectangle> needAdd = new List<Rectangle>();
        List<Rectangle> needDelete = new List<Rectangle>();
        List<Rectangle> Hearthes1 = new List<Rectangle>();
		List<Rectangle> Hearthes2 = new List<Rectangle>();
		List<Player> Players = new List<Player>();

		DispatcherTimer timer = new DispatcherTimer(DispatcherPriority.Render);
		DispatcherTimer changeSkin = new DispatcherTimer(DispatcherPriority.Render);

		ImageBrush mapText = new ImageBrush();
		ImageBrush livePoint1 = new ImageBrush();
		ImageBrush livePoint2 = new ImageBrush();
        ImageBrush dd = new ImageBrush();
        ImageBrush haste = new ImageBrush();

        Random rnd = new Random();

        int runeTimer = 0;

		ScaleTransform st = new ScaleTransform(1.5, 1.7, 0.5, 1);

		public MainWindow ( )
		{
			InitializeComponent();
			Game.Focus();
			timer.Interval = TimeSpan.FromMilliseconds(10);
			timer.Tick += update;
			changeSkin.Interval = TimeSpan.FromMilliseconds(80);
			changeSkin.Tick += chSkin;			
            

			startGame();
		}


		private void update( object sender, EventArgs e )
		{
			foreach (Player pl in Players)
			{

                if (pl.right)
				{
					pl.hv = 1.6;
					if (pl.vx < pl.maxSpeed) pl.vx += pl.speed;
				}
				if (pl.left)
				{
					pl.hv = 1.6;
					if (pl.vx > -pl.maxSpeed) pl.vx -= pl.speed;
				}
				if (pl.left == false && pl.right == false)
				{
					if (pl.vx > 0)
					{
						if (pl.hv < 0.5)
						{
							pl.vx = 0;
						}
						else
						{
							pl.vx -= pl.hv / 2;
							pl.hv = pl.hv / 2;
						}
					}
					else if (pl.vx < 0)
					{
						if (pl.hv < 0.5)
						{
							pl.vx = 0;
						}
						else
						{
							pl.vx += pl.hv / 2;
							pl.hv = pl.hv / 2;
						}
					}
				}

				if (Canvas.GetTop(pl.rect) + pl.rect.Height >= Application.Current.MainWindow.Height)
				{
					Canvas.SetLeft(pl.rect, 450);
					Canvas.SetTop(pl.rect, 350);
				}

				pl.vy += 0.3;
				pl.down = true;

				Rect PlayerHitBox = new Rect(Canvas.GetLeft(pl.rect) + 10, Canvas.GetTop(pl.rect) + 50, pl.rect.Width - 25, pl.rect.Height - 50);
				foreach (var x in Game.Children.OfType<Rectangle>())
				{
                    if((string)x.Tag == "dd")
                    {
                        Rect Rune = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);
                        if (PlayerHitBox.IntersectsWith(Rune))
                        {
                            pl.damage = pl.damage * 2;
                            needDelete.Add(x);
                            pl.ddTimer = 2000;
                            pl.dd = true;
                        }
                    }
                    if ((string)x.Tag == "haste")
                    {
                        Rect Rune = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);
                        if (PlayerHitBox.IntersectsWith(Rune))
                        {
                            pl.speed = pl.speed * 2;
                            pl.maxSpeed = pl.maxSpeed * 2;
                            needDelete.Add(x);
                            pl.hasteTimer = 2000;
                            pl.haste = true;
                        }
                    }
                    if ((string)x.Tag == "platf")
					{
						Rect PlatfHitBox = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x) - 20, x.Width, x.Height);
						if (PlayerHitBox.IntersectsWith(PlatfHitBox))
						{
							pl.vy = 0;
							pl.down = false;
							Canvas.SetTop(pl.rect, Canvas.GetTop(x) - pl.rect.Height - 20);
							if (pl.left == false && pl.right == false) pl.status = 0;

						}
					}
					if ((string)x.Tag == "Airplatf" && !(pl.status == 5 || pl.status == 6))
					{
						Rect PlatfHitBox = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x) - 20, x.Width, x.Height);
						if (PlayerHitBox.IntersectsWith(PlatfHitBox))
						{
							pl.vy = 0;
							pl.down = false;
							Canvas.SetTop(pl.rect, Canvas.GetTop(x) - pl.rect.Height - 20);
							if (pl.left == false && pl.right == false) pl.status = 0;

						}
                        if (runeTimer == 0)
                        {
                            int chance = rnd.Next(0, 100);
                            if (chance < 20)
                            {
                                Rectangle rune = new Rectangle
                                {
                                    Width = 30,
                                    Height = 30
                                };
                                chance = rnd.Next(1, 3);
                                if (chance == 1)
                                {
                                    rune.Fill = dd;
                                    rune.Tag = "dd";
                                }
                                else
                                {
                                    rune.Fill = haste;
                                    rune.Tag = "haste";
                                }
                                runeTimer = 15000;
                                needAdd.Add(rune);
                                Canvas.SetTop(rune, Canvas.GetTop(x) - 30);
                                Canvas.SetLeft(rune, rnd.Next((int)Canvas.GetLeft(x), (int)Canvas.GetLeft(x) + (int)x.Width - 30));
                            }
                        }
					}
					if ((string)x.Tag == "platfF")
					{

					}
				}
				
				if (pl.vy < 0 && pl.down == true)
				{
					pl.flying = true;
					if (pl.left)
					{
						pl.status = 6;
					}
					else
					{
						pl.status = 5;
					}
				}
				else if (pl.vy > 0 && pl.down == true)
				{
					pl.flying = false;
					pl.falling = true;
					if (pl.left)
					{
						pl.status = 8;
					}
					else
					{
						pl.status = 7;
					}
				}
				else if (pl.down == false)
				{
					if (pl.left) pl.status = 2;
					else if (pl.right) pl.status = 1;
					else pl.status = 0;
				}
				if (pl.jump && pl.down == false)
				{
					pl.vy = -7;
					pl.jump = false;
				}
				if (pl.attackTimer > 0 && pl.status != 12)
				{
					pl.attack = true;
					if (pl.attack && pl.left)
					{
						pl.status = 10;
						pl.attack = false;
					}
					else
					{
						pl.status = 9;
						pl.attack = false;
					}
					if (!pl.alrAttack)
					{
						foreach (Player p in Players)
						{
							Rect Enemy = new Rect(Canvas.GetLeft(p.rect) + 10, Canvas.GetTop(p.rect) + 10, p.rect.Width - 25, p.rect.Height - 10);
							if (p != pl && PlayerHitBox.IntersectsWith(Enemy))
							{
								p.hp -= 30;
								pl.alrAttack = true;
								deleteHearth(p);
								if (p.hp == 0)
								{
									pl.hp = 90;
									refreshHearth(pl.id);
								}
							}
						}
					}
					pl.attackTimer -= 10;
				}
				if(pl.hp <= 0)
				{
					pl.status = 12;
					pl.vx = 0;
				}
				else
				{
					
				}

				Canvas.SetLeft(pl.rect, Canvas.GetLeft(pl.rect) + pl.vx);
				Canvas.SetTop(pl.rect, Canvas.GetTop(pl.rect) + pl.vy);

				if (pl == Players[1])
				{
					skinL.Content = "Skin " + pl.skin.ToString();
					statusL.Content = "Status " + pl.status;
					Hp.Content = "Hp " + pl.hp;
				}
                if (pl.dd)
                {
                    pl.ddTimer -= 10;
                    if (pl.ddTimer == 0)
                    {
                        pl.dd = false;
                        pl.damage = pl.damage / 2;
                    }
                }
                if (pl.haste)
                {
                    pl.hasteTimer -= 10;
                    if (pl.hasteTimer == 0)
                    {
                        pl.haste = false;
                        pl.speed = pl.speed / 2;
                        pl.maxSpeed = pl.maxSpeed / 2;
                    }
                }
            }
            foreach(var f in needDelete)
            {
                Game.Children.Remove(f);
            }
            needDelete.Clear();
            foreach (var f in needAdd)
            {
                Game.Children.Add(f);
            }
            needAdd.Clear();
            runeTimer -= 10;

		}
		private void deleteHearth(Player p)
        {
			int num = p.hp / 30;
            try
            {
                if (num > 0)
                {
                    if (p.id == 1)
                    {
                        Hearthes1[num].Fill = null;
                    }
                    else if (p.id == 2)
                    {
                        Hearthes2[num].Fill = null;
                    }
                }
                else if (num == 0)
                {
                    if (p.id == 1)
                    {
                        Hearthes1[0].Fill = null;
                    }
                    else if (p.id == 2)
                    {
                        Hearthes2[0].Fill = null;
                    }
                }
            }
            catch { }
		}

		private void refreshHearth(int id)
        {
			if (id == 1)
			{
				foreach (var x in Hearthes1)
                {
                    x.Fill = livePoint1;
                }
			}
			else if (id == 2)
			{
				foreach (var x in Hearthes2)
                {
                    x.Fill = livePoint2;
                }
			}

		}
		private void chSkin ( object sender, EventArgs e )
		{
			foreach (Player pl in Players)
			{
				try
				{
					pl.playerText.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/moving" + pl.id + "/st" + pl.status + "/" + pl.skin + ".png"));
					pl.rect.Fill = pl.playerText;
					pl.rect.RenderTransform = st;
				}
				catch
				{
					if (pl.status == 3 || pl.status == 4)
					{
						pl.jump = false;
						if (pl.right) pl.status = 5;
						else if (pl.left) pl.status = 6;
						pl.flying = true;
					}
					if(pl.status == 12)
					{
						pl.status = 0;
						pl.hp = 90;
						Canvas.SetTop(pl.rect, 470);
						Canvas.SetLeft(pl.rect, 450);
						refreshHearth(pl.id);
					}
					if(pl.status == 9 || pl.status == 10)
					{
						pl.alrAttack = false;
					}
					pl.skin = 0;
				}
				pl.skin++;
				if (pl.skin == 14)
				{
					pl.skin = 1;
				}
			}
		}

		void startGame ( )
		{
			timer.Start();
			changeSkin.Start();
			generateFullFloor();
			generatePlayers();
			
		}
		void generatePlayers ( )
		{
			int posLeft = 450;
			for(int i = 1; i<=2; i++)
			{
				Player pl = new Player();
				pl.id = i;
				
				Rectangle plRect = new Rectangle
				{
					Width = 56,
					Height = 58,
					Name = "Player" + i,
					//Stroke = Brushes.Black,
					RenderTransformOrigin = new Point(0.5,0.5)
				};
				pl.rect = plRect;
				Players.Add(pl);
				Game.Children.Add(plRect);
				Canvas.SetTop(plRect, 470);
				Canvas.SetLeft(plRect, posLeft);
				posLeft += 100;
				foreach(var x in Hearthes1)
                {
					x.Fill = livePoint1;
                }
				foreach (var x in Hearthes2)
				{
					x.Fill = livePoint2;
				}
			}

		}

		private void Window_KeyDown ( object sender, KeyEventArgs e )
		{ 
			Player pl = Players[0];
			if (pl.status != 12)
			{
				if (e.Key == Key.A)
				{
					pl.left = true;
					pl.right = false;
				}
				if (e.Key == Key.D)
				{
					pl.right = true;
					pl.left = false;

				}
				if (e.Key == Key.W)
				{
					pl.skin = 1;
					if (pl.left && pl.down == false)
					{
						pl.status = 4;
						pl.jump = true;
					}
					else if (pl.right && pl.down == false)
					{
						pl.status = 3;
						pl.jump = true;
					}
					else if (!pl.down)
					{
						pl.status = 3;
						pl.jump = true;
					}
				}
				if (e.Key == Key.LeftCtrl)
				{
					if (pl.attackTimer <= 80)
					{
						pl.skin = 1;
						pl.attack = true;
						pl.attackTimer = 480;
					}
				}
			}
			pl = Players[1];
			if (pl.status != 12)
			{
				if (e.Key == Key.Left)
				{
					pl.left = true;
					pl.right = false;
				}
				if (e.Key == Key.Right)
				{
					pl.right = true;
					pl.left = false;

				}
				if (e.Key == Key.Up)
				{
					pl.skin = 1;
					if (pl.left && pl.down == false)
					{
						pl.status = 4;
						pl.jump = true;
					}
					else if (pl.right && pl.down == false)
					{
						pl.status = 3;
						pl.jump = true;
					}
					else if (!pl.down)
					{
						pl.status = 3;
						pl.jump = true;
					}
				}
				if (e.Key == Key.RightCtrl)
				{
					if (pl.attackTimer <= 80)
					{
						pl.skin = 1;
						pl.attack = true;
						pl.attackTimer = 480;
					}
				}
			}
        }
		private void Window_KeyUp ( object sender, KeyEventArgs e )
		{
			Player pl1 = Players[0];
			if (e.Key == Key.A)
			{
				pl1.left = false;
				pl1.status = 0;
			}
			if (e.Key == Key.D)
			{
				pl1.right = false;
				pl1.status = 0;
			}
			Player pl2 = Players[1];
			if (e.Key == Key.Left)
			{
				pl2.left = false;
				pl2.status = 0;
			}
			if (e.Key == Key.Right)
			{
				pl2.right = false;
				pl2.status = 0;
			}


		}
		void generateFullFloor ( )
		{
            runeTimer = 15000;
			livePoint1.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/hearth1.png"));
			livePoint2.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/hearth2.png"));
            dd.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/dd.png"));
            haste.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/haste.png"));

            ImageBrush tFiller = new ImageBrush();
			tFiller.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/floorFiller.png"));
			tFiller.TileMode = TileMode.FlipY;
			tFiller.Stretch = Stretch.Uniform;
			tFiller.AlignmentY = AlignmentY.Top;
			tFiller.Viewport = new Rect(0, 0, 48, 48);
			tFiller.ViewportUnits = BrushMappingMode.Absolute;
			foreach (var x in Game.Children.OfType<Rectangle>())
			{
				if ((string)x.Tag == "platf")
				{
					mapText.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/pl1_2.png"));
					mapText.TileMode = TileMode.FlipY;
					mapText.Stretch = Stretch.Uniform;
					mapText.AlignmentY = AlignmentY.Top;
					mapText.Viewport = new Rect(0, 0, 48, 24);
					mapText.ViewportUnits = BrushMappingMode.Absolute;

					x.Fill = mapText;

					Rectangle filler = new Rectangle
					{
						Width = x.Width,
						Height = Application.Current.MainWindow.Height - (Canvas.GetTop(x) + x.Height),
						Fill = tFiller,
						Tag = "platfF"
					};
					Canvas.SetTop(filler, Canvas.GetTop(x) + x.Height);
					Canvas.SetLeft(filler, Canvas.GetLeft(x));
					needAdd.Add(filler);
				}
				if ((string)x.Tag == "Airplatf")
				{
                    int chance = rnd.Next(0, 100);
                    if(chance < 20)
                    {
                        Rectangle rune = new Rectangle
                        {
                            Width = 30,
                            Height = 30
                        };
                        chance = rnd.Next(1, 3);
                        if(chance == 1)
                        {
                            rune.Fill = dd;
                            rune.Tag = "dd";
                        }
                        else
                        {
                            rune.Fill = haste;
                            rune.Tag = "haste";
                        }
                        needAdd.Add(rune);
                        Canvas.SetTop(rune, Canvas.GetTop(x) - 30);
                        Canvas.SetLeft(rune, rnd.Next((int)Canvas.GetLeft(x), (int)Canvas.GetLeft(x) + (int)x.Width - 30));
                    }
					mapText.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/pl1_2.png"));
					mapText.TileMode = TileMode.FlipY;
					mapText.Stretch = Stretch.Uniform;
					mapText.AlignmentY = AlignmentY.Top;
					mapText.Viewport = new Rect(0, 0, 48, 24);
					mapText.ViewportUnits = BrushMappingMode.Absolute;

					x.Fill = mapText;
				}
				if ((string)x.Tag == "hearth1")
				{
					Hearthes1.Add(x);
				}
				if ((string)x.Tag == "hearth2")
				{
					Hearthes2.Add(x);
				}
			}
			foreach (var f in needAdd)
			{
				Game.Children.Add(f);
			}
			needAdd.Clear();
		}

		private void Window_MouseDown ( object sender, MouseButtonEventArgs e )
		{
			Player pl = Players[0];
			if (e.LeftButton == MouseButtonState.Pressed)
			{
				if (pl.attackTimer <= 80)
				{
					pl.skin = 1;
					pl.attack = true;
					pl.attackTimer = 480;
				}
			}
		}

		class Player
		{
			public int score;
			public int skin = 1;
			public int status = 0;
			public int hp = 120;
			public int attackTimer = 0;
			public int id;
			public Rectangle rect;
			public ImageBrush playerText = new ImageBrush(); //58x56

			public double speed = 0.35, maxSpeed = 3;
			public double hv;
			public double vx = 0, vy = 0;
			public bool up = false, down = false, left = false, right = false, jump = false, flying = false, falling = false, attack = false;
			public bool dead = false;
			public bool alrAttack = false;
            public bool haste = false, dd = false;
            public int ddTimer = 0, hasteTimer = 0;
            public int damage = 30;

		}
	}
}
