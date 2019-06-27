using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
namespace Siemens.Simatic.WechatTest
{
    public class AutoSizeForm
    {
        /// <summary>
        /// 声明一个结构，用于保存控件位置的基本属性。
        /// </summary>
        public struct controlRect
        {
            /// <summary>
            /// 控件的left属性
            /// </summary>
            public int Left;
            /// <summary>
            /// 控件的Right属性
            /// </summary>
            public int Top;
            /// <summary>
            /// 控件的Weight属性
            /// </summary>
            public int Width;
            /// <summary>
            /// 控件的High属性
            /// </summary>
            public int Height;
            /// <summary>
            /// 控件的Fontsize属性
            /// </summary>
            public float FontSize;
        }

        /// <summary>
        /// 声明一个泛型，类型为什么的保存控件属性的结构类，
        /// </summary>
        public List<controlRect> oldCtrl = new List<controlRect>();
        int ctrlNo = 0;//初始化标识控件的变量为0，表示窗体本身。   

        /// <summary>
        /// 保存控件的位置和大小信息
        /// </summary>
        /// <param name="ctl">需要被保存的控件</param>
        private void AddControl(Control ctl)
        {
            foreach (Control c in ctl.Controls)
            {
                controlRect objCtrl;
                objCtrl.Left = c.Left;
                objCtrl.Top = c.Top;
                objCtrl.Width = c.Width;
                objCtrl.Height = c.Height;
                objCtrl.FontSize = c.Font.Size;
                oldCtrl.Add(objCtrl);
                //**放在这里，是先记录控件本身，后记录控件的子控件，重点是前后要一致
                if (c.Controls.Count > 0)
                    AddControl(c);//窗体内其余控件还可能嵌套控件(比如panel),要单独抽出,因为要递归调用
            }
        }

        /// <summary>
        /// 窗体自适应分辨率类
        /// </summary>
        /// <param name="mForm">需要进行设置的窗体</param>
        public void controlAutoSize(Control mForm)
        {
            if (ctrlNo == 0)
            { //*如果在窗体的Form1_Load中，记录控件原始的大小和位置，正常没有问题，但要加入皮肤就会出现问题，因为有些控件如dataGridView的的子控件还没有完成，个数少
                //*要在窗体的Form1_SizeChanged中，第一次改变大小时，记录控件原始的大小和位置,这里所有控件的子控件都已经形成
                controlRect cR;
                cR.Left = mForm.Left;
                cR.Top = mForm.Top;
                cR.Width = mForm.Width;
                cR.Height = mForm.Height;
                //cR.Width = int.Parse(mForm.Tag.ToString().Split(',')[0]);
                //cR.Height = int.Parse(mForm.Tag.ToString().Split(',')[1]);
                cR.FontSize = mForm.Font.Size;
                oldCtrl.Add(cR);//第一个为"窗体本身",只加入一次即可                
                AddControl(mForm);//窗体内其余控件可能嵌套其它控件(比如panel),故单独抽出以便递归调用
            }
            float wScale = (float)mForm.Width / (float)oldCtrl[0].Width;//新旧窗体之间的比例，与最早的旧窗体比较
            float hScale = (float)mForm.Height / (float)oldCtrl[0].Height;//.Height;
            ctrlNo = 1;//进入=1，第0个为窗体本身,窗体内的控件,从序号1开始
            AutoScaleControl(mForm, wScale, hScale);//窗体内其余控件还可能嵌套控件(比如panel),要单独抽出,因为要递归调用
        }

        /// 设置控件的属性
        /// </summary>
        /// <param name="ctl">需要设置的控件</param>
        /// <param name="wScale">调整的高度比例</param>
        /// <param name="hScale">调整的宽度比例</param>
        private void AutoScaleControl(Control ctl, float wScale, float hScale)
        {
            int ctrLeft0, ctrTop0, ctrWidth0, ctrHeight0;
            float ctrFontSize0;
            //第1个是窗体自身的 Left,Top,Width,Height，所以窗体控件从ctrlNo=1开始
            foreach (Control c in ctl.Controls)
            {
                //获得控件原有的位置和大小信息
                ctrLeft0 = oldCtrl[ctrlNo].Left;
                ctrTop0 = oldCtrl[ctrlNo].Top;
                ctrWidth0 = oldCtrl[ctrlNo].Width;
                ctrHeight0 = oldCtrl[ctrlNo].Height;
                ctrFontSize0 = oldCtrl[ctrlNo].FontSize;
                //设置控件新的位置和大小信息。
                c.Left = (int)((ctrLeft0) * wScale);//新旧控件之间的线性比例。控件位置只相对于窗体
                c.Top = (int)((ctrTop0) * hScale);//
                c.Width = (int)(ctrWidth0 * wScale);//只与最初的大小相关，所以不能与现在的宽度相乘 
                c.Height = (int)(ctrHeight0 * hScale);//
                c.Font = new Font(c.Font.Name, (float)(ctrFontSize0 * wScale));//设置控件中字体的大小以适应控件的大小
                ctrlNo++;//累加序号
                //**放在这里，是先缩放控件本身，后缩放控件的子控件，重点是前后要一致（与保存时）
                if (c.Controls.Count > 0)
                    AutoScaleControl(c, wScale, hScale);//窗体内其余控件还可能嵌套控件(比如panel),要单独抽出,因为要递归调用
            }
        }



    }
}