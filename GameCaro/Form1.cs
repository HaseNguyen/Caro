using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GameCaro
{
    public partial class Form1 : Form
    {
        #region Properties
        ChessBoardManeger ChessBoard;
        #endregion
        public Form1()
        {
            InitializeComponent();
            ChessBoard = new ChessBoardManeger(pnlChessBoard , txbPlayerName , pctbMark);
            ChessBoard.EndedGame += CheckBoard_EndedGame;
            ChessBoard.PlayerMarked += CheckBoard_PlayerMarked;

            prcbCoolDown.Step = Cons.COOL_DOWN_STEP;
            prcbCoolDown.Maximum = Cons.COOL_DOWN_TIME;
            prcbCoolDown.Value = 0;
            tmCoolDown.Interval = Cons.COOL_DOWN_INTERVAL;



            tmCoolDown.Start();
            NewGame();
        }

    
        #region New Quit Game
        void EndGame()
        {
            tmCoolDown.Stop();
            MessageBox.Show("End");
            pnlChessBoard.Enabled = false;
            undoToolStripMenuItem.Enabled = false;
          
        }
        void NewGame()
        {
            prcbCoolDown.Value = 0;
            tmCoolDown.Stop();
            ChessBoard.DrawChessBoard();
            undoToolStripMenuItem.Enabled = true;
        }
        void Quit()
        {
            if(MessageBox.Show("Bạn có muốn thoát","Thông báo",MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK)
            Application.Exit();
        }
        void Undo()
        {
            ChessBoard.Undo();
        }
        #endregion
        private void CheckBoard_PlayerMarked(object sender, EventArgs e)
        {
            tmCoolDown.Start();
            prcbCoolDown.Value = 0;
        }

        private void CheckBoard_EndedGame(object sender, EventArgs e)
        {
            EndGame();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void TmCoolDown_Tick(object sender, EventArgs e)
        {
            prcbCoolDown.PerformStep();
            if(prcbCoolDown.Value >= prcbCoolDown.Maximum)
            {
                tmCoolDown.Stop();
                EndGame();
               
            }
        }


        private void NewGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewGame();
        }

        private void UndoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Undo();
        }

        private void QuitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Quit();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {

            if (MessageBox.Show("Bạn có muốn thoát", "Thông báo", MessageBoxButtons.OKCancel) != System.Windows.Forms.DialogResult.OK)
         
            e.Cancel = true;
        }
    }
}
