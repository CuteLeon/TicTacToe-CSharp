using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 井字棋
{
    public partial class GameForm : Form
    {
        public GameForm()
        {
            InitializeComponent();
        }

        //每个方格对应的棋子类型
        private enum Role
        {
            Player=1,//玩家的棋子
            Empty=0,//空，没有棋子
            AI=-1//AI的棋子
        }
        //游戏表格
        Role[,] Tabel = new Role[3, 3];
        //剩余空表格总数，为0时判断平局
        sbyte EmptyCount = 9;

        private void Form1_Load(object sender, EventArgs e)
        {
            //AI先手
            AIStart();
            //刷新界面
            DrawGame();
        }

        /// <summary>
        /// 判断是否存在一行内有两个我方棋子和一个空格
        /// </summary>
        /// <returns>为真时可以直接获胜</returns>
        private bool ShouldAttack()
        {
            return HaveTwoPiece(Role.AI);
        }

        /// <summary>
        /// 判断是否存在一行内有两个敌方棋子和一个空格
        /// </summary>
        /// <returns>为真时必须拦截</returns>
        private bool ShouldDefend()
        {
            return HaveTwoPiece(Role.Player);
        }

        /// <summary>
        /// 落子，所有落子动作，必须经过此函数，以更新剩余空白方格总数
        /// </summary>
        /// <param name="x">落子位置在表格中横坐标</param>
        /// <param name="y">落子位置在表格中纵坐标</param>
        /// <param name="role">落子所属角色</param>
        private void SetPiece(byte x, byte y, Role role)
        {
            EmptyCount -= 1;
            System.Diagnostics.Debug.Print(string.Format("在 {0},{1} 落下 {2}",x,y, role.ToString()));
            Tabel[x, y] = role;
        }

        /// <summary>
        /// 判断是否存在一行内有两个属于同一方的棋子和一个空格（存在时上述行时，AI会自动落子）
        /// </summary>
        /// <param name="role">两个棋子所属的角色</param>
        /// <returns>为真：存在上述行；为假：不存在上述行</returns>
        private bool HaveTwoPiece(Role role)
        {
            for (byte i = 0; i < 3; i++){
                //横向匹配行
                if (Tabel[i,0] == Tabel[i,1] && Tabel[i,0] ==role && Tabel[i,2] ==  Role.Empty){
                    SetPiece(i,2,Role.AI);return true;
                }else if (Tabel[i, 0] == Tabel[i, 2] && Tabel[i, 0] == role && Tabel[i, 1] ==  Role.Empty){
                    SetPiece(i, 1, Role.AI); return true;
                }else if (Tabel[i, 1] == Tabel[i, 2] && Tabel[i, 1] == role && Tabel[i, 0] ==  Role.Empty){
                    SetPiece(i, 0, Role.AI); return true;}
                //纵向匹配行
                else if (Tabel[0, i] == Tabel[1, i] && Tabel[0, i] == role && Tabel[2, i] ==  Role.Empty){
                    SetPiece(2, i, Role.AI); return true;
                }else if (Tabel[0, i] == Tabel[2, i] && Tabel[0, i] == role && Tabel[1, i] ==  Role.Empty){
                    SetPiece(1, i, Role.AI); return true;
                }else if (Tabel[1,i] == Tabel[2,i] && Tabel[1,i] == role && Tabel[0,i] ==  Role.Empty){
                    SetPiece(0, i, Role.AI);return true;
                }
            }

            //对角线匹配行
            if (Tabel[0,0] == Tabel[1,1] && Tabel[0,0] == role && Tabel[2,2] ==  Role.Empty){
                SetPiece(2,2, Role.AI); return true;
            }else if (Tabel[1,1] == Tabel[2,2] && Tabel[1,1] == role && Tabel[0,0] ==  Role.Empty){
                SetPiece(0, 0, Role.AI); return true;
            }else if (Tabel[0,2] == Tabel[1,1] && Tabel[0,2] == role && Tabel[2,0] ==  Role.Empty){
                SetPiece(2, 0, Role.AI); return true;
            }else if (Tabel[1,1] == Tabel[2,0] && Tabel[1,1] == role && Tabel[0,2] ==  Role.Empty){
                SetPiece(0, 2, Role.AI); return true;
            }else{
                //不存在上述行
                return false;
            }
        }

        //AI先手
        private void AIStart()
        {
            if (ShouldAttack()){
                System.Diagnostics.Debug.Print("AI 发起攻击！");
            }
            else if (ShouldDefend()) {
                System.Diagnostics.Debug.Print("AI 发起拦截！");
            }
            else if (Center()) {
                System.Diagnostics.Debug.Print("AI 占领中心点！");
            }
            else {
                System.Diagnostics.Debug.Print("AI 进入垃圾时间...");
                PlanB();
            }
        }

        /// <summary>
        /// AI 尝试占领中心点
        /// </summary>
        /// <returns>是否占领了中心点</returns>
        public bool Center()
        {
            if (Tabel[1,1] ==  Role.Empty){
                SetPiece(1,1,Role.AI);
                return true;
            }else{
                return false;
            }
        }

        /// <summary>
        /// 垃圾时间（最低优先级）
        /// </summary>
        public void PlanB()
        {
            //先角原则：
            //判断敌方是否占领了对角线两端，若存在则尝试占领剩余两角
            if (Tabel[0, 0] == Tabel[2, 2] && Tabel[0, 0] == Role.Player)
            {
                if (Tabel[0, 2] == Role.Empty) SetPiece(0, 2, Role.AI);
                else if (Tabel[2,0]==  Role.Empty) SetPiece(2, 0, Role.AI);
            }
            else if (Tabel[0, 2] == Tabel[2, 0] && Tabel[0, 2] == Role.Player)
            {
                if (Tabel[0, 0] == Role.Empty) SetPiece(0, 0, Role.AI);
                else if (Tabel[2, 2] == Role.Empty) SetPiece(2, 2, Role.AI);
            }
            //判断敌方是否占领了相邻两棱，若存在则尝试占领两棱相对的一角
            else if ((Tabel[0, 1] == Role.Player && Tabel[1, 0] == Role.Player) && Tabel[0, 0] == Role.Empty)
            {
                SetPiece(0, 0, Role.AI);
            }
            else if ((Tabel[0, 1] == Role.Player && Tabel[1, 2] == Role.Player) && Tabel[0, 2] == Role.Empty)
            {
                SetPiece(0, 2, Role.AI);
            }
            else if ((Tabel[2, 1] == Role.Player && Tabel[1, 0] == Role.Player) && Tabel[2, 0] == Role.Empty)
            {
                SetPiece(2, 0, Role.AI);
            }
            else if ((Tabel[2, 1] == Role.Player && Tabel[1, 2] == Role.Player) && Tabel[2, 2] == Role.Empty)
            {
                SetPiece(2, 2, Role.AI);
            }
            //玩家占领棱位，我方占领对角
            else if ((Tabel[0, 1] == Role.Player || Tabel[1, 0] == Role.Player) && Tabel[2, 2] == Role.Empty)
            {
                SetPiece(2, 2, Role.AI);
            }
            else if ((Tabel[0, 1] == Role.Player || Tabel[1, 2] == Role.Player) && Tabel[2, 0] == Role.Empty)
            {
                SetPiece(2, 0, Role.AI);
            }
            else if ((Tabel[2, 1] == Role.Player || Tabel[1, 0] == Role.Player) && Tabel[0, 2] == Role.Empty)
            {
                SetPiece(0, 2, Role.AI);
            }
            else if ((Tabel[2, 1] == Role.Player || Tabel[1, 2] == Role.Player) && Tabel[0, 0] == Role.Empty)
            {
                SetPiece(0, 0, Role.AI);
            }
            //随便找空角下子
            else if (Tabel[0, 0] == Role.Empty)
            {
                SetPiece(0, 0, Role.AI);
            }
            else if (Tabel[0, 2] == Role.Empty)
            {
                SetPiece(0, 2, Role.AI);
            }
            else if (Tabel[2, 0] == Role.Empty)
            {
                SetPiece(2, 0, Role.AI);
            }
            else if (Tabel[2, 2] == Role.Empty)
            {
                SetPiece(2, 2, Role.AI);
            }
            else if (Tabel[0, 1] == Role.Empty)
            {
                SetPiece(0, 1, Role.AI);
            }
            else if (Tabel[1, 0] == Role.Empty)
            {
                SetPiece(1, 0, Role.AI);
            }
            else if (Tabel[1, 2] == Role.Empty)
            {
                SetPiece(1, 2, Role.AI);
            }
            else if (Tabel[2, 1] == Role.Empty)
                SetPiece(2, 1, Role.AI);
        }

        /// <summary>
        /// 获取获胜者
        /// </summary>
        /// <returns>获胜角色</returns>
        private Role GetWinner()
        {
            for (byte i = 0; i < 3; i++)
            {
                if (Tabel[i, 0] == Tabel[i, 1] && Tabel[i, 0] == Tabel[i,2] && Tabel[i, 0] != 0)
                {
                    return (Role)Tabel[i, 0];
                }
                if (Tabel[0,i] == Tabel[1,i] && Tabel[0,i] == Tabel[2,i] && Tabel[0,i] != 0)
                {
                    return (Role)Tabel[0,i];
                }
            }

            //对角线
            if (Tabel[0, 0] == Tabel[1, 1] && Tabel[1, 1] == Tabel[2, 2] && Tabel[1,1]!=0)
            {
                return (Role)Tabel[1, 1];
            }
            if (Tabel[0, 2] == Tabel[1, 1] && Tabel[1, 1] == Tabel[2, 0] && Tabel[1, 1] != 0)
            {
                return (Role)Tabel[1, 1];
            }
            else
            {
                //平局
                return Role.Empty;
            }
        }

        /// <summary>
        /// 更新游戏画面
        /// </summary>
        private void DrawGame()
        {
            Bitmap B = new Bitmap(300, 300);
            using (Graphics G = Graphics.FromImage(B))
            {
                G.FillRectangle(Brushes.Gray, new Rectangle(0, 0, 300, 300));

                Pen P = new Pen(Brushes.LightGray ,3);
                G.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                G.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

                G.DrawLine(P, new Point(100, 0), new Point(100, 300));
                G.DrawLine(P, new Point(200, 0), new Point(200, 300));
                G.DrawLine(P, new Point(0, 100), new Point(300, 100));
                G.DrawLine(P, new Point(0, 200), new Point(300, 200));

                P = new Pen(Brushes.White,10);
                for(byte x = 0; x < 3; x++)
                {
                    for (byte y = 0; y < 3; y++)
                    {
                        if (Tabel[x, y] == Role.Player){
                            G.DrawLine(P,new Point(x*100+10,y*100+10),new Point(x * 100 + 90, y * 100 + 90));
                            G.DrawLine(P, new Point(x*100+90, y*100+10), new Point(x * 100 + 10, y * 100 + 90));
                        }
                        else if (Tabel[x, y] == Role.AI) {
                            G.DrawEllipse(P, new Rectangle(x * 100+10, y * 100+10, 80, 80));
                        }
                    }
                }
            }
            this.BackgroundImage = B;
            this.Refresh();
            
            //清理所有代托管内存
            GC.Collect();
        }

        private void Form1_MouseClick(object sender, MouseEventArgs e)
        {
            //玩家点击落子
            byte X = (byte)(e.X / (this.Width / 3));
            byte Y = (byte)(e.Y / (this.Height / 3));
            if (Tabel[X,Y] == Role.Empty) {
                SetPiece(X,Y, Role.Player);
                DrawGame();
                AIStart();
                DrawGame();
            
                //判断游戏结束
                Role Winner = GetWinner();
                if (Winner == Role.Empty)
                {
                    if (EmptyCount <= 0)
                    {
                        MessageBox.Show("平局！", "棋逢对手：");
                        System.Diagnostics.Debug.Print("平局");
                        ResetGame();
                    }
                }
                else
                {
                    MessageBox.Show(Winner.ToString(),"获胜者：");
                    System.Diagnostics.Debug.Print("获胜者：" +Winner.ToString());
                    ResetGame();
                }
            }
        }

        /// <summary>
        /// 重新开始游戏
        /// </summary>
        private void ResetGame()
        {
            Debug.Print("游戏结束，重置游戏！");
            Tabel = new Role[3, 3];
            EmptyCount = 9;
            DrawGame();
        }
    }
}