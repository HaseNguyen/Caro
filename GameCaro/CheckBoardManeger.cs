using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GameCaro
{
    public class ChessBoardManeger
    {

        #region Properties
        private Panel chessBoard;

        public Panel ChessBoard { get => chessBoard; set => chessBoard = value; }
        

        private List<Player> players;
        private int currentPlayer;
        public int CurrentPlayer { get => currentPlayer; set => currentPlayer = value; }
      
        private TextBox playerName;
        public TextBox PlayerName { get => playerName; set => playerName = value; }

        private PictureBox playerMark;
        public PictureBox PlayerMark { get => playerMark; set => playerMark = value; }
        

        private List<List<Button>> matrix;
        public List<List<Button>> Matrix { get => matrix; set => matrix = value; }
       

        private event EventHandler playerMarked;
        public event EventHandler PlayerMarked
        {
            add
            {
                playerMarked += value;
            }
            remove
            {
                playerMarked -= value;
            }
        }

        private event EventHandler endedGame;
        public event EventHandler EndedGame
        {
            add
            {
                endedGame += value;
            }
            remove
            {
                endedGame -= value;
            }
        }
        private Stack<PlayInfo> playTimeLine;
        #endregion

        public Stack<PlayInfo> PlayTimeLine { get => playTimeLine; set => playTimeLine = value; }
        public List<Player> Players { get => players; set => players = value; }
        #region Initialize
        public ChessBoardManeger(Panel chessBoard , TextBox playerName, PictureBox mark)
        {
            this.ChessBoard = chessBoard;
            this.PlayerName = playerName;
            this.PlayerMark = mark;
            this.Players = new List<Player>()
            {
                new Player ("Hase",Image.FromFile(Application.StartupPath+"\\Resources\\istockphoto-466688576-612x612.jpg")),
                new Player("Dark Hase", Image.FromFile(Application.StartupPath+"\\Resources\\letter-o-hi.png"))
            };
            PlayTimeLine = new Stack<PlayInfo>() ;

         

        }


        #endregion

        #region Methods
        public void DrawChessBoard()
        {
            ChessBoard.Enabled = true;
            ChessBoard.Controls.Clear();
            CurrentPlayer = 0;
            ChangerPlayer();
            PlayTimeLine = new Stack<PlayInfo>();

            Matrix = new List<List<Button>>();

            Button oldButton = new Button() { Width = 0, Location = new Point(0, 0) };
            for (int i = 0; i < Cons.CHESS_BOARD_HEIGHT; i++)
            {
                Matrix.Add(new List<Button>()); //them vao cot
                for (int j = 0; j < Cons.CHESS_BOARD_WIDTH; j++)
                {
                    Button btn = new Button()
                    {
                        Width = Cons.CHESS_WIDTH,
                        Height = Cons.CHESS_HEIGHT,
                        Location = new Point(oldButton.Location.X + oldButton.Width, oldButton.Location.Y),
                        BackgroundImageLayout = ImageLayout.Stretch,
                        Tag = i.ToString() //xác định vị trí của button
                    };
                    btn.Click += Btn_Click;
                    ChessBoard.Controls.Add(btn);
                    Matrix[i].Add(btn);//add vao dong thu i
                    oldButton = btn;

                }
                oldButton.Location = new Point(0, oldButton.Location.Y + Cons.CHESS_HEIGHT);
                oldButton.Width = 0;
                oldButton.Height = 0;
            }
            
        }

        public void EndGame()
        {
            if (endedGame != null)
                endedGame(this, new EventArgs());
        }
        public bool Undo()
        {
            if(PlayTimeLine.Count <= 0)
            {
                return false;
            }
            PlayInfo oldPoint = PlayTimeLine.Pop();
            Button btn = Matrix[oldPoint.Point.Y][oldPoint.Point.X];

            btn.BackgroundImage = null;
            
            if(PlayTimeLine.Count <= 0)
    
            {
                CurrentPlayer = 0;
            }
            else
            {
                oldPoint = PlayTimeLine.Peek();
                CurrentPlayer = oldPoint.CurrentPlayer == 1 ? 0 : 1; 
            }
  
         
                ChangerPlayer();

            return true ;
        }
        private bool isEndGame(Button btn)
        {
            return isEndHorizontal(btn) || isEndVertical(btn) || isEndprimary(btn) ||isEndDiagonal(btn);
        }
        private Point GetChessPoint(Button btn) //hàm lấy vị trí của button
        {
            
            int vertical = Convert.ToInt32(btn.Tag);
            int horizontal = Matrix[vertical].IndexOf(btn); // lấy vị trí chiều ngang của ma trận
            Point point = new Point(horizontal , vertical);
            return point;
        }
        private bool isEndHorizontal(Button btn) //kết thúc ở hàng ngang    
        {
            Point point = GetChessPoint(btn);
            int countLeft = 0;
            for (int i = point.X; i >=0 ; i--)
            {
                if (Matrix[point.Y][i].BackgroundImage == btn.BackgroundImage)
                {
                    countLeft++;
                }
                else
                    break;
            }
            int countRight = 0;
            for (int i = point.X +1; i < Cons.CHESS_BOARD_WIDTH; i++)
            {
                if (Matrix[point.Y][i].BackgroundImage == btn.BackgroundImage)
                {
                    countRight++;
                }
                else
                    break;
            }

            return countLeft + countRight ==5 ;
        }

        private bool isEndVertical(Button btn) //kết thúc ở hàng dọc
        {
            Point point = GetChessPoint(btn);
            int countTop = 0;
            for (int i = point.Y; i >= 0; i--)
            {
                if (Matrix[i][point.X].BackgroundImage == btn.BackgroundImage)
                {
                    countTop++;
                }
                else
                    break;
            }
            int countBottom = 0;
            for (int i = point.Y + 1; i < Cons.CHESS_BOARD_HEIGHT; i++)
            {
                if ((Matrix[i][point.X].BackgroundImage == btn.BackgroundImage))
                {
                    countBottom++;
                }
                else
                    break;
            }

            return countTop + countBottom == 5;
          
        }
        private bool isEndprimary(Button btn) //end ở đường chéo 1
        {
            Point point = GetChessPoint(btn);
            int countTop = 0;
            for (int i = 0; i <= point.X; i++)
            {
                if (point.X - i < 0 || point.Y - i < 0)
                {
                    break;
                }   
                if (Matrix[point.Y-i][point.X -i].BackgroundImage == btn.BackgroundImage)
                {   
                    countTop++;
                }
                else
                    break;
            }
            int countBottom = 0;
            for (int i = 1; i <= Cons.CHESS_BOARD_WIDTH - point.X; i++)
            {

                if (point.Y +i >= Cons.CHESS_BOARD_HEIGHT || point.X + i >= Cons.CHESS_BOARD_WIDTH)
                {
                    break;
                }
              
                if ((Matrix[point.Y + i][point.X + i].BackgroundImage == btn.BackgroundImage))
                {
                    countBottom++;
                }
                else
                    break;
            }

            return countTop + countBottom == 5;

          
        }
        private bool isEndDiagonal(Button btn) //end ở đường chéo 2
        {
            Point point = GetChessPoint(btn);
            int countTop = 0;
            for (int i = 0; i <= point.X; i++)
            {
                if (point.X + i > Cons.CHESS_BOARD_WIDTH || point.Y - i < 0)
                {
                    break;
                }
                if (Matrix[point.Y - i][point.X +i].BackgroundImage == btn.BackgroundImage)
                {
                    countTop++;
                }
                else
                    break;
            }
            int countBottom = 0;
            for (int i = 1; i < Cons.CHESS_BOARD_WIDTH - point.X; i++)
            {

                if (point.Y + i >= Cons.CHESS_BOARD_HEIGHT || point.X - i < 0)
                {
                    break;
                }

                if ((Matrix[point.Y + i][point.X - i].BackgroundImage == btn.BackgroundImage))
                {
                    countBottom++;
                }
                else
                    break;
            }

            return countTop + countBottom == 5;

            return false;
        }

        private void Btn_Click(object sender, EventArgs e)
        {
            //Xu ly button khi click cho ra X hay O
            Button btn = sender as Button;
            if (btn.BackgroundImage != null) 
            
                return;

            Mark(btn);

            PlayTimeLine.Push(new PlayInfo (GetChessPoint(btn), CurrentPlayer)); //lấy tọa độ ra 
            
            CurrentPlayer = CurrentPlayer == 1 ? 0 : 1;
            
            ChangerPlayer();

            if (playerMarked != null)
            {
                playerMarked(this, new EventArgs());
            }


            if (isEndGame(btn))
            {
                EndGame();
            }

           
        }
        private void Mark(Button btn)
        {
            btn.BackgroundImage = Players[CurrentPlayer].Mark;

          

        }
        private void ChangerPlayer()
        {
            PlayerName.Text = Players[CurrentPlayer].Name;
            PlayerMark.Image = Players[CurrentPlayer].Mark;

        }
        #endregion

    }
}
